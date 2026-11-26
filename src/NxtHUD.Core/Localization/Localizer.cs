using System.ComponentModel;
using System.Globalization;
using System.Resources;
using NxtHUD.Core.Settings;

namespace NxtHUD.Core.Localization;

/// <summary>
/// Looks up UI strings from <c>Resources/Strings.resx</c> (and its per-language siblings).
/// </summary>
/// <remarks>
/// The UI binds to the indexer (<c>{Binding [Some_Key], Source=...}</c>). When the language
/// changes, a single "Item[]" change notification makes every bound string refresh, so the
/// language can be switched without restarting.
/// </remarks>
public sealed class Localizer : INotifyPropertyChanged
{
    private const string ResourceBaseName = "NxtHUD.Core.Resources.Strings";

    /// <summary>The Windows display language, captured before anything can override it.</summary>
    private static readonly CultureInfo SystemCulture = CultureInfo.CurrentUICulture;

    private readonly ResourceManager _resources = new(ResourceBaseName, typeof(Localizer).Assembly);
    private CultureInfo _culture = SystemCulture;

    /// <summary>The instance shared by the whole app.</summary>
    public static Localizer Instance { get; } = new();

    /// <summary>
    /// Language codes offered in settings. The first entry follows Windows; the rest are
    /// culture names that have a Strings.&lt;culture&gt;.resx (English is the neutral resource).
    /// </summary>
    public static IReadOnlyList<string> SupportedLanguages { get; } = new[] { AppSettings.SystemLanguage, "en", "vi" };

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>The culture strings are currently resolved against.</summary>
    public CultureInfo Culture => _culture;

    /// <summary>Returns the string for <paramref name="key"/>; see <see cref="Get"/>.</summary>
    public string this[string key] => Get(key);

    /// <summary>
    /// Returns the string for <paramref name="key"/> in the current language, falling back
    /// to English. A missing key yields "[key]" so it is easy to spot in the UI.
    /// </summary>
    public string Get(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return string.Empty;
        }

        try
        {
            return _resources.GetString(key, _culture) ?? $"[{key}]";
        }
        catch (MissingManifestResourceException)
        {
            return $"[{key}]";
        }
    }

    /// <summary>Whether a string exists for <paramref name="key"/>.</summary>
    public bool Contains(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return false;
        }

        try
        {
            return _resources.GetString(key, _culture) is not null;
        }
        catch (MissingManifestResourceException)
        {
            return false;
        }
    }

    /// <summary>Returns the label for an enum value, stored under "Enum_&lt;Type&gt;_&lt;Value&gt;".</summary>
    public string GetEnumLabel(Enum value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Get(GetEnumKey(value));
    }

    public static string GetEnumKey(Enum value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return $"Enum_{value.GetType().Name}_{value}";
    }

    /// <summary>Returns the name shown for a language code in the language picker.</summary>
    public string GetLanguageDisplayName(string language)
    {
        if (IsSystemLanguage(language))
        {
            return Get("Language_System");
        }

        try
        {
            var culture = CultureInfo.GetCultureInfo(language);
            var name = culture.NativeName;
            return name.Length == 0 ? language : char.ToUpper(name[0], culture) + name[1..];
        }
        catch (CultureNotFoundException)
        {
            return language;
        }
    }

    /// <summary>
    /// Switches the language. Pass <see cref="AppSettings.SystemLanguage"/> (or null) to
    /// follow Windows. Unknown culture names fall back to the Windows language.
    /// </summary>
    public void SetLanguage(string? language)
    {
        var culture = ResolveCulture(language);
        if (culture.Equals(_culture))
        {
            return;
        }

        _culture = culture;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Culture)));
    }

    public static CultureInfo ResolveCulture(string? language)
    {
        if (IsSystemLanguage(language))
        {
            return SystemCulture;
        }

        try
        {
            return CultureInfo.GetCultureInfo(language!);
        }
        catch (CultureNotFoundException)
        {
            return SystemCulture;
        }
    }

    private static bool IsSystemLanguage(string? language)
    {
        return string.IsNullOrWhiteSpace(language)
            || string.Equals(language, AppSettings.SystemLanguage, StringComparison.OrdinalIgnoreCase);
    }
}
