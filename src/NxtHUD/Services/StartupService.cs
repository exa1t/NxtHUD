using System.IO;
using Microsoft.Win32;
using NxtHUD.Core.Services;

namespace NxtHUD.Services;

/// <summary>
/// Start-with-Windows through the per-user Run key
/// (HKCU\Software\Microsoft\Windows\CurrentVersion\Run). No administrator rights needed.
/// </summary>
internal sealed class StartupService : IStartupService
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

    /// <summary>Where Task Manager / Settings record that the user switched a startup entry off.</summary>
    private const string ApprovedKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";

    private readonly string _valueName;
    private readonly string _command;

    /// <param name="valueName">Name of the Run entry (the app name).</param>
    /// <param name="command">Full command line Windows should run at sign-in.</param>
    public StartupService(string valueName, string command)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(valueName);
        ArgumentException.ThrowIfNullOrWhiteSpace(command);

        _valueName = valueName;
        _command = command;
    }

    public bool IsEnabled()
    {
        using var runKey = Registry.CurrentUser.OpenSubKey(RunKeyPath);
        if (runKey?.GetValue(_valueName) is not string)
        {
            return false;
        }

        // Windows keeps the Run entry when the user disables it in Task Manager and flags it here
        // instead: the first byte is even when enabled and odd when disabled.
        using var approvedKey = Registry.CurrentUser.OpenSubKey(ApprovedKeyPath);
        if (approvedKey?.GetValue(_valueName) is byte[] { Length: > 0 } state && (state[0] & 1) != 0)
        {
            return false;
        }

        return true;
    }

    public void SetEnabled(bool enabled)
    {
        using var runKey = Registry.CurrentUser.CreateSubKey(RunKeyPath, writable: true);

        if (enabled)
        {
            runKey.SetValue(_valueName, _command, RegistryValueKind.String);

            // Clear a "disabled in Task Manager" flag, otherwise the entry would stay switched off.
            using var approvedKey = Registry.CurrentUser.OpenSubKey(ApprovedKeyPath, writable: true);
            approvedKey?.DeleteValue(_valueName, throwOnMissingValue: false);
        }
        else
        {
            runKey.DeleteValue(_valueName, throwOnMissingValue: false);
        }
    }

    /// <summary>
    /// If the app is registered but the registered copy no longer exists (it was moved), points
    /// the Run entry at the executable that is running now. Another copy that still exists keeps
    /// the entry; see <see cref="AutostartCommand.ShouldRepoint"/>.
    /// </summary>
    public void RefreshCommandIfRegistered()
    {
        using var runKey = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
        if (runKey?.GetValue(_valueName) is string existing
            && AutostartCommand.ShouldRepoint(existing, _command, File.Exists))
        {
            Log.Info($"Start-with-Windows pointed at a missing copy ({existing}); pointing it at this one.");
            runKey.SetValue(_valueName, _command, RegistryValueKind.String);
        }
    }
}
