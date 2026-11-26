using System.IO;
using System.Windows.Threading;
using NxtHUD.Core.Media;
using Windows.Foundation;
using Windows.Media.Control;
using Windows.Storage.Streams;

namespace NxtHUD.Services;

/// <summary>
/// Follows the current media session through the System Media Transport Controls (SMTC),
/// the same source Windows uses for its own media overlay and lock screen.
/// </summary>
/// <remarks>
/// <para>Windows raises SMTC events on background threads. Every handler here hops to the UI
/// thread first, so all state is touched from one thread and no locking is needed.</para>
/// <para>Sessions can disappear at any moment (the player closes), which makes calls on them
/// throw. Those failures are expected and are swallowed at the call sites.</para>
/// </remarks>
internal sealed class SmtcMediaService : IMediaService, IDisposable
{
    private readonly Dispatcher _dispatcher;

    private GlobalSystemMediaTransportControlsSessionManager? _manager;
    private GlobalSystemMediaTransportControlsSession? _session;
    private MediaSnapshot? _current;
    private int _mediaPropertiesVersion;
    private bool _disposed;

    public SmtcMediaService(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public MediaSnapshot? Current => _current;

    public event EventHandler? Changed;

    /// <summary>Connects to SMTC and starts following the current session. Call on the UI thread.</summary>
    public async Task StartAsync()
    {
        var manager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
        if (_disposed)
        {
            return;
        }

        _manager = manager;
        manager.CurrentSessionChanged += OnCurrentSessionChanged;
        AttachSession(GetCurrentSession());
    }

    public Task TogglePlayPauseAsync() => SendAsync(session => session.TryTogglePlayPauseAsync());

    public Task SkipNextAsync() => SendAsync(session => session.TrySkipNextAsync());

    public Task SkipPreviousAsync() => SendAsync(session => session.TrySkipPreviousAsync());

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_manager is not null)
        {
            _manager.CurrentSessionChanged -= OnCurrentSessionChanged;
            _manager = null;
        }

