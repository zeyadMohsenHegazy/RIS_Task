$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

if (-not (Test-Path ".env")) {
    Copy-Item ".env.example" ".env"
    Write-Host "Created .env from .env.example. Update MSSQL_SA_PASSWORD if needed."
}

Write-Host "Starting SQL Server container..."
docker compose up -d sqlserver

Write-Host "Waiting for SQL Server to become healthy..."
$attempts = 0
while ($attempts -lt 60) {
    $status = docker inspect --format='{{.State.Health.Status}}' pos-sqlserver 2>$null
    if ($status -eq "healthy") {
        Write-Host "SQL Server is ready."
        break
    }

    Start-Sleep -Seconds 2
    $attempts++
}

if ($attempts -ge 60) {
    Write-Warning "SQL Server health check did not pass in time. The app may still connect once ready."
}

Write-Host ""
Write-Host "Run the desktop app against Docker SQL Server:"
Write-Host '  $env:DOTNET_ENVIRONMENT = "Docker"'
Write-Host "  dotnet run --project src/POS.UI/POS.UI.csproj"
Write-Host ""
Write-Host "Optional: run EF migrations in Docker:"
Write-Host "  docker compose up db-init"
