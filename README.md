<p align="center">
  <img src="docs/promo/banner.png" alt="NxtHUD: Fluent flyouts for your media and keys, right on Windows 11." width="100%">
</p>

<p align="center">
  <a href="https://github.com/exa1t/NxtHUD/releases/latest"><img src="https://img.shields.io/github/v/release/exa1t/NxtHUD?style=flat-square&color=6D5BF6&label=release" alt="Latest release"></a>
  <a href="https://github.com/exa1t/NxtHUD/releases"><img src="https://img.shields.io/github/downloads/exa1t/NxtHUD/total?style=flat-square&color=4F7FE8&label=downloads" alt="Downloads"></a>
  <img src="https://img.shields.io/badge/Windows-10%20%7C%2011-2E9BD6?style=flat-square&logo=windows11&logoColor=white" alt="Windows 10 and 11">
  <img src="https://img.shields.io/badge/.NET-8-1AA9C4?style=flat-square&logo=dotnet&logoColor=white" alt=".NET 8">
  <a href="LICENSE"><img src="https://img.shields.io/github/license/exa1t/NxtHUD?style=flat-square&color=12B5C9" alt="MIT licence"></a>
</p>

<p align="center">
  <b>English</b> · <a href="README.vi.md">Tiếng Việt</a> · <a href="README.zh-CN.md">简体中文</a>
</p>

<p align="center">
  A lightweight tray app that brings <b>Fluent-style flyouts</b> to your media keys, volume and lock keys,<br>
  plus a <b>now-playing widget</b> with a live visualizer right on the taskbar.
</p>

<p align="center">
  <a href="https://github.com/exa1t/NxtHUD/releases/latest"><img src="https://img.shields.io/badge/Download%20for%20Windows-6D5BF6?style=for-the-badge&logo=windows11&logoColor=white" alt="Download for Windows"></a>
</p>

<p align="center">
  <img src="docs/promo/hero.png" alt="NxtHUD's media flyout, Up Next, volume bar, taskbar widget, taskbar stats and lock key flyouts floating over a violet and teal background" width="100%">
</p>

## ✨ Highlights

<p align="center">
  <img src="docs/promo/media.png" alt="Media flyout: cover art, title, artist, player, progress bar and playback controls" width="49%">
  <img src="docs/promo/widget.png" alt="Taskbar widget: cover, track and controls with a live audio visualizer" width="49%">
</p>
<p align="center">
  <img src="docs/promo/stats.png" alt="Taskbar stats: CPU, RAM, GPU, GPU temperature, network and disk next to the tray" width="49%">
  <img src="docs/promo/upnext.png" alt="Up Next flyout announcing the next track" width="49%">
</p>
<p align="center">
  <img src="docs/promo/volume.png" alt="Volume flyout replacing the Windows overlay" width="49%">
  <img src="docs/promo/lockkeys.png" alt="Lock keys flyout showing Caps Lock is on" width="49%">
</p>
<p align="center">
  <img src="docs/promo/settings.png" alt="Appearance settings: theme, backdrop material, opacity, flyout position, display, compact layout and animation speed" width="98%">
</p>

## 📥 Download and install

