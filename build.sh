#!/bin/bash
# Build script for Windows Calculator C# project
# Run from the WindowsCalculator directory

set -e

CONFIGURATION="Release"
TARGET_FRAMEWORK="net8.0-windows"
RUN_APP=false

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -c|--configuration)
            CONFIGURATION="$2"
            shift 2
            ;;
        -f|--framework)
            TARGET_FRAMEWORK="$2"
            shift 2
            ;;
        -r|--run)
            RUN_APP=true
            shift
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

echo -e "\033[36mBuilding Windows Calculator...\033[0m"

# Check if dotnet is available
if ! command -v dotnet &> /dev/null; then
    echo -e "\033[31mdotnet CLI not found. Please install .NET 8 SDK.\033[0m"
    exit 1
fi

# Restore dependencies
echo -e "\033[33mRestoring dependencies...\033[0m"
dotnet restore WindowsCalculator/WindowsCalculator.csproj

# Build
echo -e "\033[33mBuilding project...\033[0m"
dotnet build WindowsCalculator/WindowsCalculator.csproj --configuration "$CONFIGURATION" --framework "$TARGET_FRAMEWORK" --no-restore

# Optional: Run
if [ "$RUN_APP" = true ]; then
    echo -e "\033[32mRunning application...\033[0m"
    dotnet run --project WindowsCalculator/WindowsCalculator.csproj --configuration "$CONFIGURATION" --framework "$TARGET_FRAMEWORK"
fi

echo -e "\033[32mBuild completed successfully!\033[0m"
echo -e "\033[36mOutput: WindowsCalculator/bin/$CONFIGURATION/$TARGET_FRAMEWORK/Windows Calculator.exe\033[0m"