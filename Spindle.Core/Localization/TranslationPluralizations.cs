using PluralizationService;
using PluralizationService.English;
using System;
using System.Collections.Concurrent;
using System.Globalization;

namespace Spindle.Localization;

public interface ITranslationPluralizationHandler
{
    string Pluralize(string word, Language language, CultureInfo culture);
}

public static class TranslationPluralizations
{
    private static readonly ConcurrentDictionary<string, ITranslationPluralizationHandler> Rules = new ConcurrentDictionary<string, ITranslationPluralizationHandler>(StringComparer.Ordinal);

    static TranslationPluralizations()
    {
        Rules.TryAdd("en", new DefaultEnglishPluralizer());
    }

    /// <summary>
    /// Add a pluralization handler for another language or replace the existing one.
    /// </summary>
    /// <remarks>This is intended to be used by plugins to add pluralization handling for languages other than English.</remarks>
    public static void AddRule(Language language, ITranslationPluralizationHandler handler)
    {
        ITranslationPluralizationHandler? old = null;
        Rules.AddOrUpdate(language.Name, _ => { old = null; return handler; }, (_, oldValue) => { old = oldValue; return handler; });
        if (old is IDisposable disp)
            disp.Dispose();
    }

    /// <summary>
    /// Convert a word or words to their plural form. By default, only English is supported but plugins can add support for other languages.
    /// </summary>
    /// <remarks>
    /// In English, singular words can be prefixed with 'a [word]' or 'an [word]' and the 'a' or 'an' will be removed when pluralizing.
    /// Other languages should implement similar functionality if applicable.
    ///
    /// If any pluralization returns an empty string for a word, a space should be removed if there's a space on both sides of the output word.  
    /// </remarks>
    public static string PluralizeWords(string words, Language language, CultureInfo culture)
    {
        return Rules.TryGetValue(language.Name, out ITranslationPluralizationHandler handler)
            ? handler.Pluralize(words, language, culture)
            : words;
    }

    /// <summary>
    /// Check if an unknown type is equal to 1.
    /// </summary>
    public static bool IsOne(object? obj) => obj is IConvertible conv && IsOne(conv);

    /// <summary>
    /// Check if an unknown convertible type is equal to 1.
    /// </summary>
    public static bool IsOne(IConvertible conv)
    {
        TypeCode tc = conv.GetTypeCode();
        return tc switch
        {
            TypeCode.Boolean => (bool)conv,
            TypeCode.Char => (char)conv == 1,
            TypeCode.SByte => (sbyte)conv == 1,
            TypeCode.Byte => (byte)conv == 1,
            TypeCode.Int16 => (short)conv == 1,
            TypeCode.UInt16 => (ushort)conv == 1,
            TypeCode.Int32 => (int)conv == 1,
            TypeCode.UInt32 => (uint)conv == 1,
            TypeCode.Int64 => (long)conv == 1,
            TypeCode.UInt64 => (ulong)conv == 1,
            TypeCode.Single => Math.Abs((float)conv - 1) <= float.Epsilon,
            TypeCode.Double => Math.Abs((double)conv - 1) <= double.Epsilon,
            TypeCode.Decimal => ((decimal)conv).Equals(1m),
            TypeCode.DateTime => ((DateTime)conv).Ticks == 1,
            TypeCode.String => ((string)conv).Equals("1", StringComparison.InvariantCultureIgnoreCase) ||
                               ((string)conv).Equals("one", StringComparison.InvariantCultureIgnoreCase),
            _ => false
        };
    }
    private class DefaultEnglishPluralizer : ITranslationPluralizationHandler
    {
        private readonly IPluralizationApi _api;
        private readonly CultureInfo _culture;

        public DefaultEnglishPluralizer()
        {
            PluralizationApiBuilder bldr = new PluralizationApiBuilder();
            bldr.AddEnglishProvider();
            _api = bldr.Build();

            _culture = new CultureInfo("en-US");

            _api.AddWord("it", "they", _culture);
            _api.AddWord("is", "are", _culture);
            _api.AddWord("did", "do", _culture);
        }

        public string Pluralize(string word, Language language, CultureInfo culture)
        {
            if (word.Length > 2 && word.StartsWith("a ", StringComparison.InvariantCultureIgnoreCase))
            {
                word = word[2..];
            }
            else if (word.Length > 3 && word.StartsWith("an ", StringComparison.InvariantCultureIgnoreCase))
            {
                word = word[3..];
            }
            else if (word.Length == 1 && word[0] is 'a' or 'A' || word.Equals("an", StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Empty;
            }
            else if (word.Equals("an ", StringComparison.InvariantCultureIgnoreCase)
                     || word.Equals(" an", StringComparison.InvariantCultureIgnoreCase)
                     || word.Equals("a ", StringComparison.InvariantCultureIgnoreCase)
                     || word.Equals(" a", StringComparison.InvariantCultureIgnoreCase))
            {
                return string.Empty;
            }

            if (!word.Contains(' '))
                return _api.Pluralize(word, _culture);

            string[] words = word.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; ++i)
            {
                words[i] = _api.Pluralize(word, _culture);
            }

            return string.Join(' ', words);

        }
    }
}