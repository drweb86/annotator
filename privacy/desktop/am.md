[Languages](README.md)

# የግላዊነት ፖሊሲ

ለመጨረሻ ጊዜ የተዘመነው፦ 20 መስከረም 2026

**Screenshot Annotator** by Siarhei Kuchuk

የመተግበሪያ ስም፦ Screenshot Annotator
የገንቢ ስም፦ Siarhei Kuchuk

ሶፍትዌሩ በዚህ ኮምፒውተር ላይ የስክሪን ምስሎችን ይይዛል፣ ማብራሪያ እንድትሰጡ ያስችላል፣ እና በOCR ከተመረጠ አካባቢ ጽሑፍ ሊያነብ ይችላል። የደመና መለያዎች አይፈጥርም። ገንቢው የስክሪን ምስሎችዎን፣ ፕሮጀክቶችዎን ወይም የአጠቃቀም ውሂብ የሚቀበል አገልጋይ አያሄድም።

## ገንቢው የማይሰበስበው ውሂብ

መተግበሪያው ማስታወቂያ፣ ትንታኔ፣ የብልሽት ሪፖርተር ወይም የክትትል SDK አያካትትም። ገንቢው የግል ውሂብ አይሰበስብም፣ አይሸጥም ወይም አያጋራም።

## በኮምፒውተርዎ ላይ የሚቀመጥ ውሂብ

### ቅንብሮች

የመተግበሪያ ቅንብሮች (የPrint Screen አቋራጭ፣ ከስርዓቱ ጋር መጀመር፣ የOCR ሞተር እና ቋንቋዎች፣ የማድመቂያ ቀለም፣ እና በፈቃድ ወይም ግላዊነት መስኮት የተመረጠው የመጨረሻ ቋንቋ) በዚህ ኮምፒውተር ብቻ ይቀመጣሉ፦

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### ፕሮጀክቶች እና ምስሎች

የተብራሩ ፕሮጀክቶች እና ቅድመ እይታዎች በምስሎች አቃፊ ውስጥ ይቀመጣሉ፦

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

የስክሪን ምስሎች፣ የገቡ ምስሎች እና የማብራሪያ ጽሑፍ በእነዚያ የአካባቢ ፋይሎች ውስጥ ይቀራሉ (ብትቀድሱ በቅንጥብ ሰሌዳም)። መተግበሪያው አያስገባቸውም።

### መዝገቦች

የምርመራ መዝገቦች ከዚህ በታች ሊጻፉ ይችላሉ፦

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

መተግበሪያው ቢበላሽ በዴስክቶፕ ላይ የአካባቢ የስህተት ሪፖርት ፋይል ሊጽፍ ይችላል። ያ ፋይል ወደ ምንም ቦታ አይላክም።

እነዚያ እሴቶች ወደ ገንቢው አይሰቀሉም።

ውሂብዎን ለማከማቸት የገንቢ አገልጋይ አይጠቀምም።

## የስክሪን መያዝ እና OCR

Print Screen (ወይም የስክሪን ምስል ትእዛዝ) ሲጠቀሙ መተግበሪያው የአሁኑን ስክሪን ይይዛል እንዲቆርጡ እና ማብራሪያ እንዲሰጡ። መያዙ በዚህ መሣሪያ ላይ ብቻ ይከሰታል።

OCR በአካባቢ ይሠራል፦

- **Windows OCR** በዚህ PC ላይ የስርዓተ ክወናውን `Windows.Media.Ocr` API ይጠቀማል።
- **Tesseract** ከተጫነ እና በPATH ውስጥ ከሆነ በዚህ ኮምፒውተር ላይ ይሠራል።

የታወቀው ጽሑፍ በመተግበሪያው ውስጥ ይታያል እንዲቀድሱ ወይም እንዲያርሙ። ወደ ገንቢው አይላክም።

## የአውታረ መረብ አጠቃቀም

### የዝማኔ ፍተሻ

ከStore ውጭ የተገነቡት የቅርብ ጊዜውን GitHub ልቀት ሊጠይቁ ይችላሉ፦

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) መደበኛ HTTPS ጥያቄ (IP አድራሻ፣ user-agent፣ ጊዜ) ይቀበላል። ገንቢው ያን ትራፊክ አይቀበልም።

የMicrosoft Store ጭነቶች ይህን ፍተሻ አይጠቀሙም፤ ዝማኔዎችን Store ያደርሳል።

### የሚከፍቷቸው አገናኞች

መተግበሪያው እነዚህን ገጾች በስርዓት አሳሽ ሊከፍት ይችላል። እነዚያ ጣቢያዎች የራሳቸው የግላዊነት ፖሊሲ አላቸው፦

- የፕሮጀክት መነሻ ገጽ፦ [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- የቅርብ ጊዜ ልቀት፦ [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

ፈቃዱ በመተግበሪያው ውስጥ ይታያል። እንደ ድረ-ገጽ አይከፈትም።

## ሌላ የአካባቢ ባህሪ

ወደ Windows ወይም Linux ሲገቡ መተግበሪያውን መጀመር ይችላሉ። በWindows የመነሻ ግቤት (ለStore ጭነቶች የMicrosoft Store የመነሻ ተግባር) ይጠቀማል። በLinux የautostart ግቤት። ያ በኮምፒውተርዎ ላይ ይህን መተግበሪያ ብቻ ይጀምራል።

አማራጭ ዓለም አቀፍ Print Screen አቋራጭ መተግበሪያው እየሠራ ሳለ በማህደረ ትውስታ ይቀራል ምርጫውን ለመክፈት።

## ልጆች

መተግበሪያው የስክሪን ምስል ማብራሪያ መሣሪያ ነው። ከ13 ዓመት በታች ለሆኑ ልጆች አይመራም።

## ሦስተኛ ወገኖች

GitHub የዝማኔ ፍተሻውን እና የሚከፍቷቸውን ገጾች እንደላይ ያስኬዳል። Microsoft Store የStore ጭነቶችን እና ዝማኔዎችን ያስኬዳል። Windows OCR በስርዓተ ክወናው ይሰጣል። ገንቢው ያን ትራፊክ አይቀበልም።

## ለውጦች

የዚህ ፖሊሲ ዝማኔዎች በፕሮጀክት ማከማቻው ውስጥ በዚህ ፋይል ይለጠፋሉ።

## ግንኙነት

የመተግበሪያ ስም፦ Screenshot Annotator
የገንቢ ስም፦ Siarhei Kuchuk

ጥያቄዎች፦ [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
