# D365VsTools

- The VSIX version (`Identity Version` in `D365VsTools\source.extension.vsixmanifest`, format `Major.Year.MMDD.HHMM`) is bumped automatically by a `BeforeBuild` target in `D365VsTools.Package.csproj`, which runs `D365VsTools\UpdateVersion.ps1` on every build (Visual Studio or command line) — no manual edit needed before building, by Claude or the user. The user installs the built VSIX directly rather than through a marketplace, so this exists so VSIX Installer always offers a clean in-place update instead of requiring uninstall/reinstall.
