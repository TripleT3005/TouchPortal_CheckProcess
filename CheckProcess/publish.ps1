﻿[string]$ProjectName = (Get-ChildItem -Filter "*.csproj" | Select-Object -First 1).BaseName
[string]$Configuration = "Publish"
[string]$Platform = "Any CPU"
[switch]$NoClean = $false

$CurrentDir = [System.IO.Path]::GetDirectoryName($myInvocation.MyCommand.Path)
$DistFolderPath = "$CurrentDir\packages-dist"
$PluginFilesPath = "$DistFolderPath\$ProjectName"
$BinFilesPath = "$PluginFilesPath\dist"
echo $PSScriptRoot

$ScriptPath = $MyInvocation.MyCommand.Path
$ProjectFolder = [System.IO.Path]::GetDirectoryName($ScriptPath)


if (Test-Path $PluginFilesPath) {
  Write-Output "Cleaning '$ProjectName' packages-dist folder '$PluginFilesPath'..."
  Remove-Item $PluginFilesPath -Force -Recurse
}

Write-Output "`nPublishing '$ProjectName' component to '$BinFilesPath' ...`n"
dotnet publish "$ProjectName" --output "$BinFilesPath" --configuration $Configuration -p:Platform=$Platform

# Run Documentation
Write-Output "`nGenerating entry.tp JSON and Documentation...`n"
#dotnet run -p "$ProjectName-Generator" -o "$PluginFilesPath"
& "$BinFilesPath\$ProjectName-Generator.exe" -o "$PluginFilesPath"

# Copy Readme, CHANGELOG, image(s) to publish folder
copy "README.md" "$PluginFilesPath"
copy "CHANGELOG.md" "$PluginFilesPath"
copy "LICENSE" "$PluginFilesPath"
copy "$ProjectName\airplane_takeoff24.png" "$PluginFilesPath"

# Get version
$FileVersion = (Get-Command $BinFilesPath\$ProjectName.dll).FileVersionInfo.ProductVersion

# Create TPP File
$TppFile = "$DistFolderPath\$ProjectName-$FileVersion.tpp"
if (Test-Path $TppFile) {
  Remove-Item $TppFile -Force
}
Write-Output "`nGenerating archive '$TppFile'..."
& "C:\Program Files\7-Zip\7z.exe" a "$TppFile" "$DistFolderPath\*" -tzip `-xr!*.tpp

if (-not $NoClean) {
  Write-Output "`nCleaning '$ProjectName' component....`n"
  dotnet clean "$ProjectName" --configuration $Configuration -p:Platform=$Platform -r "win-$Platform"
}
