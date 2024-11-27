using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Spindle.Localization;

/// <summary>
/// Base class for all translations. Also represents a translation with no arguments.
/// </summary>
public class Translation : IDisposable
{
    [ThreadStatic]
    private static List<string>? _pluralBuffer;
#nullable disable

    private readonly string _defaultText;
    public TranslationValue Original { get; private set; }
    public string Key { get; private set; }
    public TranslationData Data { get; private set; }
    public TranslationCollection Collection { get; private set; }
    public SharedTranslationDictionary Table { get; private set; }
    public bool IsInitialized { get; private set; }
    public TranslationOptions Options { get; }
    public virtual int ArgumentCount => 0;
    public ITranslationService TranslationService { get; private set; }
    public ILanguageService LanguageService { get; private set; }
    public IValueStringConvertService ValueStringConvertService { get; private set; }
#nullable restore

    public Translation(string defaultValue, TranslationOptions options = default)
    {
        _defaultText = defaultValue;
        Key = string.Empty;
        Options = options;
    }

    /// <summary>
    /// Get the value-set for a given language from the table. Defaults to the default language if <see langword="null"/>.
    /// </summary>
    public TranslationValue GetValueForLanguage(Language? language)
    {
        AssertInitialized();

        string langCode = language?.Name ?? LanguageService.GetDefaultLanguage().Name;

        if (Table.TryGetValue(langCode, out TranslationValue value))
            return value;

        if (language is not { FallbackTranslationLanguageName: { } fallbackLangName }
            || !Table.TryGetValue(fallbackLangName, out value))
        {
            return Original;
        }
        
        return value;
    }

    internal virtual void Initialize(string key,
        IDictionary<TranslationLanguageKey, TranslationValue> underlyingTable,
        TranslationCollection collection,
        ILanguageService languageService,
        ITranslationService translationService,
        IValueStringConvertService valueStringConvertService,
        TranslationData data)
    {
        Key = key;
        Data = data;

        LanguageService = languageService;
        TranslationService = translationService;
        ValueStringConvertService = valueStringConvertService;

        Language defaultLanguage = languageService.GetDefaultLanguage();
        Original = new TranslationValue(defaultLanguage, _defaultText, this);
        Collection = collection;
        Table = new SharedTranslationDictionary(this, underlyingTable);
        IsInitialized = true;
        Table[defaultLanguage.Name] = Original;
    }

    /// <summary>
    /// Returns the format of a given argument index.
    /// </summary>
    public virtual ArgumentFormat GetArgumentFormat(int index)
    {
        throw new ArgumentOutOfRangeException(nameof(index));
    }

    /// <summary>
    /// Overridden in generic translation classes to cast the arguments without reflection.
    /// </summary>
    /// <remarks><paramref name="formattingParameters"/> have already been verified to be the correct type by this point.</remarks>
    protected virtual string UnsafeTranslateIntl(in TranslationArguments arguments, object?[] formattingParameters)
    {
        return arguments.ValueSet.GetValueString(arguments.UseIMGUI, arguments.UseUncoloredTranslation, (arguments.Options & TranslationOptions.ForTerminal) != 0);
    }

    /// <summary>
    /// Checks if the translation has a value specifically for <paramref name="language"/>.
    /// </summary>
    public bool HasLanguage(Language? language)
    {
        string langCode = language?.Name ?? LanguageService.GetDefaultLanguage().Name;

        return Table.ContainsKey(langCode);
    }

    protected internal void AssertInitialized()
    {
        if (!IsInitialized)
            throw new InvalidOperationException("This translation has not been initialized.");
    }

    internal void UpdateValue(string value, Language language)
    {
        AssertInitialized();
        Table.AddOrUpdate(new TranslationValue(language, value, this));
    }

