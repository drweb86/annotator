# Pack unsigned Store MSIX from published ScreenshotAnnotator.Desktop output (x64 and arm64).
# Partner Center re-signs after certification; do not Authenticode-sign these packages.

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$Version,
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
)

$ErrorActionPreference = "Stop"
trap {
    Write-Error $_
    exit 1
}

# Add-Type against System.Drawing needs Windows PowerShell 5.1. GitHub Actions invokes this script with pwsh.
if ($PSVersionTable.PSEdition -eq 'Core') {
    $windowsPowerShell = Join-Path $env:SystemRoot 'System32\WindowsPowerShell\v1.0\powershell.exe'
    if (-not (Test-Path $windowsPowerShell)) {
        throw "Windows PowerShell 5.1 is required to build Store taskbar icons."
    }
    & $windowsPowerShell -NoProfile -ExecutionPolicy Bypass -File $PSCommandPath -Version $Version -RepoRoot $RepoRoot
    exit $LASTEXITCODE
}

function ConvertTo-MsixVersion([string]$ChangelogVersion) {
    $parts = @($ChangelogVersion.Trim().Split(".") | ForEach-Object { [int]$_ })
    while ($parts.Count -lt 4) {
        $parts += 0
    }
    if ($parts.Count -gt 4) {
        $parts = $parts[0..3]
    }
    foreach ($n in $parts) {
        if ($n -lt 0 -or $n -gt 65535) {
            throw "MSIX version segment $n is outside 0-65535 (from '$ChangelogVersion')."
        }
    }
    return ($parts -join ".")
}

function Find-WindowsKitTool([string]$Name) {
    $searchRoots = @(
        (Join-Path ${env:ProgramFiles(x86)} "Windows Kits\10\bin"),
        (Join-Path $env:ProgramFiles "Windows Kits\10\bin")
    )
    $exe = $null
    foreach ($kitsBin in $searchRoots) {
        if (-not (Test-Path $kitsBin)) {
            continue
        }
        $exe = Get-ChildItem $kitsBin -Recurse -Filter $Name -ErrorAction SilentlyContinue |
            Where-Object { $_.Directory.Name -eq "x64" } |
            Sort-Object { $_.Directory.Parent.Name } -Descending |
            Select-Object -First 1
        if ($exe) {
            break
        }
    }
    if (-not $exe) {
        throw "$Name not found under Windows Kits. Install the Windows 10/11 SDK."
    }
    return $exe.FullName
}

