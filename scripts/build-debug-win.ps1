$ErrorActionPreference = 'Stop'

# Resolve repo root relative to this script
$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

Write-Host "Restoring NuGet packages..." -ForegroundColor Cyan
dotnet restore

Write-Host "Building solution (Debug)..." -ForegroundColor Cyan
dotnet build .\TaskAndNotes.sln -c Debug --nologo

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit 1
}

Write-Host "Build (Debug) succeeded." -ForegroundColor Green

