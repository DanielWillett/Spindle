using System;
using System.Collections.Generic;

namespace Spindle.Localization;

public readonly struct TranslationLanguageKey : IEquatable<TranslationLanguageKey>
{
    /// <summary>
    /// The language code for this row.
    /// </summary>
    public readonly string LanguageName;

    /// <summary>
    /// Unique id of the translation for this row.
    /// </summary>
    public readonly string TranslationKey;
    public static IEqualityComparer<TranslationLanguageKey> EqualityComparer { get; } = new TranslationLanguageKeyComparer();
    public TranslationLanguageKey(string languageName, string translationKey)
    {
        LanguageName = languageName;
        TranslationKey = translationKey;
    }
    public bool Equals(TranslationLanguageKey key)
    {
        return string.Equals(key.LanguageName, LanguageName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(key.TranslationKey, TranslationKey, StringComparison.Ordinal);
    }
    public override bool Equals(object? obj)
    {
        return obj is TranslationLanguageKey key && Equals(key);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(
            LanguageName.GetHashCode(StringComparison.OrdinalIgnoreCase),
            TranslationKey.GetHashCode(StringComparison.Ordinal)
        );
    }
    private class TranslationLanguageKeyComparer : IEqualityComparer<TranslationLanguageKey>
    {
        public bool Equals(TranslationLanguageKey x, TranslationLanguageKey y) => x.Equals(y);
        public int GetHashCode(TranslationLanguageKey obj) => obj.GetHashCode();
    }
}