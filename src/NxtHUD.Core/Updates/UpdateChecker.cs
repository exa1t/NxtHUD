using CommunityToolkit.Mvvm.ComponentModel;

namespace NxtHUD.Core.Updates;

/// <summary>Where the latest release's tag comes from (GitHub in the app, a fake in tests).</summary>
public interface IReleaseSource
{
    /// <summary>The latest release's tag, such as "v1.3.1".</summary>
    Task<string> GetLatestTagAsync(CancellationToken cancellationToken);
}

public enum UpdateStatus
{
    /// <summary>No check has finished yet.</summary>
    Unknown,
    Checking,
    UpToDate,
    Available,
    Failed,
}

/// <summary>
/// Compares the running version with the latest GitHub release. It only finds and reports a
/// newer version; downloading and installing is left to the user (the release page).
/// </summary>
public sealed class UpdateChecker : ObservableObject
{
    private readonly IReleaseSource _source;
    private readonly Version? _current;

    private UpdateStatus _status;
    private Version? _latest;
    private Task<UpdateStatus>? _running;

    public UpdateChecker(IReleaseSource source, string currentVersion)
    {
        ArgumentNullException.ThrowIfNull(source);
        _source = source;
        _current = ParseVersion(currentVersion);
    }

    public UpdateStatus Status
    {
        get => _status;
        private set => SetProperty(ref _status, value);
    }

    /// <summary>The newer version when <see cref="Status"/> is <see cref="UpdateStatus.Available"/>, as "1.3.2".</summary>
    public string? AvailableVersion => _status == UpdateStatus.Available ? Format(_latest) : null;

    /// <summary>Why the last check failed, if it threw.</summary>
    public Exception? LastError { get; private set; }

    /// <summary>Checks once; a second call while one is running waits for that one.</summary>
    public Task<UpdateStatus> CheckAsync(CancellationToken cancellationToken = default)
    {
        if (_running is { IsCompleted: false } running)
        {
            return running;
        }

        _running = RunAsync(cancellationToken);
        return _running;
    }

    /// <summary>
    /// Turns "v1.3.1", "1.3.1" or "1.3.1-beta+abc" into a version, or null when it is not one.
    /// </summary>
    public static Version? ParseVersion(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var value = text.Trim();
        if (value.StartsWith('v') || value.StartsWith('V'))
        {
            value = value[1..];
        }

        var end = value.IndexOfAny(new[] { '-', '+', ' ' });
        if (end >= 0)
        {
            value = value[..end];
        }

        if (!Version.TryParse(value, out var version))
        {
            return null;
        }

        // Treat 1.3 and 1.3.0 alike, and 1.3.0.0 like 1.3.0; keep a real fourth part (1.4.0.1).
        var build = Math.Max(0, version.Build);
        return version.Revision > 0
            ? new Version(version.Major, version.Minor, build, version.Revision)
            : new Version(version.Major, version.Minor, build);
    }

    /// <summary>Whether <paramref name="latest"/> is newer than <paramref name="current"/>; false if either is not a version.</summary>
    public static bool IsNewer(string? latest, string? current)
    {
        return ParseVersion(latest) is { } l && ParseVersion(current) is { } c && l > c;
    }

    private async Task<UpdateStatus> RunAsync(CancellationToken cancellationToken)
    {
        Status = UpdateStatus.Checking;
        LastError = null;
        try
        {
            var tag = await _source.GetLatestTagAsync(cancellationToken).ConfigureAwait(true);
            var latest = ParseVersion(tag);
            if (latest is null || _current is null)
            {
                Status = UpdateStatus.Failed;
            }
            else
            {
                _latest = latest;
                Status = latest > _current ? UpdateStatus.Available : UpdateStatus.UpToDate;
            }
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            // Offline, GitHub unreachable, rate limited: try again next time.
            LastError = ex;
            Status = UpdateStatus.Failed;
        }

        OnPropertyChanged(nameof(AvailableVersion));
        return Status;
    }

    private static string? Format(Version? version) => version?.ToString(version.Revision > 0 ? 4 : 3);
}
