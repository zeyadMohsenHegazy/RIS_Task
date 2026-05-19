$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

Write-Host "=== RIS Task - Starting all projects ===" -ForegroundColor Cyan
Write-Host ""

Write-Host "[1/3] Starting Docker services (SQL Server, API, Web UI, POS database)..." -ForegroundColor Yellow
docker compose up --build -d
if ($LASTEXITCODE -ne 0) { throw "docker compose failed" }

Write-Host ""
Write-Host "[2/3] Waiting for POS database migrations..." -ForegroundColor Yellow
$maxWait = 180
$elapsed = 0
while ($elapsed -lt $maxWait) {
    $status = docker inspect -f '{{.State.Status}}' pos-db-init 2>$null
    if ($status -eq "exited") { break }
    Start-Sleep -Seconds 2
    $elapsed += 2
}

if ($elapsed -ge $maxWait) {
    Write-Warning "POS database init did not finish in time. The desktop app may fail to connect."
} else {
    $exitCode = docker inspect -f '{{.State.ExitCode}}' pos-db-init 2>$null
    if ($exitCode -eq "0") {
        Write-Host "POS database ready." -ForegroundColor Green
    } else {
        Write-Warning "POS database init exited with code $exitCode. Check: docker logs pos-db-init"
    }
}

Write-Host ""
Write-Host "[3/3] Launching POS Desktop application..." -ForegroundColor Yellow
$posProject = Join-Path $root "POSDesktopSystem\src\POS.UI\POS.UI.csproj"
$env:DOTNET_ENVIRONMENT = "Docker"
Start-Process -FilePath "dotnet" -ArgumentList "run --project `"$posProject`"" -WorkingDirectory $root

Write-Host ""
Write-Host "=== All projects started ===" -ForegroundColor Green
Write-Host "  Web UI  : http://localhost:8081"
Write-Host "  API     : http://localhost:8080"
Write-Host "  POS App : opening on your desktop (WinForms)"
Write-Host ""
Write-Host 'To stop Docker services: .\scripts\stop-all.ps1'
