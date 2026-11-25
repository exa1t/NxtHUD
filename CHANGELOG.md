# Changelog

## 1.4.3

- **Fixed: the music widget on the taskbar stopped reacting to clicks.** It stays above the
  taskbar only while Windows keeps it attached to the taskbar, and something (seen after
  monitor changes) could detach it, leaving the taskbar on top of it. The widget and the stats
  panel now check every 1.5 seconds and reattach themselves, noting it in the log.

## 1.4.2

- **Fixed, the real cause this time: the taskbar stats panel turning into a gray box.** Whenever
  Windows announced a colour or theme change (energy saver switching transparency off, the
  settings Windows applies a minute or two after sign-in, waking from sleep), NxtHUD's theme
  refresh gave the stats panel a themed background, because WPF had picked the panel as the
  app's "main window". The panel and the music widget now never take that role, like the
  flyouts and the settings window already didn't.

## 1.4.1

- **Lighter on games:** while a fullscreen game or app covers the taskbar, NxtHUD no longer
  re-reads the taskbar's buttons every 1.5 seconds. The widget and stats are hidden then anyway;
  they pick up where they left off when you leave the game.
- **Waking from a dimmed screen** now counts as waking too: some PCs go from dimmed straight
  back to on, and NxtHUD missed that when deciding to refresh the taskbar windows.
- **Update check:** a four-part version such as 1.4.0.1 is now recognised as newer than 1.4.0.

## 1.4.0

- **Update notices.** NxtHUD now checks GitHub for a newer release 30 seconds after it starts
  and once a day. When one is out it tells you once with a notification, and adds a
  "Download NxtHUD x" item to the tray menu; both open the release page. Settings → About has
  a new card to switch this off, check now, or download. Nothing about you is sent.
- **Fixed for good: the taskbar stats panel and music widget turning gray.** 1.3.1 rebuilt them
  after waking from sleep, but they could also go gray right after starting Windows. They are
  now drawn by the processor instead of the graphics card, so a graphics reset (sign-in, sleep,
  driver updates, switching graphics chips) can no longer wipe their see-through background.
- **Fixed: start-with-Windows jumping to another copy.** Running a second copy of NxtHUD (the
  portable zip, or a build) used to re-point start-with-Windows at that copy. Now it only moves
  the entry when the registered copy no longer exists, for example after you moved the folder.

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
