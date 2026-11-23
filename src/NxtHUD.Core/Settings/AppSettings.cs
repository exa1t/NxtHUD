namespace NxtHUD.Core.Settings;

/// <summary>
/// Everything the user can configure. Serialized as JSON to %AppData%\NxtHUD\settings.json.
/// </summary>
/// <remarks>
/// This is a record so two snapshots can be compared by value. Instances handed out by
/// <see cref="ISettingsService.Current"/> are snapshots and must not be mutated; change
/// settings through <see cref="ISettingsService.Update"/> instead.
/// </remarks>
public sealed record AppSettings
{
    public const int CurrentSchemaVersion = 3;

    public const int MinTimeoutMs = 500;
    public const int MaxTimeoutMs = 15000;
    public const int DefaultTimeoutMs = 3000;

    /// <summary>Largest allowed <see cref="TaskbarWidgetOffset"/>, in device-independent pixels.</summary>
    public const int MaxTaskbarWidgetOffset = 1000;

    public const int MinTranslucentOpacity = 50;
    public const int MaxTranslucentOpacity = 100;
    public const int DefaultTranslucentOpacity = 80;

    public const int MinVisualizerBars = 10;
    public const int MaxVisualizerBars = 20;
    public const int DefaultVisualizerBars = 14;
    public const string DefaultVisualizerColor = "#4CC2FF";

    public const int MinStatsIntervalMs = 500;
    public const int MaxStatsIntervalMs = 5000;
    public const int DefaultStatsIntervalMs = 1000;

    /// <summary>Value of <see cref="Language"/> that means "follow the Windows display language".</summary>
    public const string SystemLanguage = "system";

    /// <summary>Version of the settings file format, for future migrations.</summary>
    public int SchemaVersion { get; set; } = CurrentSchemaVersion;

    // ----- General -----

    public bool StartWithWindows { get; set; }

    /// <summary><see cref="SystemLanguage"/> or a culture name such as "en".</summary>
    public string Language { get; set; } = SystemLanguage;

    /// <summary>Suppress flyouts while a fullscreen game or app is in the foreground.</summary>
    public bool DisableInFullscreen { get; set; } = true;

    /// <summary>Look for a newer release on GitHub at start-up and once a day.</summary>
    public bool CheckForUpdates { get; set; } = true;

    /// <summary>The newest version the tray has already announced, so each one is announced once.</summary>
    public string? LastAnnouncedUpdate { get; set; }

    // ----- Appearance -----

    public AppTheme Theme { get; set; } = AppTheme.System;

    public BackdropKind Backdrop { get; set; } = BackdropKind.Mica;

    /// <summary>Opacity of the flyout background in percent when <see cref="Backdrop"/> is Translucent.</summary>
    public int TranslucentOpacity { get; set; } = DefaultTranslucentOpacity;

    public FlyoutVerticalPosition VerticalPosition { get; set; } = FlyoutVerticalPosition.Bottom;

    /// <summary>Flyouts sit at the bottom-left by default, just above the taskbar.</summary>
    public FlyoutHorizontalPosition HorizontalPosition { get; set; } = FlyoutHorizontalPosition.Left;

    public bool CompactLayout { get; set; }

    public AnimationSpeed AnimationSpeed { get; set; } = AnimationSpeed.Normal;

    /// <summary>Which display flyouts appear on.</summary>
    public FlyoutDisplay Display { get; set; } = FlyoutDisplay.Primary;

    /// <summary>How long a flyout stays visible before it hides itself, in milliseconds.</summary>
    public int FlyoutTimeoutMs { get; set; } = DefaultTimeoutMs;

    // ----- Flyouts -----

    public bool MediaFlyoutEnabled { get; set; } = true;

    public bool VolumeFlyoutEnabled { get; set; }

    public bool UpNextFlyoutEnabled { get; set; }

    public bool LockKeysFlyoutEnabled { get; set; } = true;

    public bool TaskbarWidgetEnabled { get; set; }

    // The media flyout's position is VerticalPosition + HorizontalPosition above (kept for
    // compatibility with existing settings files); the other flyouts each have their own.

    public FlyoutPosition VolumePosition { get; set; } = FlyoutPosition.BottomCenter;

    public FlyoutPosition LockKeysPosition { get; set; } = FlyoutPosition.BottomCenter;

    public FlyoutPosition UpNextPosition { get; set; } = FlyoutPosition.BottomLeft;

    /// <summary>
    /// Handle the volume keys in the app (instead of merely watching them), which keeps the
    /// built-in Windows volume overlay from appearing. Only applies while the volume flyout is on.
    /// </summary>
    public bool ReplaceNativeVolumeOverlay { get; set; } = true;

    /// <summary>Distance of the taskbar widget from the left end of the taskbar.</summary>
    public int TaskbarWidgetOffset { get; set; }

    /// <summary>Show animated bars that follow the audio next to the taskbar widget.</summary>
    public bool TaskbarVisualizerEnabled { get; set; } = true;

    /// <summary>Number of visualizer bars, between <see cref="MinVisualizerBars"/> and <see cref="MaxVisualizerBars"/>.</summary>
    public int VisualizerBarCount { get; set; } = DefaultVisualizerBars;

