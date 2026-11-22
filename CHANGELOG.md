# Changelog

## 1.3.1

- **Fixed: the taskbar stats panel (and the music widget) turning into a gray box** after the
  PC woke from sleep. Waking resets the graphics, and the see-through windows lost their
  transparency until you toggled them. NxtHUD now rebuilds them on its own a couple of seconds
  after the screen comes back on, and after display changes.

## 1.3.0

- **NxtFlyout is now NxtHUD.** Same app, new name: the installer, the exe, the Start menu
  entry, the tray and the settings window all say NxtHUD. The project moves to
  github.com/exa1t/NxtHUD (the old address redirects).
- **Upgrading from NxtFlyout** just works: run the NxtHUD installer over it. It keeps your
  install folder, removes NxtFlyout's old program files and shortcuts, and closes a running
  NxtFlyout first. On its first start NxtHUD copies your settings (and logs) from
  `%AppData%\NxtFlyout` to `%AppData%\NxtHUD` and moves start-with-Windows to the new name,
  keeping it off if you had switched it off in Task Manager. The old folder is left in place.

## 1.2.0

- **Taskbar stats.** Optional CPU, memory, GPU, network and disk figures in the free space next
  to the notification icons, with GPU memory and GPU temperature as extras. Each figure can be
  switched on or off, the refresh pace is adjustable, and a click opens Task Manager. Off by
  default: turn it on in Settings → Flyouts → Taskbar stats.

## 1.1.2

- **Volume keys work while music plays.** With the taskbar visualizer running, the volume keys
  failed and were handed back to Windows, so the Windows volume overlay appeared instead of
  NxtFlyout's flyout. Fixed.
- **Taskbar widget stays clear of the taskbar's icons.** It now finds where the Start button,
  app icons, Widgets button and tray are, and only uses the free space between them. When space
  runs short it narrows the title, then hides the visualizer, then the text, and hides itself if
  even the cover and buttons do not fit. With left-aligned icons it moves after the last icon.

## 1.1.1

- NxtFlyout is now MIT-licensed (`LICENSE`). The installer and portable zip include `LICENSE.txt`
  and `THIRD-PARTY-NOTICES.txt` with the licences of the bundled libraries.

## 1.1.0

- **One-click install.** Every release now ships `NxtFlyout-Setup-<version>.exe`, built
  automatically on GitHub. It bundles everything it needs (no .NET install), needs no
  administrator rights, and can start NxtFlyout at sign-in.
- Shorter setup wizard: options, install, done. Optional desktop shortcut.
- Upgrades close the running copy and keep settings; a silent upgrade restarts the app.
- Portable zip and SHA-256 checksums alongside the installer.
- Faster start-up: the app is precompiled (ReadyToRun).
- CI builds and tests every push to `main`.

## 1.0.0

- Media, volume, lock keys and Up Next flyouts; taskbar widget and visualizer; Fluent settings
  window; Mica/Acrylic; multi-monitor and DPI support; English localization.