function Add-UnplatedLogoWriter {
    if ("UnplatedLogoWriter" -as [type]) {
        return
    }

    # C# 5: Windows PowerShell 5.1 Add-Type cannot compile newer syntax.
    $source = @'
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

public static class UnplatedLogoWriter
{
    static readonly int[] Sizes = { 16, 20, 24, 30, 32, 36, 40, 44, 48, 60, 64, 72, 80, 96, 256 };

    public static void Write(string sourcePath, string assetsDir)
    {
        using (Bitmap raw = new Bitmap(sourcePath))
        using (Bitmap src = ToArgb(raw))
        {
            Rectangle srcRect = new Rectangle(0, 0, src.Width, src.Height);
            BitmapData srcData = src.LockBits(srcRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                int srcBytesLen = srcData.Stride * src.Height;
                byte[] srcBytes = new byte[srcBytesLen];
                Marshal.Copy(srcData.Scan0, srcBytes, 0, srcBytesLen);
                for (int n = 0; n < Sizes.Length; n++)
                {
                    int size = Sizes[n];
                    using (Bitmap scaled = Scale(srcBytes, srcData.Stride, src.Width, src.Height, size))
                    {
                        string platedPath = Path.Combine(assetsDir, "Square44x44Logo.targetsize-" + size + ".png");
                        scaled.Save(platedPath, ImageFormat.Png);
                        File.Copy(platedPath, Path.Combine(assetsDir, "Square44x44Logo.targetsize-" + size + "_altform-unplated.png"), true);
                    }
                }
            }
            finally
            {
                src.UnlockBits(srcData);
            }
        }
    }

    static Bitmap ToArgb(Bitmap raw)
    {
        if (raw.PixelFormat == PixelFormat.Format32bppArgb)
            return raw.Clone(new Rectangle(0, 0, raw.Width, raw.Height), PixelFormat.Format32bppArgb);

        Bitmap copy = new Bitmap(raw.Width, raw.Height, PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(copy))
        {
            g.DrawImage(raw, 0, 0, raw.Width, raw.Height);
        }
        return copy;
    }

    // Premultiplied box filter. Transparent pixels in the cat art are RGB black, and
    // GDI+ HighQualityBicubic blends that black into the edge.
    static Bitmap Scale(byte[] src, int srcStride, int sw, int sh, int size)
    {
        int pad = (int)Math.Round(size * 3.0 / 44.0);
        if (pad < 0)
            pad = 0;
        if (pad * 2 >= size)
            pad = 0;
        int inner = size - (pad * 2);

        Bitmap dest = new Bitmap(size, size, PixelFormat.Format32bppArgb);
        Rectangle rect = new Rectangle(0, 0, size, size);
        BitmapData data = dest.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        try
        {
            int stride = data.Stride;
            byte[] bytes = new byte[stride * size];
            for (int y = 0; y < inner; y++)
            {
                int sy0 = (int)((long)y * sh / inner);
                int sy1 = (int)((long)(y + 1) * sh / inner);
                if (sy1 <= sy0)
                    sy1 = Math.Min(sh, sy0 + 1);
                for (int x = 0; x < inner; x++)
                {
                    int sx0 = (int)((long)x * sw / inner);
                    int sx1 = (int)((long)(x + 1) * sw / inner);
                    if (sx1 <= sx0)
                        sx1 = Math.Min(sw, sx0 + 1);
                    long sumB = 0, sumG = 0, sumR = 0, sumA = 0;
                    int count = 0;
                    for (int sy = sy0; sy < sy1; sy++)
                    {
                        int row = sy * srcStride;
                        for (int sx = sx0; sx < sx1; sx++)
                        {
                            int i = row + (sx * 4);
                            int a = src[i + 3];
                            sumB += src[i] * a;
                            sumG += src[i + 1] * a;
                            sumR += src[i + 2] * a;
                            sumA += a;
                            count++;
                        }
                    }
                    int di = ((y + pad) * stride) + ((x + pad) * 4);
                    if (sumA == 0 || count == 0)
                        continue;
                    bytes[di] = (byte)(sumB / sumA);
                    bytes[di + 1] = (byte)(sumG / sumA);
                    bytes[di + 2] = (byte)(sumR / sumA);
                    bytes[di + 3] = (byte)(sumA / count);
                }
            }
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
        }
        finally
        {
            dest.UnlockBits(data);
        }
        return dest;
    }
}
'@
    Add-Type -TypeDefinition $source -ReferencedAssemblies System.Drawing
}

function Get-PriConfigXml {
    # No autoResourcePackage: those split logos into resource packs. Index from the package root
    # so resource names stay Assets\Square44x44Logo.png, matching Package.appxmanifest.
    @'
<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<resources targetOsVersion="10.0.0" majorVersion="1">
  <index root="\" startIndexAt="\">
    <default>
      <qualifier name="Language" value="en-US"/>
      <qualifier name="Contrast" value="standard"/>
      <qualifier name="Scale" value="100"/>
      <qualifier name="HomeRegion" value="001"/>
      <qualifier name="TargetSize" value="256"/>
      <qualifier name="LayoutDirection" value="LTR"/>
      <qualifier name="Theme" value="dark"/>
      <qualifier name="AlternateForm" value=""/>
      <qualifier name="DXFeatureLevel" value="DX9"/>
      <qualifier name="Configuration" value=""/>
      <qualifier name="DeviceFamily" value="Universal"/>
      <qualifier name="Custom" value=""/>
    </default>
    <indexer-config type="folder" foldernameAsQualifier="true" filenameAsQualifier="true" qualifierDelimiter="."/>
    <indexer-config type="resw" convertDotsToSlashes="true" initialPath=""/>
    <indexer-config type="resjson" initialPath=""/>
    <indexer-config type="PRI"/>
  </index>
</resources>
'@
}

$msixVersion = ConvertTo-MsixVersion $Version
$makeAppx = Find-WindowsKitTool "makeappx.exe"
$makePri = Find-WindowsKitTool "makepri.exe"
$packageDir = Join-Path $RepoRoot "sources\ScreenshotAnnotator.Desktop.Package"
$template = Join-Path $packageDir "Package.appxmanifest"
$assetsSrc = Join-Path $packageDir "Assets"
$iconSource = Join-Path $RepoRoot "sources\ScreenshotAnnotator\Assets\App.png"
$publishRoot = Join-Path $RepoRoot "Output\publish"
$outDir = Join-Path $RepoRoot "Output"

