[Languages](README.md)

# Datenschutzerklärung

Zuletzt aktualisiert: 20. September 2026

**Screenshot Annotator** by Siarhei Kuchuk

Anwendungsname: Screenshot Annotator
Entwicklername: Siarhei Kuchuk

Die Software erfasst Screenshots auf diesem Computer, ermöglicht deren Annotation und kann Text aus einem ausgewählten Bereich per OCR lesen. Sie erstellt keine Cloud-Konten. Der Entwickler betreibt kein Backend, das Ihre Screenshots, Projekte oder Nutzungsdaten empfängt.

## Daten, die der Entwickler nicht erhebt

Die App enthält keine Werbung, keine Analyse-, Absturzmelde- oder Tracking-SDKs. Der Entwickler erhebt, verkauft oder teilt keine personenbezogenen Daten.

## Daten, die auf Ihrem Computer gespeichert werden

### Einstellungen

Anwendungseinstellungen (einschließlich der Print-Screen-Tastenkombination, der Option „Mit dem System starten“, OCR-Engine und Sprachauswahl, Textmarkerfarbe sowie der zuletzt im Lizenz- oder Datenschutzfenster gewählten Sprache) werden nur auf diesem Computer gespeichert:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Projekte und Bilder

Annotierte Projekte und Vorschaubilder werden in Ihrem Bilder-Ordner gespeichert:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Screenshots, die Sie aufnehmen, importierte Bilder und Text in Annotationen bleiben in diesen lokalen Dateien (oder in der Zwischenablage, wenn Sie sie kopieren). Die App lädt sie nicht hoch.

### Protokolle

Diagnoseprotokolle können geschrieben werden unter:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

Wenn die App abstürzt, kann sie eine lokale Fehlerberichtsdatei auf Ihrem Desktop schreiben. Diese Datei wird nirgendwohin gesendet.

Diese Werte werden nicht an den Entwickler hochgeladen.

Es wird kein Entwicklerserver zum Speichern Ihrer Daten verwendet.

## Bildschirmaufnahme und OCR

Wenn Sie Print Screen (oder den Screenshot-Befehl) verwenden, erfasst die App den aktuellen Bildschirm, damit Sie ihn zuschneiden und annotieren können. Die Aufnahme erfolgt nur auf diesem Gerät.

OCR läuft lokal:

- **Windows OCR** verwendet die `Windows.Media.Ocr`-APIs des Betriebssystems auf diesem PC.
- **Tesseract** läuft auf diesem Computer, wenn Sie es installiert haben und es im PATH liegt.

Erkannter Text wird in der App angezeigt, damit Sie ihn kopieren oder bearbeiten können. Er wird nicht an den Entwickler gesendet.

## Netzwerknutzung

### Updateprüfung

Installationen außerhalb des Stores können die neueste GitHub-Version anfordern:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) erhält eine normale HTTPS-Anfrage (IP-Adresse, User-Agent, Uhrzeit). Der Entwickler empfängt diesen Datenverkehr nicht.

Installationen aus dem Microsoft Store nutzen diese Prüfung nicht; der Store liefert Updates.

### Links, die Sie öffnen

Die App kann diese Seiten im Systembrowser öffnen. Diese Websites haben eigene Datenschutzrichtlinien:

- Projekt-Homepage: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Neueste Version: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

Die Lizenz wird in der App angezeigt. Sie wird nicht als Webseite geöffnet.

## Weiteres lokales Verhalten

Sie können die App beim Anmelden unter Windows oder Linux starten lassen. Unter Windows verwendet das einen Starteintrag (oder eine Microsoft-Store-Startaufgabe bei Store-Installationen). Unter Linux wird ein Autostart-Eintrag verwendet. Das startet nur diese App auf Ihrem Computer.

Die optionale globale Print-Screen-Tastenkombination bleibt im Speicher, solange die App läuft, damit sie den Auswahlbildschirm öffnen kann.

## Kinder

Die App ist ein Werkzeug zur Screenshot-Annotation. Sie richtet sich nicht an Kinder unter 13 Jahren.

## Dritte

GitHub verarbeitet die Updateprüfung und die Seiten, die Sie öffnen, wie oben. Der Microsoft Store verarbeitet Store-Installationen und Updates. Windows OCR wird vom Betriebssystem bereitgestellt. Der Entwickler empfängt diesen Datenverkehr nicht.

## Änderungen

Aktualisierungen dieser Richtlinie werden in dieser Datei im Projekt-Repository veröffentlicht.

## Kontakt

Anwendungsname: Screenshot Annotator
Entwicklername: Siarhei Kuchuk

Fragen: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
