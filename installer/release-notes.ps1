<#
.SYNOPSIS
    Writes the notes for a GitHub Release: what changed in that version, then how to install.

.DESCRIPTION
    Takes the "## <version>" section of CHANGELOG.md (everything up to the next "## " heading),
    puts it under a "What's new in <version>" heading, and follows it with the install steps
    from installer\release-notes.md. If CHANGELOG.md has no section for the version, the notes
    are the install steps alone and a warning says so.

    The Release workflow (.github\workflows\release.yml) runs this when it publishes a release.
    Run it by hand to preview the notes:
        .\installer\release-notes.ps1 -Version 1.4.0 -OutFile notes.md

.PARAMETER Version
    The version being released, for example 1.4.0.

.PARAMETER OutFile
    Where to write the notes (Markdown).
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Version,
    [Parameter(Mandatory)][string]$OutFile
)

$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
# Read and write UTF-8 explicitly: Windows PowerShell 5.1 would otherwise read the files as ANSI
# and turn characters such as "→" into mojibake.
$install = (Get-Content (Join-Path $PSScriptRoot 'release-notes.md') -Raw -Encoding UTF8).Replace('<version>', $Version)
$changelog = Get-Content (Join-Path $root 'CHANGELOG.md') -Raw -Encoding UTF8

# "## 1.2.0" (optionally followed by more text on the line) up to the next "## " heading or the end.
# The lookahead after the version stops "## 1.2.0" from also matching "## 1.2.01".
$pattern = '(?ms)^##[ \t]+' + [regex]::Escape($Version) + '(?![\w.])[^\r\n]*\r?\n(.*?)(?=^##[ \t]|\z)'
$match = [regex]::Match($changelog, $pattern)

if ($match.Success -and $match.Groups[1].Value.Trim()) {
    $whatsNew = $match.Groups[1].Value.Trim()
    $notes = "## What's new in $Version`n`n$whatsNew`n`n$install"
} else {
    Write-Warning "CHANGELOG.md has no section for $Version; the release notes have the install steps only."
    $notes = $install
}

$path = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($OutFile)
[System.IO.File]::WriteAllText($path, $notes, (New-Object System.Text.UTF8Encoding $false))
Write-Host "Release notes for $Version written to $OutFile"
