$ErrorActionPreference = 'Stop'

# Resolve repo root relative to this script
$repoRoot = Split-Path -Parent $PSScriptRoot
Set-Location $repoRoot

$rid = 'win-x64'

Write-Host "Restoring NuGet packages..." -ForegroundColor Cyan
dotnet restore

Write-Host "Publishing self-contained single-file ($rid, Release)..." -ForegroundColor Cyan
dotnet publish .\TaskAndNotes.UI\TaskAndNotes.UI.csproj `
    -c Release -r $rid --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:DebugType=None `
    -p:DebugSymbols=false

if ($LASTEXITCODE -ne 0) {
    Write-Error "Publish failed"
    exit 1
}

$outDir = Join-Path $repoRoot "TaskAndNotes.UI\bin\Release\net9.0\$rid\publish"
Write-Host "Output: $outDir" -ForegroundColor Green