1. Go to [**Releases**](https://github.com/exa1t/NxtHUD/releases/latest) and download
   `NxtHUD-Setup-<version>.exe`.
2. Run it. It needs no administrator rights and no .NET install: everything it needs is inside.
   Windows may say *"Windows protected your PC"* because the installer is not code-signed;
   click **More info → Run anyway**.
3. Keep *"Start NxtHUD automatically when I sign in"* ticked and finish. NxtHUD starts and
   opens its settings; afterwards it lives in the notification area (possibly behind the `^`
   arrow).

**Coming from NxtFlyout?** NxtHUD is the same app under a new name (from 1.3.0). Run the
NxtHUD installer over your NxtFlyout install: it upgrades it in place, removes the old
shortcuts, and NxtHUD brings your settings and start-with-Windows choice across on its first
start. Your old `%AppData%\NxtFlyout` folder is left untouched.

From 1.4.0 NxtHUD tells you when a new version is out: a notification and a *Download* item in
the tray menu, both opening the release page. It checks GitHub at start-up and once a day; switch
it off under **Settings → About**.

To upgrade, run a newer installer: it closes the running copy and keeps your settings. To remove
it, use **Settings → Apps → Installed apps → NxtHUD → Uninstall**.

No installer wanted? Download `NxtHUD-<version>-portable-x64.zip`, unzip it anywhere and run
`NxtHUD.exe`.

## 🎵 Features

- **Media flyout** – press a media key (play/pause, next, previous, stop) to see the current
  track: album art, title, artist, player name, a live progress bar and previous / play-pause /
  next buttons.
- **Volume flyout** (optional) – a Fluent volume indicator for the volume and mute keys. By
  default NxtHUD handles those keys itself, so the built-in Windows overlay does not appear
  as well.
- **Lock keys flyout** – shows the new state when you toggle Caps Lock, Num Lock or Scroll Lock.
- **Up Next flyout** (optional) – briefly announces the new track when the song changes on its own.
- **Taskbar widget** (optional) – a compact now-playing widget on the taskbar with cover, title,
  artist and previous / play-pause / next. Click it to open the media flyout.
- **Taskbar visualizer** – animated bars next to the taskbar widget that follow the audio
  (WASAPI loopback). 10–20 bars that grow from the middle, bottom up or top down; colour can match
  the taskbar, use the accent colour or a custom hex value.
- **Taskbar stats** (optional) – CPU, memory and GPU use, network up/down and disk read/write
  in the free space next to the notification icons, refreshed every second (or at your pace).
  GPU memory and GPU temperature can be added when the graphics driver reports them. Click the
  figures to open Task Manager. No administrator rights or drivers needed.
- **Per-flyout position** – each flyout has its own on/off switch and screen position.
- **Mica / Acrylic** – flyouts and the settings window use the Windows 11 materials (or a solid
  themed card when you choose "None" or run an older Windows).
- **Multi-monitor and DPI aware** – flyouts appear on the primary display, the display with the
  pointer or the display with the active window, sized correctly for each display's scale.
- **Never in the way** – flyouts never take keyboard focus, stay out of Alt+Tab, stay open while
  the pointer is over them and can pause while a fullscreen game is running.
- **Tray app** – single instance, Fluent tray menu, optional start with Windows, System / Light /
  Dark theme that follows Windows and its accent colour live.
- **Update notices** – tells you once when a new release is out and takes you to its download.
- **Localizable** – every string comes from a resx file; English ships first.

## 💻 Requirements

- Windows 10 version 2004 (build 19041) or later. **Windows 11 22H2 or later** for Mica/Acrylic
  flyouts and rounded corners.
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or newer to build.

## 🛠️ Build and run

```powershell
dotnet build
dotnet test
dotnet run --project src/NxtHUD
```

The first build restores [WPF-UI](https://github.com/lepoco/wpfui),
[CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) and xUnit (tests only).

After `dotnet run`, look for the NxtHUD icon in the notification area (it may be behind the
`^` arrow). Left-click it for settings, right-click for the menu.

## 📦 Build the installer

The installer is a per-user [Inno Setup](https://jrsoftware.org/isinfo.php) package: no
administrator rights, installs to `%LocalAppData%\Programs\NxtHUD`, adds a Start menu entry,
optionally starts the app at sign-in, and registers an uninstaller. The app inside is
self-contained, so the target PC does not need .NET.

### On GitHub (recommended)

Releases are built by GitHub Actions on a Windows machine:

```powershell
git tag v1.4.0
git push origin v1.4.0
```

The **Release** workflow (`.github/workflows/release.yml`) takes the version from the tag, runs
the tests, builds the installer and portable zip, and publishes them with checksums as a GitHub
Release. Use **Actions → Release → Run workflow** to build a test copy without releasing it;
the files are attached to the workflow run.

The release notes are that version's section of `CHANGELOG.md` (under "What's new"), followed by
the install steps in `installer/release-notes.md`, so add the changelog entry before tagging.
Preview them with `.\installer\release-notes.ps1 -Version 1.4.0 -OutFile notes.md`.

The **CI** workflow builds and tests every push to `main`.

### On your PC

1. Install Inno Setup 6.3 or later, once:
   ```powershell
   winget install JRSoftware.InnoSetup
   ```
2. Quit NxtHUD if it is running, then from the solution folder:
   ```powershell
   powershell -ExecutionPolicy Bypass -File .\installer\build-installer.ps1
   ```
3. The installer, portable zip and `SHA256SUMS.txt` are in `artifacts\installer`.

Options: `-Version 1.2.3` to override the version in `Directory.Build.props`,
`-FrameworkDependent` for a much smaller build that requires the .NET 8 Desktop Runtime,
`-SkipTests` to skip the tests.

The installer is not code-signed, so SmartScreen warns about an unknown publisher until a
download has built up reputation. Uninstalling leaves your settings in `%AppData%\NxtHUD`.

## ⚙️ Settings

Settings are stored as JSON in `%AppData%\NxtHUD\settings.json` and are written whenever you
change something (About → *Open folder* takes you there). The file tolerates hand edits; a file
that cannot be parsed is copied to `settings.corrupt.json` and defaults are used.

| Page | Options |
| --- | --- |
| General | Start with Windows, language, pause in fullscreen games |
| Flyouts | On/off for each flyout and the taskbar widget; position of the volume, lock keys and Up Next flyouts; replace the Windows volume overlay; taskbar widget offset; visualizer on/off, bar count, direction and colour; hide-after delay |
| Appearance | Theme, backdrop material, which display to use, media flyout position, compact layout, animation speed |
| About | Version, check for updates (on/off, check now, download), settings folder, reset to defaults |

Errors are logged to `%AppData%\NxtHUD\logs\nxthud.log`.

## 🗂️ Project layout

```
NxtHUD.sln
src/
  NxtHUD.Core/        net8.0 – no WPF or Windows APIs, fully unit-testable
    Settings/            AppSettings, SettingsService (JSON), tolerant enum converter
    Media/               MediaSnapshot, IMediaService, media key map, text formatting
    Audio/               IVolumeService
    Input/               volume and lock key maps
    Flyouts/             FlyoutPlacement, FlyoutCoordinator
    Localization/        Localizer (resx lookup with live language switching)
    ViewModels/          FlyoutViewModelBase and one view model per flyout, the widget and settings
    Resources/           Strings.resx
  NxtHUD/             net8.0-windows – WPF app
    App.xaml(.cs)        start-up, single instance, shutdown, crash logging
    AppHost.cs           composition root: creates and wires the services
    Services/            SMTC media, Core Audio volume, keyboard hook, tray, theme, start-up,
                         flyout presenter, taskbar widget
    Interop/             Win32 declarations, display/DPI lookup
    Views/               FlyoutWindowBase, the flyout windows, taskbar widget, settings window
    Localization/        {loc:Loc Key} markup extension
    Assets/              app icon and logo
tests/
  NxtHUD.Tests/       xUnit tests for everything in Core
installer/               Inno Setup script, build script, release notes template
.github/workflows/       CI (build + test) and Release (installer → GitHub Release)
tools/icon/              SVG sources and the script that builds the .ico
docs/screenshots/        app screenshots the README cards are built from
docs/promo/              README banner and cards: edit cards.html, run render.ps1
```

## 🔍 How it works

- **Media** – `SmtcMediaService` follows the system's current media session through
  `Windows.Media.Control` (SMTC) and publishes an immutable `MediaSnapshot` on every change.
- **Keys** – `KeyboardHookService` installs one low-level keyboard hook (`WH_KEYBOARD_LL`), only
  while a flyout that needs it is enabled. Media and lock keys are observed and passed on.
  Volume keys are swallowed when "Replace the Windows volume overlay" is on, and the app changes
  the volume itself through the Core Audio API; if that ever fails, the keys are handed back to
  Windows.
- **View models** – all flyout logic (when to show, auto-hide, hover, fullscreen rule, one
  flyout at a time) lives in `NxtHUD.Core` and is unit-tested without any UI.
- **Windows** – `FlyoutWindowBase` builds each flyout as a topmost `WS_EX_NOACTIVATE` tool
  window, places it in physical pixels on the chosen display, and either asks Windows for a
  Mica/Acrylic backdrop or draws a solid rounded card.
- **Taskbar widget** – a small window owned by the taskbar window, repositioned on a slow timer.
  `TaskbarButtonScanner` finds the taskbar's buttons with UI Automation on a worker thread, and
  `TaskbarWidgetFit` picks the free space and the fullest layout that fits it.
- **Taskbar stats** – `SystemStatsSource` reads CPU (`GetSystemTimes`), memory
  (`GlobalMemoryStatusEx`), GPU, GPU memory and disk (the performance counters Task Manager
  uses, through PDH), network (adapter byte counts) and GPU temperature (`D3DKMTQueryAdapterInfo`)
  on a worker thread. `TaskbarStatsService` keeps the panel against the tray, clear of the widget.

## 🌍 Adding a language

1. Copy `src/NxtHUD.Core/Resources/Strings.resx` to `Strings.<culture>.resx` (for example
   `Strings.vi.resx`) and translate the values.
2. Add the culture name to `Localizer.SupportedLanguages`.

The language picker then offers it, and switching applies without a restart.

## ⚠️ Known limitations

- **Players without SMTC data.** Media info comes from the System Media Transport Controls.
  Players that do not publish to SMTC cannot be shown or controlled, and some publish only part
  of it (no album art, no timeline, or a position updated only on play/pause/seek, in which case
  the progress bar is an estimate).
- **"Up Next" shows at the track change, not before it.** Windows does not expose a player's
  queue, so the next track is not known until it starts.
- **No seeking, shuffle/repeat, session switcher or per-app volume mixer yet.** The flyout
  follows whichever session Windows treats as current, and the progress bar is display-only.
- **Volume keys that bypass the keyboard.** Replacing the Windows volume overlay works by
  handling the volume keys. Volume changed any other way (a hardware knob that does not send key
  presses, the taskbar slider, another app) still uses the Windows overlay.
- **Elevated windows.** A low-level keyboard hook is not delivered while an administrator window
  has focus unless NxtHUD is elevated too, so flyouts do not appear then, and with the overlay
  replacement on, the Windows overlay appears instead.
- **Taskbar widget.** Windows has no API for adding content to the taskbar. The widget is a
  separate window over a horizontal primary taskbar. It reads where the taskbar's own buttons are
  through UI Automation and only uses the free space between them: at the left end with centred
  icons, after the last icon with left-aligned ones. When space runs short it drops to a narrower
  title, then hides the visualizer, then the text, and it hides itself if even the cover and
  buttons do not fit. It re-checks every 1.5 seconds, so for a moment after opening an app the
  icons can overlap it. It is hidden for vertical or auto-hidden taskbars and fullscreen apps.
- **Taskbar stats.** Like the widget, the stats are a separate window beside the tray, not part
  of the clock, and they step down to one line, then hide, when the free space runs out. GPU
  temperature needs a recent graphics driver (WDDM 2.5 or later); CPU and disk temperatures
  are not shown, because reading them needs administrator rights or a kernel driver. Network
  figures count physical adapters only, so VPN traffic is not counted twice.
- **Backdrop.** Mica/Acrylic flyouts need Windows 11 22H2+, and Windows turns them off in energy
  saver (use Translucent there: see-through, but without blur). On older versions flyouts use the
  solid card. With a system backdrop, the content fades but the material itself appears and
  disappears without a fade.
- **Fullscreen detection** relies on the Windows notification state, which also reports a
  browser in F11 fullscreen as busy. Turn "Pause in fullscreen games" off to see flyouts there.
- **Start with Windows** uses the per-user registry Run key.
- **English only** for now; the structure for more languages is in place.

- **Visualizer.** It listens to the default playback device as it was when playback started; after
  switching output devices it may stay flat until the next pause/play. It reacts to all system
  audio, not only the media player.

## 🙌 Credits

NxtHUD's code, icon and branding are original. It builds on
[WPF-UI](https://github.com/lepoco/wpfui) (MIT),
[CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) (MIT) and
[NAudio](https://github.com/naudio/NAudio) (MIT). Their copyright and licence notices are in
[THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt), which is also installed next to
`NxtHUD.exe`.

## 📄 License

NxtHUD is released under the [MIT License](LICENSE).
