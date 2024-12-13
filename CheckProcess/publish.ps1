[string]$ProjectFolder = [System.IO.Path]::GetDirectoryName($MyInvocation.MyCommand.Path)
Set-Location $ProjectFolder
Write-Host "Current working directory: $ProjectFolder"

[string]$ProjectName = (Get-ChildItem -Filter "*.csproj" | Select-Object -First 1).BaseName
Write-Host "Detected project name: $ProjectName"

[string]$Configuration = "Publish"
[string]$Platform = "x64"

$PublishFolder = "$ProjectFolder\publish"
$PublishFolderRoot = "$PublishFolder\$ProjectName"
$PublishFolderFiles = "$PublishFolderRoot\files"


Write-Host "Cleaning '$ProjectName' ..."
dotnet clean "$ProjectName.csproj" --configuration $Configuration

Write-Host "Publishing '$ProjectName' to '$PublishFolderFiles' ..."
dotnet publish "$ProjectName.csproj" --output "$PublishFolderFiles" --configuration $Configuration -p:Platform=$Platform

$jsonContent = Get-Content -Path ".\entry.tp" -Raw
$jsonObject = ConvertFrom-Json $jsonContent
$ProjectVersion = $jsonObject.version

copy "entry.tp" "$PublishFolderRoot"

$TppFile = "$PublishFolder\$ProjectName-$ProjectVersion.tpp"
if (Test-Path $TppFile) {
  Remove-Item $TppFile -Force
}
Write-Host "Generating TppFile '$TppFile'..."
&"C:\Program Files\7-Zip\7z.exe" a "$TppFile" "$PublishFolder\*" -tzip `-xr!*.tpp

pause