[Languages](README.md)

# Integritetspolicy

Senast uppdaterad: 20 september 2026

**Screenshot Annotator** by Siarhei Kuchuk

Programnamn: Screenshot Annotator
Utvecklarens namn: Siarhei Kuchuk

Programmet tar skärmbilder på den här datorn, låter dig annotera dem och kan läsa text i ett valt område med OCR. Det skapar inga molnkonton. Utvecklaren driver ingen server som tar emot dina skärmbilder, projekt eller användningsdata.

## Data som utvecklaren inte samlar in

Appen innehåller ingen reklam och inga SDK:er för analys, kraschrapporter eller spårning. Utvecklaren samlar inte in, säljer eller delar personuppgifter.

## Data som lagras på din dator

### Inställningar

Programinställningar (Print Screen-kortkommando, start med systemet, OCR-motor och språk, överstrykningsfärg och senast valda språk i fönstret Licens eller Integritet) lagras bara på den här datorn:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Projekt och bilder

Annoterade projekt och förhandsvisningar lagras i mappen Bilder:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Skärmbilder, importerade bilder och annoteringstext stannar i de lokala filerna (eller i urklipp om du kopierar). Appen laddar inte upp dem.

### Loggar

Diagnostikloggar kan skrivas under:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

Om appen kraschar kan en lokal felrapport skrivas på skrivbordet. Filen skickas ingenstans.

Värdena laddas inte upp till utvecklaren.

Ingen serversida hos utvecklaren används för att lagra dina data.

## Skärminfångning och OCR

När du använder Print Screen (eller skärmbildskommandot) fångar appen den aktuella skärmen så att du kan beskära och annotera. Infångningen sker bara på den här enheten.

OCR körs lokalt:

- **Windows OCR** använder operativsystemets `Windows.Media.Ocr`-API:er på den här datorn.
- **Tesseract** körs på den här datorn om det är installerat och finns i PATH.

Igenkänd text visas i appen så att du kan kopiera eller redigera. Den skickas inte till utvecklaren.

## Nätverksanvändning

### Uppdateringskontroll

Byggen utanför Store kan begära den senaste GitHub-utgåvan:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) tar emot en vanlig HTTPS-begäran (IP-adress, user-agent, tid). Utvecklaren tar inte emot den trafiken.

Installationer från Microsoft Store använder inte den här kontrollen; Store levererar uppdateringar.

### Länkar du öppnar

Appen kan öppna de här sidorna i systemets webbläsare. Webbplatserna har egna integritetspolicyer:

- Projekthemsida: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Senaste utgåva: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

Licensen visas i appen. Den öppnas inte som en webbsida.

## Annat lokalt beteende

Du kan starta appen vid inloggning på Windows eller Linux. På Windows används en startpost (eller en Microsoft Store-startuppgift för Store-installationer). På Linux en autostart-post. Det startar bara den här appen på din dator.

Det valfria globala Print Screen-kortkommandot stannar i minnet medan appen körs så att väljaren kan öppnas.

## Barn

Appen är ett verktyg för att annotera skärmbilder. Den riktar sig inte till barn under 13 år.

## Tredje parter

GitHub behandlar uppdateringskontrollen och sidor du öppnar enligt ovan. Microsoft Store behandlar Store-installationer och uppdateringar. Windows OCR tillhandahålls av operativsystemet. Utvecklaren tar inte emot den trafiken.

## Ändringar

Uppdateringar av den här policyn publiceras i den här filen i projektarkivet.

## Kontakt

Programnamn: Screenshot Annotator
Utvecklarens namn: Siarhei Kuchuk

Frågor: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
