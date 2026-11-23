<#
.SYNOPSIS
    Publishes NxtHUD and builds its installer and portable zip.

.DESCRIPTION
    1. Runs the unit tests.
    2. Publishes a self-contained x64 build to artifacts\publish (no .NET install needed on the target PC).
    3. Compiles installer\NxtHUD.iss with Inno Setup into artifacts\installer\NxtHUD-Setup-<version>.exe.
    4. Zips the published app into artifacts\installer\NxtHUD-<version>-portable-x64.zip.
    5. Writes SHA-256 checksums of both to artifacts\installer\SHA256SUMS.txt.

    Requires the .NET 8 SDK (or newer) and Inno Setup 6.3 or later:
        winget install JRSoftware.InnoSetup

    The GitHub Release workflow (.github\workflows\release.yml) runs this same script.

.PARAMETER Version
    Version to stamp on the app and installer, for example 1.4.0. Defaults to <Version> in
    Directory.Build.props.

.PARAMETER FrameworkDependent
    Publish a small build that needs the .NET 8 Desktop Runtime on the target PC,
    instead of bundling the runtime.

.PARAMETER SkipTests
    Do not run the unit tests first.

.EXAMPLE
    .\installer\build-installer.ps1

.EXAMPLE
    .\installer\build-installer.ps1 -Version 1.4.0 -SkipTests
#>
[CmdletBinding()]
param(
    [string]$Version,
    [switch]$FrameworkDependent,
    [switch]$SkipTests
)

$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $root 'NxtHUD.sln'
$project = Join-Path $root 'src\NxtHUD\NxtHUD.csproj'
$publishDir = Join-Path $root 'artifacts\publish'
$outputDir = Join-Path $root 'artifacts\installer'
$script = Join-Path $PSScriptRoot 'NxtHUD.iss'

if (-not $Version) {
    # The version normally lives in one place: Directory.Build.props.
    [xml]$props = Get-Content (Join-Path $root 'Directory.Build.props')
    $Version = ($props.Project.PropertyGroup | ForEach-Object { $_.Version } | Where-Object { $_ } | Select-Object -First 1)
    if (-not $Version) { throw 'Could not read <Version> from Directory.Build.props.' }
}

if ($Version -notmatch '^\d+\.\d+\.\d+$') {
    throw "Version '$Version' must look like 1.2.3."
}

Write-Host "Building NxtHUD $Version" -ForegroundColor Cyan

if (-not $SkipTests) {
    dotnet test $solution -c Release
    if ($LASTEXITCODE -ne 0) { throw 'Unit tests failed.' }
}

foreach ($dir in @($publishDir, $outputDir)) {
    if (Test-Path $dir) { Remove-Item $dir -Recurse -Force }
}
New-Item -ItemType Directory -Path $outputDir | Out-Null

$selfContained = if ($FrameworkDependent) { 'false' } else { 'true' }

# ReadyToRun precompiles the app, which makes it start noticeably faster at sign-in.
dotnet publish $project `
    -c Release `
    -r win-x64 `
    --self-contained $selfContained `
    -p:Version=$Version `
    -p:PublishReadyToRun=true `
    -p:DebugType=none `
    -o $publishDir
if ($LASTEXITCODE -ne 0) { throw 'dotnet publish failed.' }

foreach ($required in @('NxtHUD.exe', 'LICENSE.txt', 'THIRD-PARTY-NOTICES.txt')) {
    if (-not (Test-Path (Join-Path $publishDir $required))) {
        throw "dotnet publish did not produce $required."
    }
}

$candidates = @(
    (Get-Command 'ISCC.exe' -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source),
    (Join-Path ${env:ProgramFiles(x86)} 'Inno Setup 6\ISCC.exe'),
    (Join-Path $env:ProgramFiles 'Inno Setup 6\ISCC.exe'),
    (Join-Path $env:LOCALAPPDATA 'Programs\Inno Setup 6\ISCC.exe')
)
$iscc = $candidates | Where-Object { $_ -and (Test-Path $_) } | Select-Object -First 1
if (-not $iscc) {
    throw "Inno Setup 6 was not found. Install it with 'winget install JRSoftware.InnoSetup' and run this script again."
}

& $iscc "/DAppVersion=$Version" $script
if ($LASTEXITCODE -ne 0) { throw 'Inno Setup failed.' }

$installer = Join-Path $outputDir "NxtHUD-Setup-$Version.exe"
$portable = Join-Path $outputDir "NxtHUD-$Version-portable-x64.zip"

Compress-Archive -Path (Join-Path $publishDir '*') -DestinationPath $portable -CompressionLevel Optimal

$sums = foreach ($file in @($installer, $portable)) {
    $hash = (Get-FileHash $file -Algorithm SHA256).Hash.ToLowerInvariant()
    "$hash  $(Split-Path $file -Leaf)"
}
Set-Content -Path (Join-Path $outputDir 'SHA256SUMS.txt') -Value $sums -Encoding ascii

Write-Host ''
Write-Host "Installer: $installer" -ForegroundColor Green
Write-Host "Portable:  $portable" -ForegroundColor Green
