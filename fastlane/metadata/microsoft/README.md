# Microsoft Store listing metadata

One folder per Partner Center CSV locale (`en-us`, `de-de`, `zh-hk`, …).
Folder names match the language columns in the Store listing export.

| File | CSV field |
|---|---|
| `name.txt` | Title |
| `short_description.txt` | ShortDescription |
| `description.txt` | Description |
| `features.txt` | Feature1–Feature18 (one line each) |
| `keywords.txt` | SearchTerm1–SearchTerm7 (one line each) |
| `release_notes.txt` | ReleaseNotes |

Screenshots are not duplicated. The export script copies the English Partner Center asset URLs into every language column.

Regenerate the listing CSV:

```
python fastlane/metadata/microsoft/export-listing-csv.py
```
