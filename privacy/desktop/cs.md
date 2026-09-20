[Languages](README.md)

# Zásady ochrany osobních údajů

Poslední aktualizace: 20. září 2026

**Screenshot Annotator** by Siarhei Kuchuk

Název aplikace: Screenshot Annotator
Jméno vývojáře: Siarhei Kuchuk

Software pořizuje snímky obrazovky na tomto počítači, umožňuje je anotovat a umí číst text z vybrané oblasti pomocí OCR. Nevytváří cloudové účty. Vývojář neprovozuje server, který by přijímal vaše snímky, projekty nebo údaje o používání.

## Údaje, které vývojář neshromažďuje

Aplikace neobsahuje reklamy ani sady SDK pro analytiku, hlášení pádů nebo sledování. Vývojář neshromažďuje, neprodává ani nesdílí osobní údaje.

## Údaje uložené v počítači

### Nastavení

Nastavení aplikace (klávesová zkratka Print Screen, spouštění se systémem, engine OCR a jazyky, barva zvýrazňovače a poslední jazyk zvolený v okně Licence nebo Soukromí) se ukládají pouze na tomto počítači:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Projekty a obrázky

Anotované projekty a náhledy se ukládají do složky Obrázky:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Snímky, importované obrázky a text anotací zůstávají v těchto místních souborech (nebo ve schránce, pokud je zkopírujete). Aplikace je nenahrává.

### Protokoly

Diagnostické protokoly se mohou zapisovat do:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

Pokud aplikace spadne, může na plochu zapsat místní hlášení o chybě. Tento soubor se nikam neodesílá.

Tyto hodnoty se vývojáři nenahrávají.

Žádný server vývojáře se k ukládání vašich dat nepoužívá.

## Snímání obrazovky a OCR

Když použijete Print Screen (nebo příkaz snímku), aplikace zachytí aktuální obrazovku, abyste ji mohli oříznout a anotovat. Snímání probíhá pouze na tomto zařízení.

OCR běží lokálně:

- **Windows OCR** používá rozhraní API `Windows.Media.Ocr` operačního systému na tomto PC.
- **Tesseract** běží na tomto počítači, pokud je nainstalován a je v PATH.

Rozpoznaný text se zobrazí v aplikaci, abyste jej mohli zkopírovat nebo upravit. Vývojáři se neodesílá.

## Použití sítě

### Kontrola aktualizací

Sestavení mimo Store mohou požádat o nejnovější vydání na GitHubu:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) obdrží běžný požadavek HTTPS (IP adresa, user-agent, čas). Vývojář tuto provozní zátěž nedostává.

Instalace z Microsoft Store tuto kontrolu nepoužívají; aktualizace dodává Store.

### Odkazy, které otevíráte

Aplikace může otevřít tyto stránky v systémovém prohlížeči. Tyto weby mají vlastní zásady ochrany osobních údajů:

- Domovská stránka projektu: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Nejnovější vydání: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

Licence se zobrazuje v aplikaci. Neotevírá se jako webová stránka.

## Jiné místní chování

Aplikaci můžete spouštět při přihlášení ve Windows nebo Linuxu. Ve Windows se použije položka po spuštění (nebo úloha spuštění Microsoft Store u instalací ze Store). V Linuxu položka autostart. Tím se na vašem počítači spustí jen tato aplikace.

Volitelná globální klávesová zkratka Print Screen zůstává v paměti, dokud aplikace běží, aby mohla otevřít výběr.

## Děti

Aplikace je nástroj pro anotaci snímků obrazovky. Není určena dětem mladším 13 let.

## Třetí strany

GitHub zpracovává kontrolu aktualizací a stránky, které otevíráte, jak je uvedeno výše. Microsoft Store zpracovává instalace a aktualizace Store. Windows OCR poskytuje operační systém. Vývojář tuto provozní zátěž nedostává.

## Změny

Aktualizace těchto zásad budou zveřejněny v tomto souboru v úložišti projektu.

## Kontakt

Název aplikace: Screenshot Annotator
Jméno vývojáře: Siarhei Kuchuk

Otázky: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
