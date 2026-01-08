#!/bin/bash

# Fail on first error.
set -e

# Default values
LATEST_SOURCES=false
for arg in "$@"; do
    if [[ "$arg" == "--latest" ]]; then
        echo "Using latest sources! Not sources related to version."
        LATEST_SOURCES=true
    fi
done

sourceCodeInstallationDirectory=/usr/local/src/screenshot-annotator
binariesInstallationDirectory=/usr/local/screenshot-annotator

if [ "$EUID" -eq 0 ]
  then echo "Please do not run this script with sudo, root permissions"
  exit
fi

echo
echo Install .Net 10
echo
wget https://dot.net/v1/dotnet-install.sh -O /tmp/dotnet-install.sh
chmod +x /tmp/dotnet-install.sh
/tmp/dotnet-install.sh --channel 10.0
sudo apt install dbus-x11

echo
echo Cleaning installation directories
echo
sudo rm -rf ${sourceCodeInstallationDirectory}
sudo rm -rf ${binariesInstallationDirectory}

echo
echo Get source code
echo
sudo git clone https://github.com/drweb86/annotator.git ${sourceCodeInstallationDirectory}
cd ${sourceCodeInstallationDirectory}
echo "A"
sudo git fetch --tags
echo "B"
sudo git describe --tags --abbrev=0
echo "C"
version=$(sudo git describe --tags --abbrev=0 2>/dev/null)
echo "Latest tag: $version"

if [ "$LATEST_SOURCES" = true ]; then
	echo
	echo Update to latest sources
	echo
	sudo git checkout main
	sudo git pull origin main 2>/dev/null || echo "Note: Could not pull from remote, continuing with local copy"
else
	echo
	echo Update to tag
	echo
	sudo git checkout tags/${version}
fi

echo
echo Building
echo
cd ./sources
sudo /root/.dotnet/dotnet publish ScreenshotAnnotator.sln /p:Version=${version} /p:AssemblyVersion=${version} -c Release --property:PublishDir=${binariesInstallationDirectory} --use-current-runtime --self-contained

echo
echo Prepare PNG icon for Gnome, ico files are not handled
echo
sudo cp "${sourceCodeInstallationDirectory}/scripts/App.png" "${binariesInstallationDirectory}/Icon.png"

echo
echo Prepare shortcut
echo

echo
echo Prepare shortcut for Console
echo

echo
echo Prepare shortcut for UI
echo

temporaryShortcutUI=/tmp/Screenshot_Annotator.desktop
sudo rm -f ${temporaryShortcutUI}
cat > ${temporaryShortcutUI} << EOL
[Desktop Entry]
Version=${version}
Name=Screenshot Annotator
GenericName=Helps to annotate screenshots
Categories=Screenshot;
Comment=Helps to annotate screenshots
Keywords=screenshot;annotate;annotator
Type=Application
Terminal=false
StartupWMClass=ScreenshotAnnotator.Desktop
Path=${binariesInstallationDirectory}
Exec=${binariesInstallationDirectory}/ScreenshotAnnotator.Desktop
Icon=${binariesInstallationDirectory}/Icon.png
EOL
shortcutFileUI=/usr/share/applications/Screenshot_Annotator.desktop
sudo cp ${temporaryShortcutUI} "${shortcutFileUI}"
sudo chmod a+x "${shortcutFileUI}"
sudo dbus-launch gio set "${shortcutFileUI}" metadata::trusted true

echo
echo
echo Everything is completed 
echo
echo
echo Application was installed too:
echo
echo Binaries: ${binariesInstallationDirectory}
echo Sources: ${sourceCodeInstallationDirectory}
echo
echo Shortcut for quick search is provisioned:
echo     search menu for Screnshot Annotator.
echo
echo UI tool: ${binariesInstallationDirectory}/ScreenshotAnnotator.Desktop
echo
echo
sleep 2m