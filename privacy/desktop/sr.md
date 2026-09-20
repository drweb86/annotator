[Languages](README.md)

# Politika privatnosti

Poslednje ažuriranje: 20. septembar 2026.

**Screenshot Annotator** by Siarhei Kuchuk

Naziv aplikacije: Screenshot Annotator
Ime programera: Siarhei Kuchuk

Softver hvata snimke ekrana na ovom računaru, omogućava anotacije i može da čita tekst iz izabranog područja pomoću OCR. Ne pravi naloge u oblaku. Programer ne vodi server koji prima vaše snimke, projekte ili podatke o korišćenju.

## Podaci koje programer ne prikuplja

Aplikacija nema reklame, analitiku, izveštaje o padovima ni SDK za praćenje. Programer ne prikuplja, ne prodaje i ne deli lične podatke.

## Podaci sačuvani na vašem računaru

### Podešavanja

Podešavanja aplikacije (prečica Print Screen, pokretanje sa sistemom, OCR mehanizam i jezici, boja markera i poslednji jezik u prozoru Licenca ili Privatnost) čuvaju se samo na ovom računaru:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Projekti i slike

Anotirani projekti i pregledi čuvaju se u folderu Slike:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Snimci, uvezene slike i tekst anotacija ostaju u tim lokalnim datotekama (ili u clipboard-u ako ih kopirate). Aplikacija ih ne otprema.

### Evidencije

Dijagnostičke evidencije mogu se pisati u:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

Ako aplikacija padne, može da napiše lokalni izveštaj o grešci na radnoj površini. Ta datoteka se nigde ne šalje.

Te vrednosti se ne otpremaju programeru.

Nijedan server programera se ne koristi za čuvanje vaših podataka.

## Hvatanje ekrana i OCR

Kada koristite Print Screen (ili komandu za snimak), aplikacija hvata trenutni ekran da biste ga isecli i anotirali. Hvatanje se dešava samo na ovom uređaju.

OCR radi lokalno:

- **Windows OCR** koristi `Windows.Media.Ocr` API operativnog sistema na ovom PC.
- **Tesseract** radi na ovom računaru ako je instaliran i u PATH.

Prepoznati tekst se prikazuje u aplikaciji da biste ga kopirali ili uredili. Ne šalje se programeru.

## Korišćenje mreže

### Provera ažuriranja

Buildovi van Store mogu zatražiti najnovije GitHub izdanje:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) prima običan HTTPS zahtev (IP adresa, user-agent, vreme). Programer ne prima taj saobraćaj.

Instalacije iz Microsoft Store ne koriste ovu proveru; Store isporučuje ažuriranja.

### Linkovi koje otvarate

Aplikacija može otvoriti ove stranice u sistemskom pregledaču. Ti sajtovi imaju sopstvene politike privatnosti:

- Početna stranica projekta: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Najnovije izdanje: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

Licenca se prikazuje u aplikaciji. Ne otvara se kao veb stranica.

## Drugo lokalno ponašanje

Možete pokrenuti aplikaciju pri prijavi na Windows ili Linux. Na Windowsu se koristi stavka pokretanja (ili Microsoft Store zadatak pokretanja za Store instalacije). Na Linuxu autostart stavka. To pokreće samo ovu aplikaciju na vašem računaru.

Opciona globalna prečica Print Screen ostaje u memoriji dok aplikacija radi da bi otvorila birač.

## Deca

Aplikacija je alat za anotaciju snimaka ekrana. Nije namenjena deci mlađoj od 13 godina.

## Treće strane

GitHub obrađuje proveru ažuriranja i stranice koje otvarate, kao gore. Microsoft Store obrađuje Store instalacije i ažuriranja. Windows OCR obezbeđuje operativni sistem. Programer ne prima taj saobraćaj.

## Izmene

Ažuriranja ove politike biće objavljena u ovoj datoteci u skladištu projekta.

## Kontakt

Naziv aplikacije: Screenshot Annotator
Ime programera: Siarhei Kuchuk

Pitanja: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
