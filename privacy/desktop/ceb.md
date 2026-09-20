[Languages](README.md)

# Palisi sa pagkapribado

Katapusang gi-update: 20 Septyembre 2026

**Screenshot Annotator** by Siarhei Kuchuk

Ngalan sa aplikasyon: Screenshot Annotator
Ngalan sa developer: Siarhei Kuchuk

Ang software magkuha og mga screenshot niini nga kompyuter, motugot nimo nga mag-annotate, ug makabasa og teksto gikan sa gipili nga dapit pinaagi sa OCR. Dili kini maghimo og cloud account. Ang developer wala magpadagan og server nga modawat sa imong mga screenshot, proyekto, o datos sa paggamit.

## Datos nga dili kolektahon sa developer

Walay ads, analytics, crash reporter, o tracking SDK ang app. Ang developer dili magkolekta, magbaligya, o magpaambit og personal nga datos.

## Datos nga gitipigan sa imong kompyuter

### Mga setting

Ang mga setting sa aplikasyon (Print Screen hotkey, pagsugod uban sa sistema, OCR engine ug mga pinulongan, kolor sa highlighter, ug katapusang pinulongan sa bintana sa Lisensya o Pagkapribado) gitipigan ra niini nga kompyuter:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Mga proyekto ug imahe

Ang mga naka-annotate nga proyekto ug preview gitipigan sa folder sa Mga Hulagway:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Ang mga screenshot, gi-import nga imahe, ug teksto sa anotasyon magpabilin nianang lokal nga mga file (o clipboard kung imong kopyahon). Ang app dili mag-upload niini.

### Mga log

Mahimong isulat ang diagnostic log sa:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

Kung mag-crash ang app, mahimo kining magsulat og lokal nga file sa report sa sayop sa Desktop. Dili ipadala kana nga file bisan asa.

Dili i-upload kadtong mga bili ngadto sa developer.

Walay server sa developer nga gigamit sa pagtipig sa imong datos.

## Pagkuha sa screen ug OCR

Kung mogamit ka og Print Screen (o screenshot command), ang app magkuha sa kasamtangang screen aron imong ma-crop ug ma-annotate. Ang pagkuha mahitabo niini nga device ra.

Ang OCR molihok lokal:

- Ang **Windows OCR** mogamit sa `Windows.Media.Ocr` API sa operating system niini nga PC.
- Ang **Tesseract** molihok niini nga kompyuter kung na-install ug naa sa PATH.

Gipakita sa app ang nakuha nga teksto aron imong makopya o ma-edit. Dili ipadala sa developer.

## Paggamit sa network

### Pagsusi sa update

Ang mga build nga dili Store mahimong mangayo sa pinakabag-ong GitHub release:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

Ang GitHub (Microsoft) makadawat og normal nga HTTPS request (IP address, user-agent, oras). Ang developer dili makadawat niana nga traffic.

Ang mga install gikan sa Microsoft Store dili mogamit niini nga pagsusi; ang Store maghatud sa mga update.

### Mga link nga imong ablihan

Ang app mahimong mag-abli niini nga mga panid sa system browser. Adunay kaugalingong palisi sa pagkapribado kadtong mga site:

- Homepage sa proyekto: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Pinakabag-ong release: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

Gipakita ang lisensya sulod sa app. Dili kini ablihan isip web page.

## Ubang lokal nga pamatasan

Mahimo nimong sugdan ang app kung mag-sign in sa Windows o Linux. Sa Windows mogamit kini og startup entry (o Microsoft Store startup task para sa Store install). Sa Linux autostart entry. Mao ra kana maglunsad niini nga app sa imong kompyuter.

Ang opsyonal nga global Print Screen hotkey magpabilin sa memory samtang nagdagan ang app aron maablihan ang selector.

## Mga bata

Ang app usa ka himan sa anotasyon sa screenshot. Dili kini gitumong sa mga bata nga wala pay 13 anyos.

## Mga third party

Ang GitHub magproseso sa pagsusi sa update ug mga panid nga imong ablihan, sama sa taas. Ang Microsoft Store magproseso sa Store install ug update. Ang Windows OCR gihatag sa operating system. Ang developer dili makadawat niana nga traffic.

## Mga kausaban

Ang mga update niini nga palisi ipagawas niini nga file sa repositoryo sa proyekto.

## Kontak

Ngalan sa aplikasyon: Screenshot Annotator
Ngalan sa developer: Siarhei Kuchuk

Mga pangutana: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