if (-not (Test-Path $template)) {
    throw "Missing $template"
}
if (-not (Test-Path $assetsSrc)) {
    throw "Missing $assetsSrc"
}
if (-not (Test-Path $iconSource)) {
    throw "Missing $iconSource"
}

Add-UnplatedLogoWriter

New-Item -ItemType Directory -Force -Path $outDir | Out-Null

$arches = @(
    @{ Folder = "x64"; ManifestArch = "x64" }
    @{ Folder = "arm64"; ManifestArch = "arm64" }
)

foreach ($arch in $arches) {
    $publishDir = Join-Path $publishRoot $arch.Folder
    $exe = Join-Path $publishDir "ScreenshotAnnotator.Desktop.exe"
    if (-not (Test-Path $exe)) {
        throw "Missing $exe. Publish win-$($arch.Folder) before packing MSIX."
    }

    $staging = Join-Path $RepoRoot "Output\msix-staging\$($arch.Folder)"
    if (Test-Path $staging) {
        Remove-Item $staging -Recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $staging | Out-Null

    Copy-Item -Path (Join-Path $publishDir "*") -Destination $staging -Recurse -Force
    $assetsDest = Join-Path $staging "Assets"
    if (Test-Path $assetsDest) {
        Remove-Item $assetsDest -Recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $assetsDest | Out-Null
    Copy-Item -Path (Join-Path $assetsSrc "*") -Destination $assetsDest -Recurse -Force
    # Square44x44Logo.png is an opaque dark tile. Windows plates that tile on the taskbar,
    # and BackgroundColor="transparent" makes the plate black. Unplated target-size assets
    # are the cat with a transparent background; resources.pri is what makes the shell use them.
    [UnplatedLogoWriter]::Write($iconSource, $assetsDest)

    $manifestText = [System.IO.File]::ReadAllText($template)
    if ($manifestText.Length -gt 0 -and [int]$manifestText[0] -eq 0xFEFF) {
        $manifestText = $manifestText.Substring(1)
    }
    $manifestText = $manifestText.Replace("__VERSION__", $msixVersion)
    $manifestText = $manifestText.Replace("__ARCH__", $arch.ManifestArch)
    $manifestPath = Join-Path $staging "AppxManifest.xml"
    $utf8NoBom = New-Object System.Text.UTF8Encoding $false
    [System.IO.File]::WriteAllText($manifestPath, $manifestText, $utf8NoBom)

    # Index only the manifest and Assets. The published app has dotted assembly names
    # (System.Collections.dll) that makepri would treat as resource qualifiers.
    $priLayout = Join-Path $staging "pri-layout"
    if (Test-Path $priLayout) {
        Remove-Item $priLayout -Recurse -Force
    }
    New-Item -ItemType Directory -Force -Path (Join-Path $priLayout "Assets") | Out-Null
    Copy-Item $manifestPath (Join-Path $priLayout "AppxManifest.xml")
    Copy-Item -Path (Join-Path $assetsDest "*") -Destination (Join-Path $priLayout "Assets") -Force
    $priConfig = Join-Path $RepoRoot "Output\priconfig.xml"
    [System.IO.File]::WriteAllText($priConfig, (Get-PriConfigXml), $utf8NoBom)
    $priPath = Join-Path $staging "resources.pri"
    Write-Output "Indexing logo variants into resources.pri"
    & $makePri new /pr $priLayout /cf $priConfig /mn (Join-Path $priLayout "AppxManifest.xml") /of $priPath /o
    if ($LASTEXITCODE -ne 0) {
        throw "makepri failed for $($arch.Folder) with exit code $LASTEXITCODE"
    }
    if (-not (Test-Path $priPath)) {
        throw "makepri did not write $priPath"
    }
    Remove-Item $priLayout -Recurse -Force
    Remove-Item $priConfig -Force -ErrorAction SilentlyContinue

    $msixName = "screenshot-annotator_${Version}_windows_$($arch.Folder).msix"
    $msixPath = Join-Path $outDir $msixName
    if (Test-Path $msixPath) {
        Remove-Item $msixPath -Force
    }

    Write-Output "Packing $msixName (Identity Version $msixVersion, $($arch.ManifestArch)) with $makeAppx"
    & $makeAppx pack /d $staging /p $msixPath /o
    if ($LASTEXITCODE -ne 0) {
        throw "makeappx failed for $($arch.Folder) with exit code $LASTEXITCODE"
    }
}

Remove-Item (Join-Path $RepoRoot "Output\msix-staging") -Recurse -Force -ErrorAction SilentlyContinue
Write-Output "MSIX packages written to $outDir"
exit 0
