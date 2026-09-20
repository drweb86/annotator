[Languages](README.md)

# Politique de confidentialité

Dernière mise à jour : 20 septembre 2026

**Screenshot Annotator** by Siarhei Kuchuk

Nom de l’application : Screenshot Annotator
Nom du développeur : Siarhei Kuchuk

Le logiciel capture des captures d’écran sur cet ordinateur, permet de les annoter et peut lire le texte d’une zone sélectionnée par OCR. Il ne crée pas de comptes cloud. Le développeur n’exploite pas de serveur qui reçoit vos captures d’écran, projets ou données d’utilisation.

## Données que le développeur ne collecte pas

L’application n’inclut ni publicité, ni SDK d’analyse, de rapport de plantage ou de suivi. Le développeur ne collecte, ne vend ni ne partage de données personnelles.

## Données stockées sur votre ordinateur

### Paramètres

Les paramètres de l’application (raccourci Print Screen, démarrage avec le système, moteur OCR et langues, couleur du surligneur, et dernière langue choisie dans les fenêtres Licence ou Confidentialité) sont stockés uniquement sur cet ordinateur :

- Windows : `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux : `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Projets et images

Les projets annotés et les aperçus sont stockés dans votre dossier Images :

- Windows : `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux : `~/Pictures/ScreenshotAnnotator`

Les captures d’écran, les images importées et le texte des annotations restent dans ces fichiers locaux (ou dans le presse-papiers si vous les copiez). L’application ne les envoie pas.

### Journaux

Des journaux de diagnostic peuvent être écrits sous :

- Windows : `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux : `~/.config/SiarheiKuchuk.ScreenshotAnnotator\Logs`

Si l’application plante, elle peut écrire un rapport d’erreur local sur votre Bureau. Ce fichier n’est envoyé nulle part.

Ces valeurs ne sont pas envoyées au développeur.

Aucun serveur du développeur n’est utilisé pour stocker vos données.

## Capture d’écran et OCR

Lorsque vous utilisez Print Screen (ou la commande de capture), l’application capture l’écran actuel pour que vous puissiez le recadrer et l’annoter. La capture a lieu uniquement sur cet appareil.

L’OCR s’exécute localement :

- **Windows OCR** utilise les API `Windows.Media.Ocr` du système sur ce PC.
- **Tesseract** s’exécute sur cet ordinateur si vous l’avez installé et qu’il est dans le PATH.

Le texte reconnu s’affiche dans l’application pour que vous puissiez le copier ou le modifier. Il n’est pas envoyé au développeur.

## Utilisation du réseau

### Vérification des mises à jour

Les versions hors Store peuvent demander la dernière version GitHub :

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) reçoit une requête HTTPS normale (adresse IP, user-agent, heure). Le développeur ne reçoit pas ce trafic.

Les installations depuis le Microsoft Store n’utilisent pas cette vérification ; le Store fournit les mises à jour.

### Liens que vous ouvrez

L’application peut ouvrir ces pages dans le navigateur du système. Ces sites ont leurs propres politiques de confidentialité :

- Page du projet : [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Dernière version : [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

La licence s’affiche dans l’application. Elle n’est pas ouverte comme une page web.

## Autre comportement local

Vous pouvez demander à l’application de démarrer à la connexion sous Windows ou Linux. Sous Windows, cela utilise une entrée de démarrage (ou une tâche de démarrage Microsoft Store pour les installations Store). Sous Linux, une entrée d’autostart. Cela lance uniquement cette application sur votre ordinateur.

Le raccourci Print Screen global facultatif reste en mémoire tant que l’application s’exécute, afin d’ouvrir le sélecteur.

## Enfants

L’application est un outil d’annotation de captures d’écran. Elle ne s’adresse pas aux enfants de moins de 13 ans.

## Tiers

GitHub traite la vérification des mises à jour et les pages que vous ouvrez, comme ci-dessus. Le Microsoft Store traite les installations et mises à jour Store. Windows OCR est fourni par le système d’exploitation. Le développeur ne reçoit pas ce trafic.

## Modifications

Les mises à jour de cette politique seront publiées dans ce fichier dans le dépôt du projet.

## Contact

Nom de l’application : Screenshot Annotator
Nom du développeur : Siarhei Kuchuk

Questions : [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
