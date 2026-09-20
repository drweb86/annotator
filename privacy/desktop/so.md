[Languages](README.md)

# Siyaasadda sirta

Markii ugu dambeysay la cusbooneysiiyay: 20 Sebtembar 2026

**Screenshot Annotator** by Siarhei Kuchuk

Magaca abka: Screenshot Annotator
Magaca horumariyaha: Siarhei Kuchuk

Software-ku wuxuu qaadaa sawirada shaashadda kumbuyuutarkan, wuxuu kuu oggolaanayaa inaad faallo ka bixiso, wuxuuna ka akhrin karaa qoraalka aagga aad dooratay OCR. Ma abuuro akoonno daruur. Horumariyuhu ma maamulo server qaata sawiradaada, mashaariicdaada, ama xogta isticmaalka.

## Xogta horumariyuhu aanu ururin

App-ku ma lahan xayaysiis, falanqayn, wariye burbur, ama SDK raadinta. Horumariyuhu ma ururiyo, iibin, ama wadaagin xogta shakhsiyeed.

## Xogta ku kaydsan kumbuyuutarkaaga

### Goobaha

Goobaha abka (furaha Print Screen, bilowga nidaamka, mashiinka OCR iyo luqadaha, midabka calaamadeeyaha, iyo luqadda ugu dambeysay ee daaqadda Liisanka ama Sirta) waxaa lagu kaydiyaa keliya kumbuyuutarkan:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Mashaariic iyo sawiro

Mashaariicda la faallooday iyo horudhacyada waxaa lagu kaydiyaa galka Sawirada:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Sawirada shaashadda, sawirada la soo dejiyay, iyo qoraalka faallada waxay ku sii jiraan faylasha maxalliga ah (ama clipboard haddii aad koobi gareyso). App-ku ma soo geliyo.

### Diiwaannada

Diiwaannada ogaanshaha waxaa laga yaabaa in lagu qoro:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

Haddii app-ku burburo, wuxuu ku qori karaa fayl warbixin khalad maxalli ah Desktop. Faylkaas meelna looma diro.

Qiimahaas looma soo gelin horumariyaha.

Server horumariye looma isticmaalo in lagu kaydiyo xogtaada.

## Qaadista shaashadda iyo OCR

Markaad isticmaasho Print Screen (ama amarka screenshot), app-ku wuxuu qaadaa shaashadda hadda si aad u jarato oo u faallaysato. Qaadistu waxay ku dhacdaa aaladdan oo keliya.

OCR wuxuu ku shaqeeyaa maxalli:

- **Windows OCR** wuxuu isticmaalaa APIs `Windows.Media.Ocr` ee nidaamka hawlgalka PC-gan.
- **Tesseract** wuxuu ku shaqeeyaa kumbuyuutarkan haddii la rakibay oo ku jiro PATH.

Qoraalka la aqoonsaday ayaa lagu muujiyaa app-ka si aad u koobi gareyso ama u tahrirto. Looma diro horumariyaha.

## Isticmaalka shabakadda

### Hubinta cusbooneysiinta

Dhismooyinka aan Store ahayn waxay weydiisan karaan sii deynta GitHub ee ugu dambeysay:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) wuxuu helayaa codsi HTTPS caadi ah (cinwaanka IP, user-agent, waqtiga). Horumariyuhu ma helayo taraafikadaas.

Rakibaadaha Microsoft Store ma isticmaalaan hubintaan; Store ayaa keenaya cusbooneysiinta.

### Xiriirada aad furto

App-ku wuxuu ku furi karaa bogaggan browser-ka nidaamka. Goobahaas waxay leeyihiin siyaasado siri oo u gaar ah:

- Bogga guriga ee mashruuca: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Sii deynta ugu dambeysay: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

Liisanka waxaa lagu muujiyaa gudaha app-ka. Looma furo sidii bog web.

## Dhaqanka kale ee maxalliga ah

Waxaad ku bilaabi kartaa app-ka markaad gasho Windows ama Linux. Windows wuxuu isticmaalaa gelitaanka bilowga (ama hawl bilow Microsoft Store ee rakibaadaha Store). Linux gelitaanka autostart. Taasi waxay ku bilaabaysaa kaliya app-kan kumbuyuutarkaaga.

Furaha caalamiga ah ee Print Screen ee ikhtiyaariga ah wuxuu ku sii jiraa xusuusta inta app-ku shaqeynayo si uu u furo xulashada.

## Carruurta

App-ku waa qalab faallo sawir shaashad. Looma jeedin carruurta ka yar 13 sano.

## Dhinacyada saddexaad

GitHub wuxuu farsameeyaa hubinta cusbooneysiinta iyo bogagga aad furto, sida kor. Microsoft Store wuxuu farsameeyaa rakibaadaha iyo cusbooneysiinta Store. Windows OCR waxaa bixiya nidaamka hawlgalka. Horumariyuhu ma helayo taraafikadaas.

## Isbeddellada

Cusbooneysiinta siyaasaddan waxaa lagu dhaji doonaa faylkan kaydka mashruuca.

## Xiriir

Magaca abka: Screenshot Annotator
Magaca horumariyaha: Siarhei Kuchuk

Su'aalaha: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