    public VisualizerColorMode VisualizerColorMode { get; set; } = VisualizerColorMode.Accent;

    /// <summary>Which way the visualizer bars grow.</summary>
    public VisualizerAlignment VisualizerAlignment { get; set; } = VisualizerAlignment.Center;

    /// <summary>Colour used when <see cref="VisualizerColorMode"/> is Custom, as "#RRGGBB" or "#AARRGGBB".</summary>
    public string VisualizerCustomColor { get; set; } = DefaultVisualizerColor;

    // ----- Taskbar stats -----

    /// <summary>Show CPU, memory, GPU, network and disk figures in the free space of the taskbar.</summary>
    public bool TaskbarStatsEnabled { get; set; }

    public bool StatsShowCpu { get; set; } = true;

    public bool StatsShowRam { get; set; } = true;

    public bool StatsShowGpu { get; set; } = true;

    /// <summary>Dedicated GPU memory in use.</summary>
    public bool StatsShowVram { get; set; }

    /// <summary>GPU temperature, where the graphics driver reports it.</summary>
    public bool StatsShowGpuTemperature { get; set; }

    public bool StatsShowNetwork { get; set; } = true;

    public bool StatsShowDisk { get; set; } = true;

    /// <summary>How often the figures refresh, in milliseconds.</summary>
    public int StatsIntervalMs { get; set; } = DefaultStatsIntervalMs;

    /// <summary>
    /// App id of the media session the user pinned, or <c>null</c> for "Automatic"
    /// (follow whichever session Windows considers current).
    /// </summary>
    public string? PinnedMediaSessionId { get; set; }

    /// <summary>Returns an independent copy that is equal to this instance.</summary>
    public AppSettings Copy() => this with { };

    /// <summary>
    /// Repairs out-of-range or unknown values in place, so the rest of the app can trust
    /// whatever it reads. Called after every load and every update.
    /// </summary>
    public void Normalize()
    {
        // Schema 1 defaulted to bottom-centre. The default is now bottom-left, so a file
        // from before the change that still holds the old default moves with it.
        if (SchemaVersion < 2 && HorizontalPosition == FlyoutHorizontalPosition.Center)
        {
            HorizontalPosition = FlyoutHorizontalPosition.Left;
        }

        SchemaVersion = CurrentSchemaVersion;

        Language = string.IsNullOrWhiteSpace(Language) ? SystemLanguage : Language.Trim();

        if (!Enum.IsDefined(Theme))
        {
            Theme = AppTheme.System;
        }

        if (!Enum.IsDefined(Backdrop))
        {
            Backdrop = BackdropKind.Mica;
        }

        if (!Enum.IsDefined(VerticalPosition))
        {
            VerticalPosition = FlyoutVerticalPosition.Bottom;
        }

        if (!Enum.IsDefined(HorizontalPosition))
        {
            HorizontalPosition = FlyoutHorizontalPosition.Left;
        }

        if (!Enum.IsDefined(AnimationSpeed))
        {
            AnimationSpeed = AnimationSpeed.Normal;
        }

        if (!Enum.IsDefined(Display))
        {
            Display = FlyoutDisplay.Primary;
        }

        if (!Enum.IsDefined(VolumePosition))
        {
            VolumePosition = FlyoutPosition.BottomCenter;
        }

        if (!Enum.IsDefined(LockKeysPosition))
        {
            LockKeysPosition = FlyoutPosition.BottomCenter;
        }

        if (!Enum.IsDefined(UpNextPosition))
        {
            UpNextPosition = FlyoutPosition.BottomLeft;
        }

        FlyoutTimeoutMs = Math.Clamp(FlyoutTimeoutMs, MinTimeoutMs, MaxTimeoutMs);
        TaskbarWidgetOffset = Math.Clamp(TaskbarWidgetOffset, 0, MaxTaskbarWidgetOffset);

        TranslucentOpacity = Math.Clamp(TranslucentOpacity, MinTranslucentOpacity, MaxTranslucentOpacity);

        VisualizerBarCount = Math.Clamp(VisualizerBarCount, MinVisualizerBars, MaxVisualizerBars);
        StatsIntervalMs = Math.Clamp(StatsIntervalMs, MinStatsIntervalMs, MaxStatsIntervalMs);

        if (!Enum.IsDefined(VisualizerColorMode))
        {
            VisualizerColorMode = VisualizerColorMode.Accent;
        }

        if (!Enum.IsDefined(VisualizerAlignment))
        {
            VisualizerAlignment = VisualizerAlignment.Center;
        }

        VisualizerCustomColor = IsValidHexColor(VisualizerCustomColor)
            ? VisualizerCustomColor.Trim().ToUpperInvariant()
            : DefaultVisualizerColor;

        if (string.IsNullOrWhiteSpace(PinnedMediaSessionId))
        {
            PinnedMediaSessionId = null;
        }
    }

    /// <summary>True for "#RRGGBB" or "#AARRGGBB" (hex digits, either case).</summary>
    public static bool IsValidHexColor(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var text = value.Trim();
        if (text.Length is not (7 or 9) || text[0] != '#')
        {
            return false;
        }

        for (var i = 1; i < text.Length; i++)
        {
            if (!Uri.IsHexDigit(text[i]))
            {
                return false;
            }
        }

        return true;
    }
}
