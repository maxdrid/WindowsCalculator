<# 
.SYNOPSIS
    Build script for Windows Calculator C# project
.DESCRIPTION
    Builds the Windows Calculator solution in Release configuration
.NOTES
    Run from PowerShell in the WindowsCalculator directory
#>

param(
    [string]$Configuration = "Release",
    [string]$TargetFramework = "net8.0-windows",
    [switch]$Run
)

Write-Host "Building Windows Calculator..." -ForegroundColor Cyan

# Check if dotnet is available
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error "dotnet CLI not found. Please install .NET 8 SDK."
    exit 1
}

# Restore dependencies
Write-Host "Restoring dependencies..." -ForegroundColor Yellow
dotnet restore WindowsCalculator/WindowsCalculator.csproj
if ($LASTEXITCODE -ne 0) {
    Write-Error "Restore failed"
    exit $LASTEXITCODE
}

# Build
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build WindowsCalculator/WindowsCalculator.csproj --configuration $Configuration --framework $TargetFramework --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit $LASTEXITCODE
}

# Optional: Run
if ($Run) {
    Write-Host "Running application..." -ForegroundColor Green
    dotnet run --project WindowsCalculator/WindowsCalculator.csproj --configuration $Configuration --framework $TargetFramework
}

Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host "Output: WindowsCalculator/bin/$Configuration/$TargetFramework/Windows Calculator.exe" -ForegroundColor Cyan