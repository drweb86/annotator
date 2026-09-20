# Privacy Statement
Last Updated: 20 September 2026

**Screenshot Annotator** by Siarhei Kuchuk

Application name: Screenshot Annotator
Developer name: Siarhei Kuchuk

The software captures screenshots on this computer, lets you annotate them, and can read text from a selected area with OCR. It does not create cloud accounts. The developer does not operate a backend that receives your screenshots, projects, or usage data.

Translations live in [privacy/desktop](privacy/desktop/README.md).

## Data the developer does not collect

The app does not include ads, analytics, crash reporters, or tracking SDKs. The developer does not collect, sell, or share personal data.

## Data stored on your computer

### Settings

Application settings (including the Print Screen hotkey, start-with-system preference, OCR engine and language choices, highlighter color, and the language last chosen in the License or Privacy window) are stored only on this computer:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Projects and images

Annotated projects and preview images are stored in your Pictures folder:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Screenshots you take, images you import, and text you type into annotations stay in those local files (or on the clipboard if you copy them). The app does not upload them.

### Logs

Diagnostic logs may be written under:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

If the app crashes, it may write a local bug report file on your Desktop. That file is not sent anywhere.

Those values are not uploaded to the developer.

No developer server is used to store your data.

## Screen capture and OCR

When you use Print Screen (or the screenshot command), the app captures the current screen so you can crop and annotate it. Capture happens on this device only.

OCR runs locally:

- **Windows OCR** uses the operating system’s `Windows.Media.Ocr` APIs on this PC.
- **Tesseract** runs on this computer if you installed it and it is on PATH.

Recognized text is shown in the app so you can copy or edit it. It is not sent to the developer.

## Network use

### Update check

Non-Store builds may request the latest GitHub release:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) receives a normal HTTPS request (IP address, user-agent, time). The developer does not receive that traffic.

Installs from the Microsoft Store do not use this check; the Store delivers updates.

### Links you open

The app can open these pages in your system browser. Those sites have their own privacy policies:

- Project homepage: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Latest release: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

The license is shown inside the app. It is not opened as a web page.

## Other local behavior

You can ask the app to start when you sign in to Windows or Linux. On Windows that uses a startup entry (or a Microsoft Store startup task for Store installs). On Linux it uses an autostart entry. That only launches this app on your computer.

The optional global Print Screen hotkey stays in memory while the app is running so it can open the selector.

## Children

The app is a screenshot annotation tool. It is not directed at children under 13.

## Third parties

GitHub processes the update-check request and pages you open, as above. The Microsoft Store processes Store installs and updates. Windows OCR is provided by the operating system. The developer does not receive that traffic.

## Changes

Updates to this policy will be posted in this file in the project repository.

## Contact

Application name: Screenshot Annotator
Developer name: Siarhei Kuchuk

Questions: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
