# D365VsTools

- Before every build, bump the `Identity Version` in `D365VsTools\source.extension.vsixmanifest` (format: `Major.Year.MMDD.HHMM`, e.g. `1.2026.0915.1339`). The user installs the built VSIX directly rather than through a marketplace, so a higher version is required for VSIX Installer to offer a clean in-place update instead of requiring uninstall/reinstall.
