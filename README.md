# Windows Calculator
<!-- Updated to trigger a new workflow run -->

A modern calculator application for Windows with two implementations:

## 1. Python tkinter Version (Cross-platform)
**File:** `calculator.py`

Runs immediately on Windows, Linux, and macOS with Python 3.8+.

### Features
- Standard calculator operations (+, -, ×, ÷)
- Scientific functions (sin, cos, tan, log, ln, x!, xʸ, √x, ¹/x, x²)
- Constants (π, e)
- Percentage, reciprocal, negation
- Keyboard support
- Dark theme (Windows 11 style)
- History display

### Run
```bash
python3 calculator.py
```

## 2. C# Windows Forms Version (Native Windows)
**Directory:** `WindowsCalculator/`

Native Windows application built with .NET 8 and Windows Forms.

### Features
- Standard and Scientific modes (switch via View menu)
- All standard calculator operations
- Scientific functions (trigonometric, logarithmic, factorial, powers)
- Constants (π, e)
- Keyboard shortcuts
- Copy/Paste support
- Dark theme (Windows 11 style)
- High DPI support
- Menu bar with View, Edit, Help

### Requirements
- .NET 8 SDK or Runtime
- Windows 10/11

### Build & Run
```bash
cd WindowsCalculator
dotnet build
dotnet run
```

Or open `WindowsCalculator.csproj` in Visual Studio 2022+ and press F5.

### Project Structure
```
WindowsCalculator/
├── .github/workflows/build.yml   # GitHub Actions CI/CD
├── WindowsCalculator.sln         # Visual Studio solution
├── build.sh / build.ps1          # Local build scripts
├── WindowsCalculator/
│   ├── WindowsCalculator.csproj
│   ├── Program.cs
│   ├── CalculatorForm.cs
│   └── Properties/
│       ├── Resources.resx
│       └── Resources.Designer.cs
└── publish/                      # Build output (gitignored)
```

## Screenshots

The calculator features a modern dark theme matching Windows 11's design language with:
- Orange accent color for operators (=, +, -, ×, ÷)
- Dark gray buttons for numbers
- Medium gray buttons for functions
- Large, readable display
- History/previous operation display above main display

## Automated Builds (GitHub Actions)

**No local .NET install needed!** Push to GitHub and download the `.exe` from Actions.

### How it works
1. Push this folder to a GitHub repository
2. GitHub Actions automatically builds on `windows-latest`
3. Download the **single-file `.exe`** from:
   - **Artifacts** (every push/PR): Actions tab → latest run → `Windows-Calculator-win-x64`
   - **Releases** (on tag `v*`): Releases page → `Windows Calculator.exe`

### Release tags
```bash
git tag v1.0.0
git push origin v1.0.0
```
Creates a GitHub Release with the `.exe` attached.

## Local Build Options

### Option 1: Visual Studio 2022+ (Recommended)
```
Open WindowsCalculator.sln → Build → Build Solution (Ctrl+Shift+B)
```
Output: `WindowsCalculator/bin/Release/net8.0-windows/Windows Calculator.exe`

### Option 2: Command Line (.NET 8 SDK required)
```bash
cd WindowsCalculator
./build.sh --run          # Linux/macOS (builds for Windows via cross-compile)
# or
.uild.ps1 -Run          # Windows PowerShell
```
Output: `publish/win-x64/Windows Calculator.exe` (~30 MB, standalone)

### Option 3: One-liner publish (Windows)
```powershell
dotnet publish WindowsCalculator/WindowsCalculator.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish/win-x64
```

## License
MIT License - Feel free to use and modify.