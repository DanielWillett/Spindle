using DanielWillett.JavaPropertiesParser;
using DanielWillett.ReflectionTools;
using Spindle.Logging;
using Spindle.Plugins;
using Spindle.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Spindle.Localization.ValueFormatters;

/// <summary>
/// Translates enums using their .properties files in <c>Localization/[LANGUAGE]/<typeparamref name="TEnum"/></c>".
/// </summary>
/// <typeparam name="TEnum">The enum type to translate.</typeparam>
[Priority(int.MinValue + 1)]
public class PropertiesEnumValueConverter<TEnum> : IEnumStringConverter where TEnum : unmanaged, Enum
{
    private readonly ILanguageService _languageService;
    private readonly ILogger _logger;
    private readonly string? _filePathBegin;
    private readonly string? _fileName;

    private readonly bool _translatable;
    private readonly string? _description;
    private readonly string _displayName;
    private readonly bool _isPriority;

    private Dictionary<string, Dictionary<TEnum, string>>? _translations;
    private Dictionary<string, string>? _nameTranslations;

    public PropertiesEnumValueConverter(ILanguageService languageService, ILoggerFactory logger, SpindlePluginLoader pluginLoader)
    {
        _languageService = languageService;
        _logger = logger.CreateLogger("Uncreated.Warfare.Translations.ValueFormatters.PropertiesEnumValueFormatter");

        Assembly enumAssembly = typeof(TEnum).Assembly;
        string? folder = pluginLoader.GetAssemblyFolder(enumAssembly, SpecialPluginFolder.Localization);

        TranslatableAttribute? attribute = null;
        foreach (Attribute a in TypeDescriptor.GetAttributes(typeof(TEnum)))
        {
            if (a is TranslatableAttribute t)
                attribute = t;
        }

        _displayName = string.IsNullOrWhiteSpace(attribute?.DefaultValue)
            ? typeof(TEnum).FullName!
            : attribute.DefaultValue;

        if (attribute == null)
        {
            _translatable = false;
            return;
        }

        _description = attribute.Description;
        _isPriority = attribute.IsPrioritizedTranslation;

        _filePathBegin = folder ?? SpindlePaths.LocalizationDirectory;
        _fileName = FileUtility.SanitizeFileName(_displayName + ".properties");
    }

    public string GetName(Language language)
    {
        CheckTranslations();
        if (_nameTranslations!.TryGetValue(language.Name, out string name))
        {
            return name;
        }

        if (!language.IsDefault && _nameTranslations.TryGetValue(_languageService.GetDefaultLanguageName(), out name))
        {
            return name;
        }

        return _nameTranslations.Values.FirstOrDefault() ?? typeof(TEnum).Name;
    }
    
    public string GetValue(object value, Language language)
    {
        return GetValue((TEnum)value, language);
    }
    public string GetValue(TEnum value, Language language)
    {
        CheckTranslations();
        if (_translations!.TryGetValue(language.Name, out Dictionary<TEnum, string> table))
        {
            if (table.TryGetValue(value, out string translation))
            {
                return translation;
            }
        }

        if (!language.IsDefault && _translations.TryGetValue(_languageService.GetDefaultLanguageName(), out table))
        {
            return table.TryGetValue(value, out string translation) ? translation : value.ToString();
        }

        Dictionary<TEnum, string>? dict = _translations.Values.FirstOrDefault();
        return dict != null && dict.TryGetValue(value, out string t) ? t : value.ToString();
    }

    public string Format(IValueStringConvertService formatter, object value, in ValueFormatParameters parameters)
    {
        return GetValue((TEnum)value, parameters.Language);
    }

    public string Format(IValueStringConvertService formatter, TEnum value, in ValueFormatParameters parameters)
    {
        return GetValue(value, parameters.Language);
    }

    private void CheckTranslations()
    {
        if (_translations != null)
        {
            return;
        }

        lock (this)
        {
            if (_translations == null)
                ReadTranslations();
        }
    }

