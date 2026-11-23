namespace NxtHUD.Core.Services;

/// <summary>Reads and compares the command lines kept in the start-with-Windows Run entry.</summary>
public static class AutostartCommand
{
    /// <summary>
    /// The executable a command line runs: the quoted part of <c>"C:\x\NxtHUD.exe" --autostart</c>,
    /// or everything up to the first ".exe" when it is not quoted. Null when there is none.
    /// </summary>
    public static string? GetExecutablePath(string? command)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            return null;
        }

        var text = command.Trim();
        if (text.StartsWith('"'))
        {
            var close = text.IndexOf('"', 1);
            return close > 1 ? text[1..close] : null;
        }

        var exe = text.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
        return exe > 0 ? text[..(exe + 4)] : text.Split(' ')[0];
    }

    /// <summary>
    /// Whether the Run entry should be pointed at the copy that is running now.
    /// </summary>
    /// <remarks>
    /// Only when the registered copy is gone (the app was moved, or an old install was removed).
    /// A different copy that still exists, such as the installed app while a portable or test
    /// copy runs, keeps the entry: start-with-Windows belongs to that copy, and quietly moving
    /// it to whichever copy ran last would leave the user starting a download folder at sign-in.
    /// </remarks>
    public static bool ShouldRepoint(string? registered, string current, Func<string, bool> fileExists)
    {
        ArgumentNullException.ThrowIfNull(fileExists);

        if (string.IsNullOrWhiteSpace(registered))
        {
            return false;
        }

        if (string.Equals(registered.Trim(), current.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var path = GetExecutablePath(registered);
        return path is null || !fileExists(path);
    }
}
