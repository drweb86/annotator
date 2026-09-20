[Languages](README.md)

# Polityka prywatności

Ostatnia aktualizacja: 20 września 2026

**Screenshot Annotator** by Siarhei Kuchuk

Nazwa aplikacji: Screenshot Annotator
Nazwa dewelopera: Siarhei Kuchuk

Oprogramowanie przechwytuje zrzuty ekranu na tym komputerze, pozwala je adnotować i może odczytywać tekst z wybranego obszaru za pomocą OCR. Nie tworzy kont w chmurze. Deweloper nie prowadzi serwera, który odbiera Twoje zrzuty, projekty ani dane o użytkowaniu.

## Dane, których deweloper nie zbiera

Aplikacja nie zawiera reklam ani SDK analityki, raportowania awarii lub śledzenia. Deweloper nie zbiera, nie sprzedaje ani nie udostępnia danych osobowych.

## Dane przechowywane na Twoim komputerze

### Ustawienia

Ustawienia aplikacji (skrót Print Screen, uruchamianie wraz z systemem, silnik OCR i języki, kolor zakreślacza oraz ostatni język wybrany w oknie Licencja lub Prywatność) są przechowywane tylko na tym komputerze:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Projekty i obrazy

Adnotowane projekty i miniatury są przechowywane w folderze Obrazy:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Zrzuty ekranu, importowane obrazy i tekst adnotacji pozostają w tych lokalnych plikach (lub w schowku, jeśli je kopiujesz). Aplikacja ich nie wysyła.

### Dzienniki

Dzienniki diagnostyczne mogą być zapisywane w:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator\Logs`

Jeśli aplikacja ulegnie awarii, może zapisać lokalny raport błędu na Pulpicie. Ten plik nie jest nigdzie wysyłany.

Te wartości nie są przesyłane do dewelopera.

Żaden serwer dewelopera nie jest używany do przechowywania Twoich danych.

## Przechwytywanie ekranu i OCR

Gdy używasz Print Screen (lub polecenia zrzutu), aplikacja przechwytuje bieżący ekran, abyś mógł go przyciąć i adnotować. Przechwycenie odbywa się tylko na tym urządzeniu.

OCR działa lokalnie:

- **Windows OCR** używa interfejsów API `Windows.Media.Ocr` systemu na tym PC.
- **Tesseract** działa na tym komputerze, jeśli jest zainstalowany i dostępny w PATH.

Rozpoznany tekst jest pokazywany w aplikacji, aby można go było skopiować lub edytować. Nie jest wysyłany do dewelopera.

## Korzystanie z sieci

### Sprawdzanie aktualizacji

Kompilacje spoza Store mogą żądać najnowszego wydania GitHub:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) otrzymuje zwykłe żądanie HTTPS (adres IP, user-agent, czas). Deweloper nie otrzymuje tego ruchu.

Instalacje z Microsoft Store nie używają tego sprawdzenia; aktualizacje dostarcza Store.

### Linki, które otwierasz

Aplikacja może otworzyć te strony w przeglądarce systemowej. Te witryny mają własne polityki prywatności:

- Strona projektu: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Najnowsze wydanie: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

Licencja jest wyświetlana w aplikacji. Nie jest otwierana jako strona internetowa.

## Inne zachowanie lokalne

Możesz uruchamiać aplikację przy logowaniu w Windows lub Linux. W Windows używany jest wpis autostartu (lub zadanie startowe Microsoft Store dla instalacji ze Store). W Linux — wpis autostart. To uruchamia tylko tę aplikację na Twoim komputerze.

Opcjonalny globalny skrót Print Screen pozostaje w pamięci, gdy aplikacja działa, aby otworzyć selektor.

## Dzieci

Aplikacja jest narzędziem do adnotacji zrzutów ekranu. Nie jest przeznaczona dla dzieci poniżej 13. roku życia.

## Strony trzecie

GitHub przetwarza sprawdzanie aktualizacji i strony, które otwierasz, jak powyżej. Microsoft Store przetwarza instalacje i aktualizacje Store. Windows OCR jest dostarczany przez system operacyjny. Deweloper nie otrzymuje tego ruchu.

## Zmiany

Aktualizacje tej polityki będą publikowane w tym pliku w repozytorium projektu.

## Kontakt

Nazwa aplikacji: Screenshot Annotator
Nazwa dewelopera: Siarhei Kuchuk

Pytania: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
