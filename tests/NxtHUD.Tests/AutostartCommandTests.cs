using NxtHUD.Core.Services;
using Xunit;

namespace NxtHUD.Tests;

public class AutostartCommandTests
{
    private const string Installed = @"""C:\Users\Me\AppData\Local\Programs\NxtHUD\NxtHUD.exe"" --autostart";
    private const string Portable = @"""E:\Downloads\NxtHUD-1.4.0-portable-x64\NxtHUD.exe"" --autostart";

    [Theory]
    [InlineData(Installed, @"C:\Users\Me\AppData\Local\Programs\NxtHUD\NxtHUD.exe")]
    [InlineData(@"C:\Tools\NxtHUD.exe --autostart", @"C:\Tools\NxtHUD.exe")]
    [InlineData(@"C:\My Tools\NxtHUD.exe --autostart", @"C:\My Tools\NxtHUD.exe")]
    [InlineData(@"""C:\Program Files\dotnet\dotnet.exe"" ""C:\x\NxtHUD.dll"" --autostart", @"C:\Program Files\dotnet\dotnet.exe")]
    public void GetExecutablePath_ReadsThePath(string command, string expected)
    {
        Assert.Equal(expected, AutostartCommand.GetExecutablePath(command));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\"")]
    public void GetExecutablePath_IsNullForNoCommand(string? command)
    {
        Assert.Null(AutostartCommand.GetExecutablePath(command));
    }

    [Fact]
    public void ShouldRepoint_KeepsAnEntryForAnotherCopyThatStillExists()
    {
        // The installed app is registered and a portable or test copy is running.
        Assert.False(AutostartCommand.ShouldRepoint(Installed, Portable, _ => true));
    }

    [Fact]
    public void ShouldRepoint_MovesAnEntryWhoseCopyIsGone()
    {
        Assert.True(AutostartCommand.ShouldRepoint(Portable, Installed, _ => false));
    }

    [Fact]
    public void ShouldRepoint_LeavesTheSameCommandAlone()
    {
        Assert.False(AutostartCommand.ShouldRepoint(Installed, Installed.ToUpperInvariant(), _ => false));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldRepoint_IgnoresAMissingEntry(string? registered)
    {
        Assert.False(AutostartCommand.ShouldRepoint(registered, Installed, _ => false));
    }

    [Fact]
    public void ShouldRepoint_ChecksTheRegisteredExecutable()
    {
        string? asked = null;
        AutostartCommand.ShouldRepoint(Portable, Installed, path =>
        {
            asked = path;
            return true;
        });

        Assert.Equal(@"E:\Downloads\NxtHUD-1.4.0-portable-x64\NxtHUD.exe", asked);
    }
}