        DetachSession();
    }

    // ----- Session tracking -----

    private void OnCurrentSessionChanged(
        GlobalSystemMediaTransportControlsSessionManager sender,
        CurrentSessionChangedEventArgs args)
    {
        _dispatcher.InvokeAsync(() =>
        {
            if (!_disposed)
            {
                AttachSession(GetCurrentSession());
            }
        });
    }

    private GlobalSystemMediaTransportControlsSession? GetCurrentSession()
    {
        try
        {
            return _manager?.GetCurrentSession();
        }
        catch (Exception ex)
        {
            Log.Error("Reading the current media session failed.", ex);
            return null;
        }
    }

    private void AttachSession(GlobalSystemMediaTransportControlsSession? session)
    {
        DetachSession();
        _session = session;

        if (session is null)
        {
            Publish(null);
            return;
        }

        var sourceAppId = string.Empty;
        try
        {
            sourceAppId = session.SourceAppUserModelId ?? string.Empty;

            session.MediaPropertiesChanged += OnMediaPropertiesChanged;
            session.PlaybackInfoChanged += OnPlaybackInfoChanged;
            session.TimelinePropertiesChanged += OnTimelinePropertiesChanged;
        }
        catch (Exception ex)
        {
            Log.Error("Attaching to the media session failed.", ex);
        }

        // Windows sometimes re-announces the same player. Keep its title and art on screen
        // until the fresh ones arrive instead of flashing an empty flyout.
        var previous = _current;
        var snapshot = previous is not null && previous.SourceAppId == sourceAppId
            ? previous
            : new MediaSnapshot { SourceAppId = sourceAppId };

        snapshot = ReadPlayback(snapshot, session);
        snapshot = ReadTimeline(snapshot, session);
        Publish(snapshot);

        _ = RefreshMediaPropertiesAsync(session);
    }

    private void DetachSession()
    {
        var session = _session;
        _session = null;
        _mediaPropertiesVersion++;

        if (session is null)
        {
            return;
        }

        try
        {
            session.MediaPropertiesChanged -= OnMediaPropertiesChanged;
            session.PlaybackInfoChanged -= OnPlaybackInfoChanged;
            session.TimelinePropertiesChanged -= OnTimelinePropertiesChanged;
        }
        catch (Exception)
        {
            // The session is already gone; there is nothing left to unsubscribe from.
        }
    }

    // ----- Session events (raised on background threads) -----
    //
    // The handlers deliberately ignore their "sender" argument and work with the session this
    // service attached to: a notification that was already queued when the session changed
    // then simply causes one harmless extra refresh of the new session.

    private void OnMediaPropertiesChanged(
        GlobalSystemMediaTransportControlsSession sender,
        MediaPropertiesChangedEventArgs args)
    {
        _dispatcher.InvokeAsync(() =>
        {
            var session = _session;
            if (!_disposed && session is not null)
            {
                _ = RefreshMediaPropertiesAsync(session);
            }
        });
    }

    private void OnPlaybackInfoChanged(
        GlobalSystemMediaTransportControlsSession sender,
        PlaybackInfoChangedEventArgs args)
    {
        _dispatcher.InvokeAsync(() =>
        {
            var session = _session;
            if (!_disposed && session is not null && _current is not null)
            {
                Publish(ReadPlayback(_current, session));
            }
        });
    }

    private void OnTimelinePropertiesChanged(
        GlobalSystemMediaTransportControlsSession sender,
        TimelinePropertiesChangedEventArgs args)
    {
        _dispatcher.InvokeAsync(() =>
        {
            var session = _session;
            if (!_disposed && session is not null && _current is not null)
            {
                Publish(ReadTimeline(_current, session));
            }
        });
    }

    private bool IsCurrent(GlobalSystemMediaTransportControlsSession session)
    {
        return !_disposed && ReferenceEquals(session, _session);
    }

    // ----- Reading session state -----

    private async Task RefreshMediaPropertiesAsync(GlobalSystemMediaTransportControlsSession session)
    {
        // Track changes arrive in bursts; only the newest request may publish its result.
        var version = ++_mediaPropertiesVersion;

        try
        {
            var properties = await session.TryGetMediaPropertiesAsync();
            if (version != _mediaPropertiesVersion || !IsCurrent(session))
            {
                return;
            }

            byte[]? artwork = null;
            if (properties?.Thumbnail is { } thumbnail)
            {
                artwork = await ReadThumbnailAsync(thumbnail);
                if (version != _mediaPropertiesVersion || !IsCurrent(session))
                {
                    return;
                }
            }

            if (_current is null)
            {
                return;
            }

            var title = properties?.Title ?? string.Empty;
            Publish(_current with
            {
                Title = title,
                Artist = properties?.Artist ?? string.Empty,
                AlbumTitle = properties?.AlbumTitle ?? string.Empty,
                Artwork = artwork,
            });

            if (artwork is null && title.Length > 0)
            {
                _ = RetryArtworkAsync(session, version, title);
            }
        }
        catch (Exception ex)
        {
            Log.Error("Reading media properties failed.", ex);
        }
    }

    /// <summary>Waits before each new try for a cover that is not ready yet.</summary>
    private static readonly TimeSpan[] ArtworkRetryDelays =
    {
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(2.5),
        TimeSpan.FromSeconds(5),
    };

    /// <summary>
    /// Some players (Spotify among them) announce a new track before its cover is ready, hand
    /// out an empty image, and do not say when the real one arrives. Ask again a few times
    /// while the same track is still playing.
    /// </summary>
    private async Task RetryArtworkAsync(GlobalSystemMediaTransportControlsSession session, int version, string title)
    {
        foreach (var delay in ArtworkRetryDelays)
        {
            await Task.Delay(delay);
            if (version != _mediaPropertiesVersion || !IsCurrent(session) || _current?.Title != title || _current.Artwork is not null)
            {
                return;
            }

            try
            {
                var properties = await session.TryGetMediaPropertiesAsync();
                if (properties?.Title != title || properties.Thumbnail is not { } thumbnail)
                {
                    continue;
                }

                var artwork = await ReadThumbnailAsync(thumbnail);
                if (artwork is not null
                    && version == _mediaPropertiesVersion
                    && IsCurrent(session)
                    && _current is { } current
                    && current.Title == title
                    && current.Artwork is null)
                {
                    Publish(current with { Artwork = artwork });
                    return;
                }
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                // Art is optional; give up quietly and keep the player icon.
                return;
            }
        }
    }

    private static async Task<byte[]?> ReadThumbnailAsync(IRandomAccessStreamReference thumbnail)
    {
        try
        {
            using var source = await thumbnail.OpenReadAsync();
            if (source is null || source.Size == 0)
            {
                return null;
            }

            using var input = source.AsStreamForRead();
            using var buffer = new MemoryStream();
            await input.CopyToAsync(buffer);
            return buffer.Length == 0 ? null : buffer.ToArray();
        }
        catch (Exception)
        {
            // Art is optional; some players hand out thumbnails that cannot be opened.
            return null;
        }
    }

    private static MediaSnapshot ReadPlayback(MediaSnapshot snapshot, GlobalSystemMediaTransportControlsSession session)
    {
        try
        {
            var info = session.GetPlaybackInfo();
            if (info is null)
            {
                return snapshot;
            }

            var controls = info.Controls;
            return snapshot with
            {
                PlaybackState = ToPlaybackState(info.PlaybackStatus),
                PlaybackRate = info.PlaybackRate ?? 1.0,
                CanTogglePlayPause = controls is not null
                    && (controls.IsPlayPauseToggleEnabled || controls.IsPlayEnabled || controls.IsPauseEnabled),
                CanGoNext = controls is not null && controls.IsNextEnabled,
                CanGoPrevious = controls is not null && controls.IsPreviousEnabled,
            };
        }
        catch (Exception)
        {
            return snapshot;
        }
    }

    private static MediaSnapshot ReadTimeline(MediaSnapshot snapshot, GlobalSystemMediaTransportControlsSession session)
    {
        try
        {
            var timeline = session.GetTimelineProperties();
            if (timeline is null)
            {
                return snapshot;
            }

            var duration = timeline.EndTime - timeline.StartTime;
            return snapshot with
            {
                Position = timeline.Position - timeline.StartTime,
                Duration = duration > TimeSpan.Zero ? duration : TimeSpan.Zero,
                PositionUpdatedAt = timeline.LastUpdatedTime,
            };
        }
        catch (Exception)
        {
            return snapshot;
        }
    }

    private static MediaPlaybackState ToPlaybackState(GlobalSystemMediaTransportControlsSessionPlaybackStatus status)
    {
        return status switch
        {
            GlobalSystemMediaTransportControlsSessionPlaybackStatus.Closed => MediaPlaybackState.Closed,
            GlobalSystemMediaTransportControlsSessionPlaybackStatus.Opened => MediaPlaybackState.Opened,
            GlobalSystemMediaTransportControlsSessionPlaybackStatus.Changing => MediaPlaybackState.Changing,
            GlobalSystemMediaTransportControlsSessionPlaybackStatus.Stopped => MediaPlaybackState.Stopped,
            GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing => MediaPlaybackState.Playing,
            GlobalSystemMediaTransportControlsSessionPlaybackStatus.Paused => MediaPlaybackState.Paused,
            _ => MediaPlaybackState.Unknown,
        };
    }

    // ----- Publishing and commands -----

    private void Publish(MediaSnapshot? snapshot)
    {
        if (Equals(snapshot, _current))
        {
            return;
        }

        _current = snapshot;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private async Task SendAsync(Func<GlobalSystemMediaTransportControlsSession, IAsyncOperation<bool>> command)
    {
        var session = _session;
        if (session is null)
        {
            return;
        }

        try
        {
            await command(session);
        }
        catch (Exception ex)
        {
            Log.Error("Sending a media command failed.", ex);
        }
    }
}
