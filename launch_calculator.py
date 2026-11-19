#!/usr/bin/env python3
"""
Launcher for the Python calculator - checks dependencies and runs the app.
"""

import sys
import subprocess

def check_tkinter():
    """Check if tkinter is available."""
    try:
        import tkinter
        return True
    except ImportError:
        return False

def install_tkinter():
    """Attempt to install tkinter based on platform."""
    import platform
    system = platform.system().lower()
    
    if system == "linux":
        # Try common package managers
        for cmd in [
            ["apt-get", "install", "-y", "python3-tk"],
            ["dnf", "install", "-y", "python3-tkinter"],
            ["pacman", "-S", "--noconfirm", "tk"],
            ["zypper", "install", "-y", "python3-tk"],
        ]:
            try:
                subprocess.run(cmd, check=True, capture_output=True)
                return True
            except (subprocess.CalledProcessError, FileNotFoundError):
                continue
    elif system == "darwin":
        # macOS - tkinter usually comes with Python from python.org
        # For Homebrew Python, may need: brew install python-tk
        try:
            subprocess.run(["brew", "install", "python-tk"], check=True, capture_output=True)
            return True
        except (subprocess.CalledProcessError, FileNotFoundError):
            pass
    elif system == "windows":
        # Windows - tkinter comes with standard Python installer
        print("On Windows, tkinter is included with the standard Python installer.")
        print("Please reinstall Python from python.org and ensure 'tcl/tk' is checked.")
        return False
    
    return False

def main():
    print("Windows Calculator Launcher")
    print("=" * 30)
    
    if check_tkinter():
        print("✓ tkinter is available")
        print("Starting calculator...")
        # Import and run the calculator
        from calculator import main
        main()
    else:
        print("✗ tkinter not found")
        print("\nAttempting to install tkinter...")
        if install_tkinter():
            print("✓ tkinter installed successfully")
            print("Please restart this script.")
        else:
            print("✗ Could not install tkinter automatically")
            print("\nManual installation:")
            print("  Ubuntu/Debian: sudo apt-get install python3-tk")
            print("  Fedora: sudo dnf install python3-tkinter")
            print("  Arch: sudo pacman -S tk")
            print("  macOS (Homebrew): brew install python-tk")
            print("  Windows: Reinstall Python from python.org with 'tcl/tk' checked")
        sys.exit(1)

if __name__ == "__main__":
    main()