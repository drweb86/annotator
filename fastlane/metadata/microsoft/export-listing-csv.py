"""Fill a Partner Center listing CSV from fastlane/metadata/microsoft/{locale}/ files."""

from __future__ import annotations

import csv
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent
TEMPLATE = Path(
    r"C:\Users\siarh\Downloads\listingData-9NFPJCS6R9R6-1152921505701937539 (1).csv"
)
DEVELOPER = "Siarhei Kuchuk"
TITLE = "Screenshot Annotator"
SCREENSHOT_URLS = [
    "https://developer.microsoft.com/en-us/dashboard/apps/9NFPJCS6R9R6/submissions/1152921505701937539/listings/1152922700027662560/listingassets/3040704988215426762",
    "https://developer.microsoft.com/en-us/dashboard/apps/9NFPJCS6R9R6/submissions/1152921505701937539/listings/1152922700027662560/listingassets/3057176470511919281",
    "https://developer.microsoft.com/en-us/dashboard/apps/9NFPJCS6R9R6/submissions/1152921505701937539/listings/1152922700027662560/listingassets/3038862810310318415",
    "https://developer.microsoft.com/en-us/dashboard/apps/9NFPJCS6R9R6/submissions/1152921505701937539/listings/1152922700027662560/listingassets/3049989495359416536",
]

LIMITS = {
    "Description": 10000,
    "ReleaseNotes": 1500,
    "Title": 50,
    "ShortDescription": 1000,
    "Feature": 200,
    "SearchTerm": 40,
}


def read_text(path: Path) -> str:
    return path.read_text(encoding="utf-8").replace("\r\n", "\n").strip("\n")


def read_lines(path: Path) -> list[str]:
    return [line.strip() for line in read_text(path).split("\n") if line.strip()]


def load_locale(folder: Path) -> dict[str, str]:
    values: dict[str, str] = {
        "Title": TITLE,
        "ShortDescription": read_text(folder / "short_description.txt").strip(),
        "Description": read_text(folder / "description.txt").strip(),
        "ReleaseNotes": read_text(folder / "release_notes.txt").strip(),
        "DevStudio": DEVELOPER,
    }
    features = read_lines(folder / "features.txt")
    keywords = read_lines(folder / "keywords.txt")
    if len(features) != 18:
        raise SystemExit(f"{folder.name}: expected 18 features, got {len(features)}")
    if len(keywords) != 7:
        raise SystemExit(f"{folder.name}: expected 7 keywords, got {len(keywords)}")
    for i, feature in enumerate(features, start=1):
        values[f"Feature{i}"] = feature
    for i, keyword in enumerate(keywords, start=1):
        values[f"SearchTerm{i}"] = keyword
    for i, url in enumerate(SCREENSHOT_URLS, start=1):
        values[f"DesktopScreenshot{i}"] = url
    return values


def validate(locale: str, values: dict[str, str]) -> None:
    errors: list[str] = []
    if len(values["Description"]) > LIMITS["Description"]:
        errors.append(f"Description {len(values['Description'])} > {LIMITS['Description']}")
    if len(values["ReleaseNotes"]) > LIMITS["ReleaseNotes"]:
        errors.append(f"ReleaseNotes {len(values['ReleaseNotes'])} > {LIMITS['ReleaseNotes']}")
    if len(values["Title"]) > LIMITS["Title"]:
        errors.append(f"Title {len(values['Title'])} > {LIMITS['Title']}")
    if len(values["ShortDescription"]) > LIMITS["ShortDescription"]:
        errors.append(f"ShortDescription {len(values['ShortDescription'])} > {LIMITS['ShortDescription']}")
    word_count = 0
    for i in range(1, 19):
        feature = values[f"Feature{i}"]
        if len(feature) > LIMITS["Feature"]:
            errors.append(f"Feature{i} {len(feature)} > {LIMITS['Feature']}")
    for i in range(1, 8):
        term = values[f"SearchTerm{i}"]
        word_count += len(term.split())
        if len(term) > LIMITS["SearchTerm"]:
            errors.append(f"SearchTerm{i} {len(term)} > {LIMITS['SearchTerm']}")
    if word_count > 21:
        errors.append(f"search terms have {word_count} words > 21")
    if errors:
        raise SystemExit(f"{locale}: " + "; ".join(errors))


def main() -> int:
    csv_path = Path(sys.argv[1]) if len(sys.argv) > 1 else TEMPLATE
    if not csv_path.exists():
        raise SystemExit(f"CSV not found: {csv_path}")

    locale_dirs = {
        path.name.lower(): path
        for path in ROOT.iterdir()
        if path.is_dir() and (path / "description.txt").exists()
    }
    locales = sorted(locale_dirs)
    if "en-us" not in locales:
        raise SystemExit("en-us metadata is missing")

    translations = {locale: load_locale(locale_dirs[locale]) for locale in locales}
    for locale, values in translations.items():
        validate(locale, values)

    with csv_path.open("r", encoding="utf-8-sig", newline="") as handle:
        rows = list(csv.reader(handle))
    if not rows:
        raise SystemExit("CSV is empty")

    header = rows[0]
    locale_indexes = {
        name.lower(): index
        for index, name in enumerate(header)
        if index >= 3 and name
    }
    skipped = [locale for locale in locales if locale not in locale_indexes]
    if skipped:
        print("WinGet-only locales, not in the Store CSV: " + ", ".join(skipped))

    field_row_indexes = {
        row[0]: index
        for index, row in enumerate(rows[1:], start=1)
        if row and row[0]
    }

    for locale, values in translations.items():
        if locale not in locale_indexes:
            continue
        column = locale_indexes[locale]
        for field, value in values.items():
            row_index = field_row_indexes.get(field)
            if row_index is None:
                raise SystemExit(f"CSV is missing field {field}")
            row = rows[row_index]
            while len(row) < len(header):
                row.append("")
            row[column] = value

    with csv_path.open("w", encoding="utf-8-sig", newline="") as handle:
        writer = csv.writer(handle, lineterminator="\r\n")
        writer.writerows(rows)

    print(f"Wrote {len(locales) - len(skipped)} locales into {csv_path}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
