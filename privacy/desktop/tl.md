[Languages](README.md)

# Patakaran sa privacy

Huling na-update: 20 Setyembre 2026

**Screenshot Annotator** by Siarhei Kuchuk

Pangalan ng aplikasyon: Screenshot Annotator
Pangalan ng developer: Siarhei Kuchuk

Kinukuha ng software ang mga screenshot sa computer na ito, nagbibigay-daan sa anotasyon, at maaaring magbasa ng teksto mula sa napiling lugar gamit ang OCR. Hindi ito gumagawa ng cloud account. Hindi nagpapatakbo ang developer ng server na tumatanggap ng iyong mga screenshot, proyekto, o datos ng paggamit.

## Datos na hindi kinokolekta ng developer

Walang ads, analytics, crash reporter, o tracking SDK ang app. Hindi kinokolekta, ibinebenta, o ibinabahagi ng developer ang personal na datos.

## Datos na naka-imbak sa iyong computer

### Mga setting

Ang mga setting ng aplikasyon (Print Screen hotkey, simula kasama ang system, OCR engine at mga wika, kulay ng highlighter, at huling wikang pinili sa window ng Lisensya o Privacy) ay naka-imbak lang sa computer na ito:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Mga proyekto at larawan

Ang mga naka-anotasyong proyekto at preview ay naka-imbak sa folder ng Mga Larawan:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Ang mga screenshot, inangkat na larawan, at teksto ng anotasyon ay nananatili sa mga lokal na file na iyon (o sa clipboard kung kopyahin). Hindi ina-upload ng app ang mga iyon.

### Mga log

Maaaring isulat ang diagnostic log sa:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

Kung mag-crash ang app, maaari itong magsulat ng lokal na file ng ulat ng error sa Desktop. Hindi ipinapadala ang file na iyon kahit saan.

Hindi ina-upload sa developer ang mga halagang iyon.

Walang server ng developer na ginagamit para mag-imbak ng iyong datos.

## Pagkuha ng screen at OCR

Kapag ginamit mo ang Print Screen (o ang screenshot command), kinukuha ng app ang kasalukuyang screen para ma-crop at ma-anotate. Ang pagkuha ay sa device na ito lang.

Tumatakbo ang OCR nang lokal:

- Gumagamit ang **Windows OCR** ng `Windows.Media.Ocr` API ng operating system sa PC na ito.
- Tumatakbo ang **Tesseract** sa computer na ito kung naka-install at nasa PATH.

Ipinapakita sa app ang nakilalang teksto para makopya o mai-edit. Hindi ipinapadala sa developer.

## Paggamit ng network

### Pagsusuri ng update

Maaaring hingin ng mga build na hindi Store ang pinakabagong GitHub release:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

Tumanggap ang GitHub (Microsoft) ng karaniwang HTTPS request (IP address, user-agent, oras). Hindi natatanggap ng developer ang traffic na iyon.

Hindi ginagamit ng mga install mula sa Microsoft Store ang pagsusuring ito; ang Store ang naghahatid ng update.

### Mga link na binubuksan mo

Maaaring buksan ng app ang mga pahinang ito sa system browser. May sariling privacy policy ang mga site na iyon:

- Homepage ng proyekto: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Pinakabagong release: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

Ipinapakita ang lisensya sa loob ng app. Hindi ito binubuksan bilang web page.

## Iba pang lokal na kilos

Maaari mong simulan ang app sa pag-sign in sa Windows o Linux. Sa Windows, gumagamit ito ng startup entry (o Microsoft Store startup task para sa Store install). Sa Linux, autostart entry. Sinisimulan lang nito ang app na ito sa iyong computer.

Nanatili sa memory ang opsyonal na global Print Screen hotkey habang tumatakbo ang app para mabuksan ang selector.

## Mga bata

Ang app ay tool sa anotasyon ng screenshot. Hindi ito para sa mga batang wala pang 13 taong gulang.

## Mga third party

Pinoproseso ng GitHub ang pagsusuri ng update at mga pahinang binubuksan mo, gaya ng nasa itaas. Pinoproseso ng Microsoft Store ang mga Store install at update. Ibinibigay ng operating system ang Windows OCR. Hindi natatanggap ng developer ang traffic na iyon.

## Mga pagbabago

Ilalathala ang mga update sa patakarang ito sa file na ito sa repositoryo ng proyekto.

## Kontak

Pangalan ng aplikasyon: Screenshot Annotator
Pangalan ng developer: Siarhei Kuchuk

Mga tanong: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
