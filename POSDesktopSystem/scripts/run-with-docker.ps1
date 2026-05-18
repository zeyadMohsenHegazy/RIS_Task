$ErrorActionPreference = "Stop"

& "$PSScriptRoot/start-docker-db.ps1"

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

$env:DOTNET_ENVIRONMENT = "Docker"
dotnet run --project src/POS.UI/POS.UI.csproj
