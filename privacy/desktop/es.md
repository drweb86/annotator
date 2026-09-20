[Languages](README.md)

# Política de privacidad

Última actualización: 20 de septiembre de 2026

**Screenshot Annotator** by Siarhei Kuchuk

Nombre de la aplicación: Screenshot Annotator
Nombre del desarrollador: Siarhei Kuchuk

El software captura capturas de pantalla en este equipo, permite anotarlas y puede leer texto de un área seleccionada con OCR. No crea cuentas en la nube. El desarrollador no opera un servidor que reciba sus capturas, proyectos o datos de uso.

## Datos que el desarrollador no recopila

La aplicación no incluye anuncios ni SDK de analítica, informes de fallos o seguimiento. El desarrollador no recopila, vende ni comparte datos personales.

## Datos almacenados en su equipo

### Configuración

La configuración de la aplicación (el atajo Print Screen, el inicio con el sistema, el motor OCR y los idiomas, el color del resaltador y el último idioma elegido en las ventanas Licencia o Privacidad) se guarda solo en este equipo:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Proyectos e imágenes

Los proyectos anotados y las miniaturas se guardan en su carpeta Imágenes:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Las capturas, las imágenes importadas y el texto de las anotaciones permanecen en esos archivos locales (o en el portapapeles si los copia). La aplicación no los carga.

### Registros

Los registros de diagnóstico pueden escribirse en:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator\Logs`

Si la aplicación se cierra de forma inesperada, puede escribir un informe de error local en el Escritorio. Ese archivo no se envía a ningún sitio.

Esos valores no se cargan al desarrollador.

No se usa ningún servidor del desarrollador para almacenar sus datos.

## Captura de pantalla y OCR

Al usar Print Screen (o el comando de captura), la aplicación captura la pantalla actual para que pueda recortarla y anotarla. La captura ocurre solo en este dispositivo.

El OCR se ejecuta de forma local:

- **Windows OCR** usa las API `Windows.Media.Ocr` del sistema en este PC.
- **Tesseract** se ejecuta en este equipo si lo instaló y está en el PATH.

El texto reconocido se muestra en la aplicación para copiarlo o editarlo. No se envía al desarrollador.

## Uso de la red

### Comprobación de actualizaciones

Las compilaciones que no son de Store pueden solicitar la última versión de GitHub:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) recibe una solicitud HTTPS normal (dirección IP, user-agent, hora). El desarrollador no recibe ese tráfico.

Las instalaciones desde Microsoft Store no usan esta comprobación; Store entrega las actualizaciones.

### Enlaces que abre

La aplicación puede abrir estas páginas en el navegador del sistema. Esos sitios tienen sus propias políticas de privacidad:

- Página del proyecto: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Última versión: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

La licencia se muestra dentro de la aplicación. No se abre como página web.

## Otro comportamiento local

Puede pedir que la aplicación se inicie al iniciar sesión en Windows o Linux. En Windows usa una entrada de inicio (o una tarea de inicio de Microsoft Store en instalaciones de Store). En Linux usa una entrada de autostart. Eso solo inicia esta aplicación en su equipo.

El atajo global opcional de Print Screen permanece en memoria mientras la aplicación se ejecuta para abrir el selector.

## Niños

La aplicación es una herramienta de anotación de capturas. No está dirigida a menores de 13 años.

## Terceros

GitHub procesa la comprobación de actualizaciones y las páginas que abre, como se indica arriba. Microsoft Store procesa las instalaciones y actualizaciones de Store. Windows OCR lo proporciona el sistema operativo. El desarrollador no recibe ese tráfico.

## Cambios

Las actualizaciones de esta política se publicarán en este archivo en el repositorio del proyecto.

## Contacto

Nombre de la aplicación: Screenshot Annotator
Nombre del desarrollador: Siarhei Kuchuk

Preguntas: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
