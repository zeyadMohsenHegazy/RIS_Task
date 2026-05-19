$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

Write-Host "Stopping all Docker services..." -ForegroundColor Yellow
docker compose down

Write-Host "Done. Close the POS Desktop window manually if it is still open." -ForegroundColor Green
