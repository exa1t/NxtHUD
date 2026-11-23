using NxtHUD.Core.Settings;
using NxtHUD.Core.Updates;
using NxtHUD.Core.ViewModels;
using Xunit;

namespace NxtHUD.Tests;

public class UpdateCheckerTests
{
    private sealed class FakeReleaseSource : IReleaseSource
    {
        public Func<Task<string>> Next { get; set; } = () => Task.FromResult("v1.0.0");

        public int Calls { get; private set; }

        public Task<string> GetLatestTagAsync(CancellationToken cancellationToken)
        {
            Calls++;
            return Next();
        }
    }

    [Theory]
    [InlineData("v1.3.1", "1.3.1")]
    [InlineData("1.3.1", "1.3.1")]
    [InlineData("V2.0", "2.0.0")]
    [InlineData(" v1.4.0-beta.1 ", "1.4.0")]
    [InlineData("1.3.1+abc123", "1.3.1")]
    public void ParseVersion_ReadsTagsAndVersions(string text, string expected)
    {
        Assert.Equal(Version.Parse(expected), UpdateChecker.ParseVersion(text));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("latest")]
    [InlineData("v")]
    public void ParseVersion_RejectsNonVersions(string? text)
    {
        Assert.Null(UpdateChecker.ParseVersion(text));
    }

    [Theory]
    [InlineData("v1.3.2", "1.3.1", true)]
    [InlineData("v1.10.0", "1.9.0", true)]
    [InlineData("v1.3.1", "1.3.1", false)]
    [InlineData("v1.3", "1.3.0", false)]
    [InlineData("v1.3.0", "1.3.1", false)]
    [InlineData("nonsense", "1.3.1", false)]
    [InlineData("v1.3.2", null, false)]
    public void IsNewer_ComparesVersions(string? latest, string? current, bool expected)
    {
        Assert.Equal(expected, UpdateChecker.IsNewer(latest, current));
    }

    [Fact]
    public async Task Check_ReportsANewerRelease()
    {
        var checker = new UpdateChecker(new FakeReleaseSource { Next = () => Task.FromResult("v1.4.0") }, "1.3.1");

        Assert.Equal(UpdateStatus.Available, await checker.CheckAsync());
        Assert.Equal("1.4.0", checker.AvailableVersion);
    }

    [Fact]
    public async Task Check_ReportsUpToDate_ForTheSameOrAnOlderRelease()
    {
        var source = new FakeReleaseSource { Next = () => Task.FromResult("v1.3.1") };
        var checker = new UpdateChecker(source, "1.3.1");
        Assert.Equal(UpdateStatus.UpToDate, await checker.CheckAsync());
        Assert.Null(checker.AvailableVersion);

        source.Next = () => Task.FromResult("v1.2.0");
        Assert.Equal(UpdateStatus.UpToDate, await checker.CheckAsync());
    }

    [Fact]
    public async Task Check_Fails_WhenTheSourceThrowsOrTheTagIsNotAVersion()
    {
        var source = new FakeReleaseSource { Next = () => Task.FromException<string>(new HttpRequestException("offline")) };
        var checker = new UpdateChecker(source, "1.3.1");
        Assert.Equal(UpdateStatus.Failed, await checker.CheckAsync());
        Assert.IsType<HttpRequestException>(checker.LastError);

        source.Next = () => Task.FromResult("nightly");
        Assert.Equal(UpdateStatus.Failed, await checker.CheckAsync());
        Assert.Null(checker.LastError);
    }

    [Fact]
    public async Task Check_CanRunAgain_AfterASynchronousResult()
    {
        var source = new FakeReleaseSource { Next = () => Task.FromResult("v1.3.1") };
        var checker = new UpdateChecker(source, "1.3.1");
        await checker.CheckAsync();

        source.Next = () => Task.FromResult("v1.3.2");
        Assert.Equal(UpdateStatus.Available, await checker.CheckAsync());
        Assert.Equal(2, source.Calls);
    }

    [Fact]
    public async Task Check_WhileOneIsRunning_SharesIt()
    {
        var pending = new TaskCompletionSource<string>();
        var source = new FakeReleaseSource { Next = () => pending.Task };
        var checker = new UpdateChecker(source, "1.3.1");

        var first = checker.CheckAsync();
        var second = checker.CheckAsync();
        Assert.Equal(UpdateStatus.Checking, checker.Status);

        pending.SetResult("v1.3.2");
        Assert.Equal(UpdateStatus.Available, await first);
        Assert.Equal(UpdateStatus.Available, await second);
        Assert.Equal(1, source.Calls);
    }

    [Fact]
    public void Settings_CheckForUpdatesIsOnByDefault()
    {
        var settings = new AppSettings();
        Assert.True(settings.CheckForUpdates);
        Assert.Null(settings.LastAnnouncedUpdate);
    }

    [Fact]
    public async Task SettingsViewModel_ShowsWhatTheCheckFound()
    {
        var settings = new InMemorySettingsService();
        var checker = new UpdateChecker(new FakeReleaseSource { Next = () => Task.FromResult("v9.0.0") }, "1.3.1");
        var opened = 0;
        using var viewModel = new SettingsViewModel(settings, new FakeStartupService(), "1.3.1", "", checker, () => opened++);
        var changed = new List<string?>();
        viewModel.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

        Assert.False(viewModel.IsUpdateAvailable);
        await viewModel.CheckForUpdatesNowCommand.ExecuteAsync(null);

        Assert.True(viewModel.IsUpdateAvailable);
        Assert.Contains("9.0.0", viewModel.UpdateStatusText);
        Assert.Contains(nameof(SettingsViewModel.IsUpdateAvailable), changed);

        viewModel.OpenReleasePageCommand.Execute(null);
        Assert.Equal(1, opened);

        viewModel.CheckForUpdates = false;
        Assert.False(settings.Current.CheckForUpdates);
    }
}