    private void ReadTranslations()
    {
        Dictionary<string, string> nameTranslations;
        Dictionary<string, Dictionary<TEnum, string>> tableTranslations;

        if (!_translatable)
        {
            nameTranslations = new Dictionary<string, string>(1, StringComparer.OrdinalIgnoreCase);
            tableTranslations = new Dictionary<string, Dictionary<TEnum, string>>(1, StringComparer.OrdinalIgnoreCase);

            string defaultLanguage = _languageService.GetDefaultLanguageName();

            nameTranslations.Add(defaultLanguage, _displayName);

            TEnum[] enums = (TEnum[])Enum.GetValues(typeof(TEnum));
            Dictionary<TEnum, string> englishVals = new Dictionary<TEnum, string>(enums.Length);
            foreach (TEnum value in enums)
            {
                englishVals.Add(value, value.ToString());
            }

            tableTranslations.Add(defaultLanguage, englishVals);
        }
        else
        {
            nameTranslations = new Dictionary<string, string>(4, StringComparer.OrdinalIgnoreCase);
            tableTranslations = new Dictionary<string, Dictionary<TEnum, string>>(4, StringComparer.OrdinalIgnoreCase);

            string[] folders = Directory.GetDirectories(_filePathBegin!, "*", SearchOption.TopDirectoryOnly);

            for (int i = 0; i < folders.Length; ++i)
            {
                string dir = folders[i];
                string dirName = Path.GetFileName(dir);

                if (_languageService.Languages.FirstOrDefault(x => x.GetFileName().Equals(dirName, StringComparison.OrdinalIgnoreCase) || x.Name.Equals(dirName, StringComparison.OrdinalIgnoreCase) ) is not { } language)
                    continue;

                if (!dirName.Equals(language.GetFileName(), StringComparison.Ordinal))
                {
                    try
                    {
                        Directory.Move(dirName, language.GetFileName());
                    }
                    catch
                    {
                        // ignored
                    }
                }

                ReadTranslations(language, out Dictionary<TEnum, string>? table, out string? name);
                if (table == null && name == null)
                    continue;

                if (table != null)
                    tableTranslations.Add(dirName, table);
                if (name != null)
                    nameTranslations.Add(dirName, name);
            }
        }

        _nameTranslations = nameTranslations;
        _translations = tableTranslations;
    }

    private void ReadTranslations(Language language, out Dictionary<TEnum, string>? table, out string? name)
    {
        bool isDefault = language.IsDefault;
        string path = Path.Combine(_filePathBegin!, language.GetFileName(), _fileName!);

        Dictionary<TEnum, string>? translationTable = null;
        string? nameTranslation = null;
        if (File.Exists(path))
        {
            using PropertiesReader reader = new PropertiesReader(path);
            while (reader.TryReadPair(out string key, out string value))
            {
                if (key.Equals("%NAME%", StringComparison.OrdinalIgnoreCase))
                {
                    nameTranslation = value;
                }
                else if (Enum.TryParse(key, false, out TEnum enumKeyValue) || Enum.TryParse(key, true, out enumKeyValue))
                {
                    if (!(translationTable ??= new Dictionary<TEnum, string>(16)).TryAdd(enumKeyValue, value))
                    {
                        _logger.LogWarning("Duplicate enum key for type {0}: \"{1}\".", typeof(TEnum), enumKeyValue.ToString());
                    }
                }
                else if (key.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    nameTranslation = value;
                }
                else
                {
                    _logger.LogWarning("Unknown enum key for type {0}: \"{1}\".", typeof(TEnum), value);
                }
            }

            if (nameTranslation == null)
            {
                _logger.LogWarning("Missing %NAME% translation for enum type {0}.", typeof(TEnum));
            }
        }

        if (isDefault)
        {
            WriteTranslations(path, nameTranslation, translationTable, false, true);
        }

        table = translationTable;
        name = nameTranslation;
    }

    private void WriteTranslations(string path, string? nameTranslation, IReadOnlyDictionary<TEnum, string>? translationTable, bool isExport, bool writeAll)
    {
        using PropertiesWriter writer = new PropertiesWriter(path);

        Type enumType = typeof(TEnum);

        writer.WriteComment(enumType.FullName!, PropertiesCommentType.Exclamation);
        writer.WriteComment($" in {enumType.Assembly.GetName().Name}.dll", PropertiesCommentType.Exclamation);
        if (_description != null)
        {
            writer.WriteLine();
            writer.WriteComment(_description);
        }

        writer.WriteLine();

        if (writeAll || nameTranslation != null)
        {
            writer.WriteComment(Properties.Resources.TranslationsEnumNameDescription, PropertiesCommentType.Hashtag);
            writer.WriteKeyValue("%NAME%", nameTranslation ?? enumType.Name);
            writer.WriteLine();
        }

        if (!writeAll && translationTable == null)
            return;

        bool isFirst = false;
        foreach (FieldInfo field in enumType.GetFields(BindingFlags.Static | BindingFlags.Public))
        {
            if (field.FieldType != typeof(TEnum))
                continue;

            TEnum val = (TEnum)field.GetValue(null);

            string? value = null;
            if (!writeAll && (translationTable == null || !translationTable.TryGetValue(val, out value)))
                continue;

            if (field.TryGetAttributeSafe(out TranslatableAttribute attribute))
            {
                if (!(isExport || writeAll) && value == null && !attribute.IsPrioritizedTranslation)
                    continue;

                value ??= attribute.DefaultValue;

                if (!string.IsNullOrWhiteSpace(attribute.Description))
                {
                    if (!isFirst)
                        writer.WriteLine();
                    else
                        isFirst = false;

                    writer.WriteComment(attribute.Description, PropertiesCommentType.Hashtag);
                }
            }

            writer.WriteKey(val.ToString());
            writer.WriteValue(value ?? val.ToString());
        }
    }
}
