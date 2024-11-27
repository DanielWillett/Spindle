using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Spindle.Localization;

/// <summary>
/// Manages storing <see cref="Language"/> objects.
/// </summary>
/// <remarks>Can be <see cref="IDisposable"/>.</remarks>
public interface ILanguageService
{
    /// <summary>
    /// List of all languages currently registered.
    /// </summary>
    IReadOnlyList<Language> Languages { get; }

    /// <summary>
    /// Download/read the language list.
    /// </summary>
    UniTask InitializeAsync();

    /// <summary>
    /// Re-download or re-read the language list.
    /// </summary>
    UniTask ReloadAsync();
}

public static class LanguageServiceExtensions
{
    public static string GetDefaultLanguageName(this ILanguageService languageService)
    {
        return SpindleLauncher.Configuration["DefaultLanguage"] ?? throw new FormatException("DefaultLanguage not configured or no language matching it's value.");
    }

    public static Language GetDefaultLanguage(this ILanguageService languageService)
    {
        string? defaultLanguageName = SpindleLauncher.Configuration["DefaultLanguage"];

        Language? defaultLanguage = languageService.Languages.FirstOrDefault(x => x.Name.Equals(defaultLanguageName, StringComparison.OrdinalIgnoreCase));

        return defaultLanguage ?? throw new FormatException("DefaultLanguage not configured or no language matching it's value.");
    }
    
    public static CultureInfo GetDefaultCulture(this ILanguageService languageService)
    {
        string? defaultCultureName = SpindleLauncher.Configuration["DefaultCulture"];

        if (defaultCultureName == null)
        {
            throw new FormatException("DefaultCulture not configured or no language matching it's value.");
        }
        
        try
        {
            return new CultureInfo(defaultCultureName);
        }
        catch (CultureNotFoundException)
        {
            throw new FormatException("DefaultCulture is not a valid culture name.");
        }
    }

    public static Language? FindLanguage(this ILanguageService languageService, string searchTerm, bool exactOnly = false)
    {
        IReadOnlyList<Language> languages = languageService.Languages;

        Language? found = languages.FirstOrDefault(lang => lang.Name.Equals(searchTerm, StringComparison.OrdinalIgnoreCase));
        if (found != null)
            return found;
        
        found = languages.FirstOrDefault(lang => lang.DisplayName.Equals(searchTerm, StringComparison.OrdinalIgnoreCase));
        if (found != null)
            return found;
        
        found = languages.FirstOrDefault(lang => lang.Aliases.Any(alias => alias.Alias.Equals(searchTerm, StringComparison.OrdinalIgnoreCase)));
        if (found != null)
            return found;

        found = languages.FirstOrDefault(lang => lang.DisplayName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0);
        if (found != null)
            return found;
        
        found = languages.FirstOrDefault(lang => lang.Aliases.Any(alias => alias.Alias.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0));

        return found;
    }
}