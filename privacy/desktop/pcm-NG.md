[Languages](README.md)

# Privacy policy

Last update: 20 September 2026

**Screenshot Annotator** by Siarhei Kuchuk

App name: Screenshot Annotator
Developer name: Siarhei Kuchuk

Di software dey take screenshot for dis computer, e go let you annotate dem, and e fit read text from area wey you select with OCR. E no dey create cloud account. Di developer no dey run any server wey go receive your screenshot, project or usage data.

## Data wey di developer no dey collect

Di app no get ads, analytics, crash reporter or tracking SDK. Di developer no dey collect, sell or share personal data.

## Data wey dey store for your computer

### Settings

App settings (Print Screen hotkey, start with system, OCR engine and languages, highlighter colour, and di last language for License or Privacy window) dey store only for dis computer:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Projects and images

Annotated projects and previews dey store for your Pictures folder:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Screenshots, imported images and annotation text remain for those local files (or clipboard if you copy). Di app no dey upload dem.

### Logs

Diagnostic logs fit write under:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

If di app crash, e fit write local error report file for Desktop. Dat file no dey send anywhere.

Dem no dey upload those values to di developer.

No developer server dey used to store your data.

## Screen capture and OCR

When you use Print Screen (or screenshot command), di app capture di current screen so you fit crop and annotate. Capture dey happen for dis device only.

OCR dey run locally:

- **Windows OCR** dey use di operating system `Windows.Media.Ocr` APIs for dis PC.
- **Tesseract** dey run for dis computer if e dey installed and for PATH.

Recognised text dey show for di app so you fit copy or edit. E no dey send to di developer.

## Network use

### Update check

Builds wey no be Store fit request di latest GitHub release:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) dey receive normal HTTPS request (IP address, user-agent, time). Di developer no dey receive dat traffic.

Microsoft Store installs no dey use dis check; Store dey deliver updates.

### Links wey you open

Di app fit open these pages for system browser. Those sites get their own privacy policy:

- Project homepage: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Latest release: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

Di license dey show inside di app. E no dey open as web page.

## Other local behaviour

You fit start di app when you sign in for Windows or Linux. For Windows e dey use startup entry (or Microsoft Store startup task for Store install). For Linux e dey use autostart entry. Dat one only start dis app for your computer.

Optional global Print Screen hotkey dey remain for memory while di app dey run so selector fit open.

## Children

Di app na screenshot annotation tool. E no dey directed at children under 13.

## Third parties

GitHub dey process di update check and pages wey you open, as above. Microsoft Store dey process Store installs and updates. Windows OCR na di operating system dey provide. Di developer no dey receive dat traffic.

## Changes

Updates to dis policy go post for dis file for di project repository.

## Contact

App name: Screenshot Annotator
Developer name: Siarhei Kuchuk

Questions: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
