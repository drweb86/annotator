#!/bin/bash

echo
echo Removing sources installation folder
echo
sudo rm -rf /usr/local/src/screenshot-annotator

echo
echo Removing binaries installation folder
echo
sudo rm -rf /usr/local/screenshot-annotator

echo
echo Removing configuration files
echo

echo
echo Removing shortcut
echo
sudo rm /usr/share/applications/Screenshot_Annotator.desktop

echo
echo Application was uninstalled
echo
echo
echo
sleep 2m