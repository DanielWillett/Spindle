using DanielWillett.ReflectionTools;
using Spindle.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Spindle.Localization;

public abstract class TranslationCollection
{
    private ILogger? _logger;

#nullable disable
    private ILoggerFactory _loggerFactory;
    private bool _isInitialized;
    private Dictionary<string, Translation> _translations;
    private Dictionary<TranslationLanguageKey, TranslationValue> _valueTable;
    private ITranslationDataStore _storage;
    private ILanguageService _languageService;
    private IValueStringConvertService _converter;

    /// <summary>
    /// Display name of the translation collection.
    /// </summary>
    /// <remarks>Usually this is also the file name and can contain forward slashes to create subfolders.</remarks>
    public abstract string Name { get; }

    public ITranslationService TranslationService { get; private set; }
    public IReadOnlyDictionary<string, Translation> Translations { get; private set; }
#nullable restore

    /// <summary>
    /// Initialize if it hasn't already.
    /// </summary>
    /// <remarks>I have it set up like this to make thread-safety easier for service injections.</remarks>
    internal bool TryInitialize(ITranslationService translationService, ILanguageService languageService, ITranslationDataStore translationStorage, IValueStringConvertService converter, ILoggerFactory loggerFactory)
    {
        if (_isInitialized)
            return false;

        lock (this)
        {
            if (_isInitialized)
                return false;

            Initialize(translationService, languageService, translationStorage, converter, loggerFactory);
            _isInitialized = true;
            return true;
        }
    }

    protected ILogger GetLogger()
    {
        return _logger ??= _loggerFactory.CreateLogger(GetType());
    }

    protected virtual void Initialize(ITranslationService translationService, ILanguageService languageService, ITranslationDataStore translationStorage, IValueStringConvertService converter, ILoggerFactory loggerFactory)
    {
        _translations = new Dictionary<string, Translation>(32);
        _valueTable = new Dictionary<TranslationLanguageKey, TranslationValue>(64, TranslationLanguageKey.EqualityComparer);

        _languageService = languageService;
        TranslationService = translationService;
        _storage = translationStorage;
        _converter = converter;
        _loggerFactory = loggerFactory;

        Translations = new ReadOnlyDictionary<string, Translation>(_translations);

        // get logger for parent collection type

        FindTranslationsInMembers();
        Reload();

        // save default language
        try
        {
            _storage.Save(this, Translations.Values, null!, true);
        }
        catch (Exception ex)
        {
            GetLogger().LogWarning(ex, "Failed to re-save default translations for {0} using storage {1}: {2}.", GetType(), _storage.GetType(), ToString());
        }
    }

    public void Reload()
    {
        IReadOnlyDictionary<TranslationLanguageKey, string> translationData;
        try
        {
            translationData = _storage.Load(this);
        }
        catch (Exception ex)
        {
            GetLogger().LogError(ex, "Failed to load translations for {0} using storage {1}: {2}.", GetType(), _storage.GetType(), ToString());
            translationData = new Dictionary<TranslationLanguageKey, string>(0);
        }

        foreach (KeyValuePair<TranslationLanguageKey, string> translation in translationData)
        {
            if (!_translations.TryGetValue(translation.Key.TranslationKey, out Translation translationMember))
            {
                GetLogger().LogWarning("Unknown translation in collection {0}: {1}.", GetType(), ToString());
                continue;
            }

            Language? language = _languageService.FindLanguage(translation.Key.LanguageName, exactOnly: true);

            if (language is null)
            {
                GetLogger().LogWarning("Unknown language {0} in collection {1}: {2}.", translation.Key.LanguageName, GetType(), ToString());
                continue;
            }

            translationMember.UpdateValue(translation.Value, language);
        }
    }

    private void FindTranslationsInMembers()
    {
        Type type = GetType();
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // each field and property in the collection's type.
        foreach (MemberInfo member in fields
                     .Concat<MemberInfo>(properties)
                     .Where(member => typeof(Translation).IsAssignableFrom(member.GetMemberType())))
        {
            if (member.IsIgnored())
                continue;

            Translation? translation = member switch
            {
                FieldInfo field => (Translation?)field.GetValue(this),
                PropertyInfo property => (Translation?)property.GetValue(this),
                _ => null
            };

            TranslationDataAttribute? data = member.GetAttributeSafe<TranslationDataAttribute>();
            string key = data?.Key ?? member.Name;

            if (translation == null)
            {
                _logger.LogError($"Translation '{key}' in collection '{type.Name}' is null.");
                continue;
            }

            translation.Initialize(key,
                // storing this as readonly so it's obvious it shouldn't be modified outside the translation class
                _valueTable,
                this,
                _languageService,
                TranslationService,
                _converter,
                new TranslationData(
                    data?.Description,
                    data?.Parameters,
                    data == null || data.IsPriorityTranslation));

            _translations.Add(translation.Key, translation);
        }
    }

    public override string ToString()
    {
        string name = Path.DirectorySeparatorChar == '/' ? Name.Replace('\\', '/') : Name.Replace('/', '\\');

        return Path.GetFileName(name);
    }
}