#!/bin/bash

# Builds Screenshot Annotator .deb packages for amd64 and arm64.
# Run from the scripts/ directory on Ubuntu with .NET 10 SDK installed.

set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
OUTPUT_DIR="$REPO_ROOT/Output"

cd "$REPO_ROOT/sources"

version=$(head -1 "$REPO_ROOT/CHANGELOG.md" | sed 's/^\xEF\xBB\xBF//' | sed 's/^# //')
echo "Building Screenshot Annotator v$version deb packages"
echo ""

rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

declare -A ARCH_MAP
ARCH_MAP["linux-x64"]="amd64"
ARCH_MAP["linux-arm64"]="arm64"

for rid in "${!ARCH_MAP[@]}"; do
    deb_arch="${ARCH_MAP[$rid]}"
    publish_dir="$OUTPUT_DIR/staging/$deb_arch/publish"
    pkg_root="$OUTPUT_DIR/staging/$deb_arch/pkg"
    deb_file="$OUTPUT_DIR/screenshot-annotator_${version}_${deb_arch}.deb"

    echo "========================================="
    echo "  Building $rid ($deb_arch)"
    echo "========================================="
    echo ""

    rm -rf "$OUTPUT_DIR/staging/$deb_arch"

    echo "Publishing..."
    dotnet publish ScreenshotAnnotator.sln \
        "/p:InformationalVersion=$version" \
        "/p:VersionPrefix=$version" \
        "/p:Version=$version" \
        "/p:AssemblyVersion=$version" \
        "--runtime=$rid" \
        -c Release \
        "/p:PublishDir=$publish_dir" \
        /p:PublishReadyToRun=false \
        /p:RunAnalyzersDuringBuild=False \
        --self-contained true \
        --property WarningLevel=0

    echo "Creating package structure..."
    mkdir -p "$pkg_root/DEBIAN"
    mkdir -p "$pkg_root/usr/lib/screenshot-annotator"
    mkdir -p "$pkg_root/usr/bin"
    mkdir -p "$pkg_root/usr/share/applications"
    mkdir -p "$pkg_root/usr/share/pixmaps"

    cp -a "$publish_dir/"* "$pkg_root/usr/lib/screenshot-annotator/"

    ln -sf ../lib/screenshot-annotator/ScreenshotAnnotator.Desktop "$pkg_root/usr/bin/screenshot-annotator"

    cp "$SCRIPT_DIR/App.png" "$pkg_root/usr/share/pixmaps/screenshot-annotator.png"

    cat > "$pkg_root/usr/share/applications/screenshot-annotator.desktop" << 'DESKTOP'
[Desktop Entry]
Version=1.0
Name=Screenshot Annotator
GenericName=Screenshot Annotation Tool
Comment=Helps to annotate screenshots
Categories=Graphics;2DGraphics;RasterGraphics;
Keywords=screenshot;annotate;annotator;
Type=Application
Terminal=false
Exec=screenshot-annotator
Icon=screenshot-annotator
StartupWMClass=ScreenshotAnnotator.Desktop
DESKTOP

    installed_size=$(du -sk "$pkg_root" | cut -f1)

    cat > "$pkg_root/DEBIAN/control" << CONTROL
Package: screenshot-annotator
Version: $version
Section: graphics
Priority: optional
Architecture: $deb_arch
Installed-Size: $installed_size
Depends: libc6, libgcc-s1, libstdc++6, libx11-6, libfontconfig1, dbus-x11, gnome-screenshot
Maintainer: Siarhei Kuchuk <https://github.com/drweb86>
Homepage: https://github.com/drweb86/annotator
Description: Screenshot annotation tool
 Screenshot Annotator helps to annotate screenshots with arrows,
 callouts, blur, highlighter, and more.
CONTROL

    cat > "$pkg_root/DEBIAN/postinst" << 'POSTINST'
#!/bin/bash
set -e
chmod +x /usr/lib/screenshot-annotator/ScreenshotAnnotator.Desktop
if command -v update-desktop-database > /dev/null 2>&1; then
    update-desktop-database -q /usr/share/applications || true
fi
if [ -n "$SUDO_USER" ]; then
    DESKTOP_DIR=$(su - "$SUDO_USER" -c 'xdg-user-dir DESKTOP' 2>/dev/null) || true
    if [ -n "$DESKTOP_DIR" ] && [ -d "$DESKTOP_DIR" ]; then
        cp /usr/share/applications/screenshot-annotator.desktop "$DESKTOP_DIR/Screenshot_Annotator.desktop"
        chown "$SUDO_USER":"$SUDO_USER" "$DESKTOP_DIR/Screenshot_Annotator.desktop"
        chmod 755 "$DESKTOP_DIR/Screenshot_Annotator.desktop"
        su - "$SUDO_USER" -c "gio set '$DESKTOP_DIR/Screenshot_Annotator.desktop' metadata::trusted true" 2>/dev/null || true
    fi
fi
POSTINST
    chmod 755 "$pkg_root/DEBIAN/postinst"

    cat > "$pkg_root/DEBIAN/postrm" << 'POSTRM'
#!/bin/bash
set -e
if command -v update-desktop-database > /dev/null 2>&1; then
    update-desktop-database -q /usr/share/applications || true
fi
if [ -n "$SUDO_USER" ]; then
    DESKTOP_DIR=$(su - "$SUDO_USER" -c 'xdg-user-dir DESKTOP' 2>/dev/null) || true
    if [ -n "$DESKTOP_DIR" ]; then
        rm -f "$DESKTOP_DIR/Screenshot_Annotator.desktop"
    fi
fi
POSTRM
    chmod 755 "$pkg_root/DEBIAN/postrm"

    echo "Setting permissions..."
    find "$pkg_root/usr" -type d -exec chmod 755 {} \;
    find "$pkg_root/usr/lib/screenshot-annotator" -type f -exec chmod 644 {} \;
    chmod 755 "$pkg_root/usr/lib/screenshot-annotator/ScreenshotAnnotator.Desktop"
    find "$pkg_root/usr/lib/screenshot-annotator" \( -name "*.so" -o -name "*.so.*" \) -exec chmod 755 {} \;
    chmod 644 "$pkg_root/usr/share/applications/screenshot-annotator.desktop"
    chmod 644 "$pkg_root/usr/share/pixmaps/screenshot-annotator.png"

    echo "Building .deb..."
    dpkg-deb --build --root-owner-group "$pkg_root" "$deb_file"

    echo "Created: $deb_file"
    echo ""
done

rm -rf "$OUTPUT_DIR/staging"

echo "========================================="
echo "  Build complete"
echo "========================================="
ls -lh "$OUTPUT_DIR"/*.deb
