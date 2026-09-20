[Languages](README.md)

# Informativa sulla privacy

Ultimo aggiornamento: 20 settembre 2026

**Screenshot Annotator** by Siarhei Kuchuk

Nome dell’applicazione: Screenshot Annotator
Nome dello sviluppatore: Siarhei Kuchuk

Il software cattura screenshot su questo computer, consente di annotarli e può leggere il testo di un’area selezionata con OCR. Non crea account cloud. Lo sviluppatore non gestisce un server che riceve i tuoi screenshot, progetti o dati di utilizzo.

## Dati che lo sviluppatore non raccoglie

L’app non include pubblicità né SDK di analisi, segnalazione crash o tracciamento. Lo sviluppatore non raccoglie, vende o condivide dati personali.

## Dati memorizzati sul computer

### Impostazioni

Le impostazioni dell’applicazione (scorciatoia Print Screen, avvio con il sistema, motore OCR e lingue, colore evidenziatore e ultima lingua scelta nelle finestre Licenza o Privacy) sono memorizzate solo su questo computer:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Progetti e immagini

I progetti annotati e le anteprime sono salvati nella cartella Immagini:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Gli screenshot, le immagini importate e il testo delle annotazioni restano in questi file locali (o negli appunti se li copi). L’app non li carica.

### Registri

I registri diagnostici possono essere scritti in:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator\Logs`

Se l’app si chiude in modo imprevisto, può scrivere un report di errore locale sul Desktop. Quel file non viene inviato da nessuna parte.

Questi valori non vengono caricati allo sviluppatore.

Nessun server dello sviluppatore è usato per archiviare i tuoi dati.

## Acquisizione schermo e OCR

Quando usi Print Screen (o il comando di screenshot), l’app cattura lo schermo corrente così puoi ritagliarlo e annotarlo. L’acquisizione avviene solo su questo dispositivo.

L’OCR viene eseguito in locale:

- **Windows OCR** usa le API `Windows.Media.Ocr` del sistema su questo PC.
- **Tesseract** viene eseguito su questo computer se è installato e presente nel PATH.

Il testo riconosciuto è mostrato nell’app per copiarlo o modificarlo. Non viene inviato allo sviluppatore.

## Uso della rete

### Controllo aggiornamenti

Le build non Store possono richiedere l’ultima versione GitHub:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) riceve una normale richiesta HTTPS (indirizzo IP, user-agent, orario). Lo sviluppatore non riceve quel traffico.

Le installazioni da Microsoft Store non usano questo controllo; lo Store fornisce gli aggiornamenti.

### Link che apri

L’app può aprire queste pagine nel browser di sistema. Questi siti hanno proprie informative sulla privacy:

- Homepage del progetto: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Ultima versione: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

La licenza è mostrata nell’app. Non viene aperta come pagina web.

## Altro comportamento locale

Puoi avviare l’app all’accesso su Windows o Linux. Su Windows usa una voce di avvio (o un’attività di avvio Microsoft Store per le installazioni Store). Su Linux usa una voce autostart. Avvia solo questa app sul tuo computer.

La scorciatoia globale opzionale Print Screen resta in memoria mentre l’app è in esecuzione per aprire il selettore.

## Minori

L’app è uno strumento di annotazione screenshot. Non è destinata a bambini sotto i 13 anni.

## Terze parti

GitHub elabora il controllo aggiornamenti e le pagine che apri, come sopra. Microsoft Store elabora installazioni e aggiornamenti Store. Windows OCR è fornito dal sistema operativo. Lo sviluppatore non riceve quel traffico.

## Modifiche

Gli aggiornamenti di questa informativa saranno pubblicati in questo file nel repository del progetto.

## Contatto

Nome dell’applicazione: Screenshot Annotator
Nome dello sviluppatore: Siarhei Kuchuk

Domande: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
