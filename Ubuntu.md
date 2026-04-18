# Ubuntu

There're several installation methods.

## Method 1. Installation via APT Repository (best)

This method lets you install and update Screenshot Annotator using standard `apt` commands.

### One-time setup

Click copy and paste in the terminal.

```
curl -fsSL https://drweb86.github.io/annotator/gpg-key.pub | sudo gpg --dearmor -o /usr/share/keyrings/screenshot-annotator.gpg
echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/screenshot-annotator.gpg] https://drweb86.github.io/annotator stable main" | sudo tee /etc/apt/sources.list.d/screenshot-annotator.list > /dev/null
```

### Install

```
sudo apt update && sudo apt install screenshot-annotator
```

### Update

```
sudo apt update && sudo apt upgrade screenshot-annotator
```

### Uninstall

```
sudo apt remove screenshot-annotator
sudo rm /etc/apt/sources.list.d/screenshot-annotator.list /usr/share/keyrings/screenshot-annotator.gpg
```

## Method 2. Installation via .deb Download

Download the `.deb` file for your architecture from the [latest release](https://github.com/drweb86/annotator/releases/latest) and install:

```
sudo dpkg -i screenshot-annotator_*_amd64.deb
sudo apt-get install -f
```

For ARM64:

```
sudo dpkg -i screenshot-annotator_*_arm64.deb
sudo apt-get install -f
```

### Uninstall

```
sudo apt remove screenshot-annotator
```

## Method 3. Installation via Bash script

Open terminal, paste

```
wget -O - https://raw.githubusercontent.com/drweb86/annotator/master/scripts/ubuntu-install.sh | bash
```

for preview version

```
wget -O - https://raw.githubusercontent.com/drweb86/annotator/master/scripts/ubuntu-install.sh | bash -s -- --latest
```

### Uninstallation (source install only)

```
wget -O - https://raw.githubusercontent.com/drweb86/annotator/master/scripts/ubuntu-uninstall.sh | bash
```

## Executable

After installation (APT or .deb), the following command is available:

- **`screenshot-annotator`** — Screenshot Annotator graphical application
