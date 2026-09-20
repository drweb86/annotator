[Languages](README.md)

# Adatvédelmi irányelv

Utolsó frissítés: 2026. szeptember 20.

**Screenshot Annotator** by Siarhei Kuchuk

Az alkalmazás neve: Screenshot Annotator
A fejlesztő neve: Siarhei Kuchuk

A szoftver képernyőképeket készít ezen a számítógépen, lehetővé teszi a jegyzetelést, és OCR-rel olvashat szöveget a kijelölt területről. Nem hoz létre felhőfiókokat. A fejlesztő nem üzemeltet olyan háttérrendszert, amely fogadná a képernyőképeket, projekteket vagy használati adatokat.

## Adatok, amelyeket a fejlesztő nem gyűjt

Az alkalmazás nem tartalmaz hirdetést, elemzést, hibajelentést vagy követő SDK-t. A fejlesztő nem gyűjt, nem ad el és nem oszt meg személyes adatokat.

## A számítógépen tárolt adatok

### Beállítások

Az alkalmazásbeállítások (Print Screen gyorsbillentyű, indítás a rendszerrel, OCR-motor és nyelvek, kiemelőszín, valamint a Licenc vagy Adatvédelem ablakban utoljára választott nyelv) csak ezen a számítógépen tárolódnak:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Projektek és képek

A jegyzetelt projektek és előnézetek a Képek mappában tárolódnak:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

A készített képernyőképek, az importált képek és a jegyzetek szövege ezekben a helyi fájlokban marad (vagy a vágólapon, ha másolja). Az alkalmazás nem tölti fel őket.

### Naplók

Diagnosztikai naplók ide írhatók:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

Ha az alkalmazás összeomlik, helyi hibajelentés fájlt írhat az Asztalra. Ez a fájl sehova sem kerül elküldésre.

Ezek az értékek nem töltődnek fel a fejlesztőnek.

A fejlesztő szervere nem használatos az adatok tárolására.

## Képernyőfelvétel és OCR

Print Screen (vagy a képernyőkép parancs) használatakor az alkalmazás rögzíti az aktuális képernyőt, hogy kivághassa és jegyzetelhesse. A rögzítés csak ezen az eszközön történik.

Az OCR helyben fut:

- A **Windows OCR** a rendszer `Windows.Media.Ocr` API-jait használja ezen a PC-n.
- A **Tesseract** ezen a számítógépen fut, ha telepítve van és a PATH-ban van.

A felismert szöveg az alkalmazásban jelenik meg, hogy másolhassa vagy szerkeszthesse. Nem kerül a fejlesztőhöz.

## Hálózathasználat

### Frissítésellenőrzés

A Store-on kívüli buildek lekérhetik a legújabb GitHub-kiadást:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

A GitHub (Microsoft) egy szokásos HTTPS-kérést kap (IP-cím, user-agent, idő). A fejlesztő ezt a forgalmat nem kapja meg.

A Microsoft Store telepítések nem használják ezt az ellenőrzést; a frissítéseket a Store szállítja.

### Megnyitott hivatkozások

Az alkalmazás ezeket az oldalakat megnyithatja a rendszer böngészőjében. Ezeknek a webhelyeknek saját adatvédelmi irányelvük van:

- A projekt honlapja: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Legújabb kiadás: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

A licenc az alkalmazásban jelenik meg. Nem nyílik meg weboldalként.

## Egyéb helyi viselkedés

Az alkalmazást elindíthatja Windows vagy Linux bejelentkezéskor. Windowson indítási bejegyzés (Store-telepítéseknél Microsoft Store indítási feladat) használatos. Linuxon autostart bejegyzés. Ez csak ezt az alkalmazást indítja a számítógépén.

Az opcionális globális Print Screen gyorsbillentyű a memóriában marad, amíg az alkalmazás fut, hogy megnyissa a kijelölőt.

## Gyermekek

Az alkalmazás képernyőkép-jegyzetelő eszköz. Nem 13 év alatti gyermekeknek szól.

## Harmadik felek

A GitHub a fentiek szerint dolgozza fel a frissítésellenőrzést és a megnyitott oldalakat. A Microsoft Store a Store-telepítéseket és frissítéseket dolgozza fel. A Windows OCR-t az operációs rendszer biztosítja. A fejlesztő ezt a forgalmat nem kapja meg.

## Változások

Az irányelv frissítései ebben a fájlban jelennek meg a projekt adattárában.

## Kapcsolat

Az alkalmazás neve: Screenshot Annotator
A fejlesztő neve: Siarhei Kuchuk

Kérdések: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
