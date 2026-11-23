using System.Net.Http;
using NxtHUD.Core.Updates;

namespace NxtHUD.Services;

/// <summary>
/// Reads the latest release's tag from GitHub.
/// </summary>
/// <remarks>
/// GitHub answers ".../releases/latest" with a redirect to ".../releases/tag/v1.3.1". Reading
/// that redirect, rather than calling the REST API, needs no token and is not subject to the
/// API's 60-requests-an-hour limit for anonymous callers. Nothing about the user is sent.
/// </remarks>
internal sealed class GitHubReleaseSource : IReleaseSource, IDisposable
{
    private const string TagMarker = "/releases/tag/";

    private readonly HttpClient _http;

    public GitHubReleaseSource()
    {
        _http = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false })
        {
            Timeout = TimeSpan.FromSeconds(15),
        };
        _http.DefaultRequestHeaders.UserAgent.ParseAdd($"{AppInfo.Name}/{AppInfo.Version}");
    }

    public async Task<string> GetLatestTagAsync(CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Head, AppInfo.LatestReleaseUrl);
        using var response = await _http.SendAsync(request, cancellationToken).ConfigureAwait(false);

        var location = response.Headers.Location?.ToString()
            ?? throw new HttpRequestException($"GitHub answered {(int)response.StatusCode} without a redirect.");

        var index = location.IndexOf(TagMarker, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            throw new HttpRequestException($"GitHub redirected to an unexpected address: {location}");
        }

        return Uri.UnescapeDataString(location[(index + TagMarker.Length)..].TrimEnd('/'));
    }

    public void Dispose() => _http.Dispose();
}
