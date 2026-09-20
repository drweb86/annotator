[Languages](README.md)

# Politică de confidențialitate

Ultima actualizare: 20 septembrie 2026

**Screenshot Annotator** by Siarhei Kuchuk

Numele aplicației: Screenshot Annotator
Numele dezvoltatorului: Siarhei Kuchuk

Software-ul surprinde capturi de ecran pe acest computer, permite adnotarea și poate citi text dintr-o zonă selectată cu OCR. Nu creează conturi cloud. Dezvoltatorul nu operează un server care primește capturile, proiectele sau datele de utilizare.

## Date pe care dezvoltatorul nu le colectează

Aplicația nu include reclame, analize, rapoarte de blocare sau SDK-uri de urmărire. Dezvoltatorul nu colectează, nu vinde și nu partajează date personale.

## Date stocate pe computer

### Setări

Setările aplicației (comanda rapidă Print Screen, pornirea cu sistemul, motorul OCR și limbile, culoarea evidențiatorului și ultima limbă aleasă în ferestrele Licență sau Confidențialitate) sunt stocate doar pe acest computer:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Proiecte și imagini

Proiectele adnotate și previzualizările sunt stocate în folderul Imagini:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Capturile, imaginile importate și textul adnotărilor rămân în aceste fișiere locale (sau în clipboard dacă le copiați). Aplicația nu le încarcă.

### Jurnale

Jurnalele de diagnostic pot fi scrise în:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

Dacă aplicația se blochează, poate scrie un raport de eroare local pe Desktop. Fișierul nu este trimis nicăieri.

Aceste valori nu sunt încărcate către dezvoltator.

Niciun server al dezvoltatorului nu este folosit pentru a stoca datele dvs.

## Captură de ecran și OCR

Când folosiți Print Screen (sau comanda de captură), aplicația surprinde ecranul curent ca să-l puteți decupa și adnota. Captura are loc doar pe acest dispozitiv.

OCR rulează local:

- **Windows OCR** folosește API-urile `Windows.Media.Ocr` ale sistemului pe acest PC.
- **Tesseract** rulează pe acest computer dacă este instalat și se află în PATH.

Textul recunoscut este afișat în aplicație pentru copiere sau editare. Nu este trimis dezvoltatorului.

## Utilizarea rețelei

### Verificarea actualizărilor

Build-urile din afara Store pot solicita cea mai recentă versiune GitHub:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) primește o cerere HTTPS obișnuită (adresă IP, user-agent, oră). Dezvoltatorul nu primește acel trafic.

Instalările din Microsoft Store nu folosesc această verificare; Store livrează actualizările.

### Linkuri pe care le deschideți

Aplicația poate deschide aceste pagini în browserul sistemului. Acele site-uri au propriile politici de confidențialitate:

- Pagina proiectului: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Ultima versiune: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

Licența este afișată în aplicație. Nu este deschisă ca pagină web.

## Alt comportament local

Puteți porni aplicația la autentificare pe Windows sau Linux. Pe Windows se folosește o intrare de pornire (sau o activitate de pornire Microsoft Store pentru instalările Store). Pe Linux o intrare autostart. Aceasta pornește doar această aplicație pe computerul dvs.

Comanda rapidă globală opțională Print Screen rămâne în memorie cât timp aplicația rulează, pentru a deschide selectorul.

## Copii

Aplicația este un instrument de adnotare a capturilor de ecran. Nu este destinată copiilor sub 13 ani.

## Terți

GitHub procesează verificarea actualizărilor și paginile pe care le deschideți, ca mai sus. Microsoft Store procesează instalările și actualizările Store. Windows OCR este furnizat de sistemul de operare. Dezvoltatorul nu primește acel trafic.

## Modificări

Actualizările acestei politici vor fi publicate în acest fișier din depozitul proiectului.

## Contact

Numele aplicației: Screenshot Annotator
Numele dezvoltatorului: Siarhei Kuchuk

Întrebări: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
