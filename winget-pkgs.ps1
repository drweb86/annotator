$ErrorActionPreference = "Stop"

$version = Get-Content ".\CHANGELOG.md" -First 1
$version = $version.Substring(2)

Write-Output "Version is $version"

Write-Output "Checking if required repositories are checked out."
$wingetForkFolder="../winget-pkgs"
if (-Not (Test-Path $wingetForkFolder))
{
	Write-Error "Checkout https://github.com/drweb86/winget-pkgs into folder $wingetForkFolder" 
	Exit 1
}

Write-Output "Prepare win-get release"
$wingetReleaseFolder="$($wingetForkFolder)\manifests\s\SiarheiKuchuk\ScreenshotAnnotator\$($version)"
$wingetReleaseDateReplacement = $version -replace '\.', '-'
$wingetReleaseHash = Get-FileHash -Path ".\Output\ScreenshotAnnotator_v$($version).exe" -Algorithm SHA256

if (Test-Path $wingetReleaseFolder)
{
	Remove-Item $wingetReleaseFolder -Confirm:$false -Recurse:$true
	if ($LastExitCode -ne 0)
	{
		Write-Error "Fail." 
		Exit 1
	}
}
md "$($wingetReleaseFolder)"

$currentYear = "{0:yyyy}" -f (Get-Date)

& ".\scripts\Template-Copy.ps1"`
    -TemplateFilePath "scripts\winget-pkgs\SiarheiKuchuk.ScreenshotAnnotator.installer.yaml" `
    -DestinationFilePath "$wingetReleaseFolder\SiarheiKuchuk.ScreenshotAnnotator.installer.yaml" `
    -Replacements @{ 'APP_VERSION_STRING' = $version; '2001-01-01' = $wingetReleaseDateReplacement; 'aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa' = $wingetReleaseHash.Hash; }

$localeFiles = Get-ChildItem -Path "scripts\winget-pkgs\" -Filter "SiarheiKuchuk.ScreenshotAnnotator.locale.*.yaml"
foreach ($localeFile in $localeFiles) {
    & ".\scripts\Template-Copy.ps1" `
        -TemplateFilePath $localeFile.FullName `
        -DestinationFilePath "$wingetReleaseFolder\$($localeFile.Name)" `
        -Replacements @{ 'APP_VERSION_STRING' = $version; 'CURRENT_YEAR' = $currentYear }
}

& ".\scripts\Template-Copy.ps1"`
    -TemplateFilePath "scripts\winget-pkgs\SiarheiKuchuk.ScreenshotAnnotator.yaml" `
    -DestinationFilePath "$wingetReleaseFolder\SiarheiKuchuk.ScreenshotAnnotator.yaml" `
    -Replacements @{ 'APP_VERSION_STRING' = $version; }

Write-Output "Release files were put into win-get repo fork. Release it"