    /// <summary>
    /// Applies all pluralizers using the given language.
    /// </summary>
    protected ReadOnlySpan<char> ApplyPluralizers(scoped in TranslationArguments args, ArgumentSpan[] pluralizers, int argumentOffset, int argCt, Func<int, object?> accessor)
    {
        Span<int> indices = stackalloc int[pluralizers.Length];
        int numToReplace = 0;
        int firstToReplace = -1;
        for (int i = 0; i < pluralizers.Length; ++i)
        {
            ref ArgumentSpan argSpan = ref pluralizers[i];
            if (argSpan.Argument >= argCt)
            {
                indices[i] = -1;
                continue;
            }

            object? argValue = accessor(argSpan.Argument);

            bool isOne = argValue is IConvertible conv && !TranslationPluralizations.IsOne(conv);
            if (isOne ^ argSpan.Inverted)
            {
                if (firstToReplace == -1)
                    firstToReplace = i;
                ++numToReplace;
                continue;
            }

            indices[i] = -1;
        }

        if (numToReplace == 0)
            return args.PreformattedValue;

        if (numToReplace == 1)
        {
            ref ArgumentSpan span = ref pluralizers[firstToReplace];
            ReadOnlySpan<char> word = args.PreformattedValue.Slice(span.StartIndex + argumentOffset, span.Length);
            string pluralWord = TranslationPluralizations.PluralizeWords(new string(word), args.Language, args.Culture);
            return TranslationArgumentModifiers.ReplaceModifiers(args.PreformattedValue, pluralWord, indices, pluralizers, argumentOffset);
        }

        List<string> pluralBuffer = _pluralBuffer ??= new List<string>(numToReplace);

        if (pluralBuffer.Capacity < numToReplace)
            pluralBuffer.Capacity = numToReplace;

        try
        {
            int spanCt = pluralizers.Length;
            int totalSize = 0;
            for (int i = 0; i < spanCt; ++i)
            {
                ref ArgumentSpan span = ref pluralizers[i];
                ref int index = ref indices[i];
                if (index < 0)
                    continue;

                ReadOnlySpan<char> word = args.PreformattedValue.Slice(span.StartIndex + argumentOffset, span.Length);
                string pluralWord = TranslationPluralizations.PluralizeWords(new string(word), args.Language, args.Culture);
                index = totalSize;
                totalSize += pluralWord.Length;
                pluralBuffer.Add(pluralWord);
            }

            Span<char> pluralWordsBuffer = stackalloc char[totalSize];
            int bufferIndex = 0;
            for (int i = 0; i < pluralBuffer.Count; ++i)
            {
                string pluralWord = pluralBuffer[i];
                pluralWord.AsSpan().CopyTo(pluralWordsBuffer[bufferIndex..]);
                bufferIndex += pluralWord.Length;
            }

            return TranslationArgumentModifiers.ReplaceModifiers(args.PreformattedValue, pluralWordsBuffer, indices, pluralizers, argumentOffset);
        }
        finally
        {
            pluralBuffer.Clear();
        }
    }

    /// <summary>
    /// Translate using an object[] instead of type-safe generics.
    /// </summary>
    /// <exception cref="ArgumentException">One of the values wasn't the right type.</exception>
    public string TranslateUnsafe(in TranslationArguments arguments, object?[] formatting)
    {
        Type[] genericArguments = GetType().GetGenericArguments();
        if (genericArguments.Length == 0)
        {
            return arguments.ValueSet.GetValueString(arguments.UseIMGUI, arguments.UseUncoloredTranslation, (arguments.Options & TranslationOptions.ForTerminal) != 0);
        }

        // resize formatting to correct length
        if (formatting == null)
            formatting = new object[genericArguments.Length];
        else if (genericArguments.Length > formatting.Length)
            Array.Resize(ref formatting, genericArguments.Length);
        
        // convert arguments
        for (int i = 0; i < genericArguments.Length; ++i)
        {
            object? v = formatting[i];
            Type expectedType = genericArguments[i];
            if (v == null)
            {
                if (expectedType.IsValueType)
                {
                    throw new ArgumentException(string.Format(Properties.Resources.TranslateUnsafe_FormattingArgumentMismatchValueTypeNull, i), $"{nameof(formatting)}[{i}]");
                }

                continue;
            }

            Type suppliedType = v.GetType();
            if (expectedType.IsAssignableFrom(suppliedType))
                continue;

            if (expectedType == typeof(string))
            {
                ArgumentFormat argFmt = GetArgumentFormat(i);
                ValueFormatParameters parameters = new ValueFormatParameters(-1, in arguments, in argFmt, null, 0);
                formatting[i] = ValueStringConvertService.Format(genericArguments[i], in parameters);
            }

            try
            {
                formatting[i] = Convert.ChangeType(v, expectedType);
            }
            catch (Exception ex)
            {
                try
                {
                    TypeConverter fromSupplied = TypeDescriptor.GetConverter(suppliedType);
                    if (fromSupplied.CanConvertTo(expectedType))
                    {
                        formatting[i] = fromSupplied.ConvertTo(v, expectedType);
                        continue;
                    }
                }
                catch (NotSupportedException) { }

                try
                {
                    TypeConverter toSupplied = TypeDescriptor.GetConverter(expectedType);
                    if (toSupplied.CanConvertFrom(suppliedType))
                    {
                        formatting[i] = toSupplied.ConvertFrom(v);
                        continue;
                    }
                }
                catch (NotSupportedException) { }

                throw new ArgumentException(string.Format(Properties.Resources.TranslateUnsafe_FormattingArgumentMismatch, i), $"{nameof(formatting)}[{i}]", ex);
            }
        }

        return UnsafeTranslateIntl(in arguments, formatting);
    }

    void IDisposable.Dispose()
    {
        Table.Clear();
    }
}