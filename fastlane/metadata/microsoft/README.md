# Microsoft Store listing metadata

One folder per language (`en-us`, `de-de`, `zh-hk`, …).
Folder names match the language columns in the Store listing export when that language is listed there.

| File | CSV field |
|---|---|
| `short_description.txt` | ShortDescription |
| `description.txt` | Description |
| `features.txt` | Feature1–Feature18 (one line each) |
| `keywords.txt` | SearchTerm1–SearchTerm7 (one line each) |
| `release_notes.txt` | ReleaseNotes |

The listing title is always Screenshot Annotator. The export script writes that title into every language column. Screenshots are not duplicated: the script copies the English Partner Center asset URLs into every language column.

Folders that have no column in the Store CSV are still used for WinGet locale manifests. The export skips those folders.

WinGet locale manifests are generated from this folder. `short_description.txt` becomes ShortDescription, `description.txt` and `features.txt` are combined into Description, and `keywords.txt` becomes Tags. That copy is not stored in `.resx`.

Regenerate the listing CSV:

```
python fastlane/metadata/microsoft/export-listing-csv.py
```
