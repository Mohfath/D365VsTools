# Bumps the VSIX Identity Version in source.extension.vsixmanifest before every build, using the
# project's Major.Year.MMDD.HHMM convention (e.g. 1.2026.0916.0926). Wired up as a BeforeBuild target
# in D365VsTools.Package.csproj so it runs whether you build from Visual Studio or the command line.
param(
    [string]$ManifestPath = (Join-Path $PSScriptRoot "source.extension.vsixmanifest")
)

if (-not (Test-Path $ManifestPath)) {
    Write-Error "Manifest not found: $ManifestPath"
    exit 1
}

$newVersion = "1." + (Get-Date -Format "yyyy.MMdd.HHmm")

$content = Get-Content -Path $ManifestPath -Raw
$updated = $content -replace '(<Identity\b[^>]*\bVersion=")[^"]*(")', "`${1}$newVersion`${2}"

if ($updated -eq $content) {
    Write-Warning "Could not find a Version attribute on the Identity element in $ManifestPath - version left unchanged."
    exit 0
}

Set-Content -Path $ManifestPath -Value $updated -NoNewline -Encoding UTF8
Write-Host "D365VsTools: bumped VSIX version to $newVersion"
