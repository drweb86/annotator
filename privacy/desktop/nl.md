[Languages](README.md)

# Privacybeleid

Laatst bijgewerkt: 20 september 2026

**Screenshot Annotator** by Siarhei Kuchuk

Toepassingsnaam: Screenshot Annotator
Naam van de ontwikkelaar: Siarhei Kuchuk

De software maakt schermafbeeldingen op deze computer, laat u ze annoteren en kan tekst uit een geselecteerd gebied met OCR lezen. Er worden geen cloudaccounts gemaakt. De ontwikkelaar beheert geen server die uw schermafbeeldingen, projecten of gebruiksgegevens ontvangt.

## Gegevens die de ontwikkelaar niet verzamelt

De app bevat geen advertenties en geen SDK’s voor analyse, crashrapportage of tracking. De ontwikkelaar verzamelt, verkoopt of deelt geen persoonsgegevens.

## Gegevens die op uw computer worden opgeslagen

### Instellingen

Toepassingsinstellingen (de Print Screen-sneltoets, starten met het systeem, OCR-engine en talen, markeringskleur en de laatst gekozen taal in het venster Licentie of Privacy) worden alleen op deze computer opgeslagen:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Projecten en afbeeldingen

Geannoteerde projecten en voorbeelden worden opgeslagen in uw map Afbeeldingen:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Schermafbeeldingen, geïmporteerde afbeeldingen en annotatietekst blijven in die lokale bestanden (of op het klembord als u ze kopieert). De app uploadt ze niet.

### Logboeken

Diagnostische logboeken kunnen worden geschreven onder:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

Als de app vastloopt, kan een lokaal foutrapport op het Bureaublad worden geschreven. Dat bestand wordt nergens naartoe gestuurd.

Die waarden worden niet naar de ontwikkelaar geüpload.

Er wordt geen ontwikkelaarsserver gebruikt om uw gegevens op te slaan.

## Schermopname en OCR

Wanneer u Print Screen (of de schermafbeeldingsopdracht) gebruikt, legt de app het huidige scherm vast zodat u het kunt bijsnijden en annoteren. De opname gebeurt alleen op dit apparaat.

OCR draait lokaal:

- **Windows OCR** gebruikt de `Windows.Media.Ocr`-API’s van het besturingssysteem op deze pc.
- **Tesseract** draait op deze computer als het is geïnstalleerd en in PATH staat.

Herkende tekst wordt in de app getoond zodat u die kunt kopiëren of bewerken. Die wordt niet naar de ontwikkelaar gestuurd.

## Netwerkgebruik

### Updatecontrole

Builds buiten de Store kunnen de nieuwste GitHub-release opvragen:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) ontvangt een normale HTTPS-aanvraag (IP-adres, user-agent, tijd). De ontwikkelaar ontvangt dat verkeer niet.

Installaties uit de Microsoft Store gebruiken deze controle niet; de Store levert updates.

### Koppelingen die u opent

De app kan deze pagina’s in de systeembrowser openen. Die sites hebben hun eigen privacybeleid:

- Projecthomepage: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Nieuwste release: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

De licentie wordt in de app getoond. Die wordt niet als webpagina geopend.

## Ander lokaal gedrag

U kunt de app laten starten bij aanmelden op Windows of Linux. Op Windows gebruikt dat een opstartitem (of een Microsoft Store-opstarttaak bij Store-installaties). Op Linux een autostart-item. Dat start alleen deze app op uw computer.

De optionele globale Print Screen-sneltoets blijft in het geheugen zolang de app draait, zodat de kiezer kan worden geopend.

## Kinderen

De app is een hulpmiddel voor het annoteren van schermafbeeldingen. Die is niet gericht op kinderen onder de 13.

## Derden

GitHub verwerkt de updatecontrole en pagina’s die u opent, zoals hierboven. De Microsoft Store verwerkt Store-installaties en updates. Windows OCR wordt door het besturingssysteem geleverd. De ontwikkelaar ontvangt dat verkeer niet.

## Wijzigingen

Updates van dit beleid worden in dit bestand in de projectrepository geplaatst.

## Contact

Toepassingsnaam: Screenshot Annotator
Naam van de ontwikkelaar: Siarhei Kuchuk

Vragen: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
