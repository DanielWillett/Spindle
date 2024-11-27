using System.Collections.Generic;

namespace Spindle.Localization;

/// <summary>
/// Controls how translations are saved and loaded from files.
/// </summary>
public interface ITranslationDataStore
{
    /// <summary>
    /// Write values to their data source for a specific language.
    /// </summary>
    /// <param name="language">The language to save.</param>
    void Save(TranslationCollection collection, IEnumerable<Translation> translations, Language language, bool writeAll);

    /// <summary>
    /// Read all languages from their data source.
    /// </summary>
    IReadOnlyDictionary<TranslationLanguageKey, string> Load(TranslationCollection collection);
}