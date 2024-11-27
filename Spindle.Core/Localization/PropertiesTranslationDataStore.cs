using DanielWillett.JavaPropertiesParser;
using DanielWillett.ReflectionTools;
using DanielWillett.ReflectionTools.Formatting;
using Spindle.Plugins;
using Spindle.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Spindle.Localization;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public class PropertiesTranslationDataStore : ITranslationDataStore
{
    private readonly SpindlePluginLoader _pluginLoader;
    private readonly ILanguageService _languageService;
    private readonly ILogger<PropertiesTranslationDataStore> _logger;

    public PropertiesTranslationDataStore(SpindlePluginLoader pluginLoader, ILanguageService languageService, ILogger<PropertiesTranslationDataStore> logger)
    {
        _pluginLoader = pluginLoader;
        _languageService = languageService;
        _logger = logger;
    }

    private void GetPaths(TranslationCollection collection, out string folderPath, out string filePath)
    {
        folderPath = _pluginLoader.GetAssemblyFolder(collection.GetType().Assembly, SpecialPluginFolder.Localization)
                            ?? SpindlePaths.LocalizationDirectory;

        filePath = Path.DirectorySeparatorChar == '/' ? collection.Name.Replace('\\', '/') : collection.Name.Replace('/', '\\');
        if (!filePath.EndsWith(".properties", StringComparison.OrdinalIgnoreCase))
            filePath += ".properties";
    }

    public IReadOnlyDictionary<TranslationLanguageKey, string> Load(TranslationCollection collection)
    {
        GetPaths(collection, out string folderPath, out string filePath);

        // find and parse files
        string[] languageDirs = Directory.GetDirectories(folderPath, "*", SearchOption.TopDirectoryOnly);

        Dictionary<TranslationLanguageKey, string> translationDict = new Dictionary<TranslationLanguageKey, string>(32);

        foreach (string languageDirectory in languageDirs)
        {
            string dirName = Path.GetFileName(languageDirectory)!;

            if (_languageService.Languages.FirstOrDefault(x => x.GetFileName().Equals(dirName, StringComparison.OrdinalIgnoreCase)
                                                               || x.Name.Equals(dirName, StringComparison.OrdinalIgnoreCase)) is not { } language)
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

            string languageFileName = Path.Combine(folderPath, language.GetFileName(), filePath);

            if (!File.Exists(languageFileName))
                continue;

            using PropertiesReader reader = new PropertiesReader(languageFileName);

            while (reader.TryReadPair(out string key, out string value))
            {
                TranslationLanguageKey dictKey = new TranslationLanguageKey(dirName, key);
                if (translationDict.ContainsKey(dictKey))
                {
                    _logger.LogWarning("Duplicate translation key {0} in file {1}.", key, FileUtility.GetRelativeToSpindle(languageFileName));
                    continue;
                }

                if (!collection.Translations.TryGetValue(key, out Translation translation))
                {
                    _logger.LogWarning("Unknown translation key {0} in file {1}.", key, FileUtility.GetRelativeToSpindle(languageFileName));
                    continue;
                }

                int maxArg = TranslationFormattingUtility.GetMaximumFormattingArgument(value);
                if (maxArg >= translation.ArgumentCount)
                {
                    _logger.LogWarning("Translation key {0} in file {1} has more arguments than supported ({2}).", key, FileUtility.GetRelativeToSpindle(languageFileName), translation.ArgumentCount);
                    continue;
                }

                translationDict.Add(dictKey, value);
            }
        }

        return translationDict;
    }

    public void Save(TranslationCollection collection, IEnumerable<Translation> translations, Language language, bool writeAll)
    {
        GetPaths(collection, out string folderPath, out string filePath);

        filePath = Path.Combine(folderPath, language.GetFileName(), filePath);

        string? dir = Path.GetDirectoryName(filePath);

        if (dir != null)
        {
            Directory.CreateDirectory(dir);
        }

        using PropertiesWriter writer = new PropertiesWriter(filePath);

        bool isFirst = true;
        foreach (Translation translation in translations)
        {
            if (!isFirst)
                writer.WriteLine();

            isFirst = false;
            WriteTranslation(writer, translation, language);
        }
    }

    private static void WriteTranslation(PropertiesWriter writer, Translation translation, Language language)
    {
        translation.AssertInitialized();

        Type type = translation.GetType();

        string value = translation.Original.Value;
        if (translation.Table.TryGetValue(language.Name, out TranslationValue valueEntry))
        {
            value = valueEntry.Value;
        }

        if (!string.IsNullOrWhiteSpace(translation.Data.Description))
        {
            writer.WriteComment("Description: " + translation.Data.Description);
        }

        // write argument descriptions
        StringBuilder argBuilder = new StringBuilder();
        for (int i = 0; i < translation.ArgumentCount; ++i)
        {
            Type argumentType = type.GenericTypeArguments[i];
            argBuilder
                .Append(" {")
                .Append(i.ToString(CultureInfo.InvariantCulture))
                .Append("} - ")
                .Append(Accessor.Formatter.Format(argumentType, refMode: ByRefTypeMode.Ignore));

            ArgumentFormat fmt = translation.GetArgumentFormat(i);
            if (fmt.FormatDisplayName != null)
            {
                argBuilder.Append(" (Format: ").Append(fmt.FormatDisplayName).Append(')');
            }
            else if (fmt.Format != null)
            {
                argBuilder.Append(" (Format: \"").Append(fmt.Format).Append("\")");
            }

            if (translation.Data.ParameterDescriptions is { } descs && descs.Length > i && !string.IsNullOrWhiteSpace(descs[i]))
            {
                argBuilder.Append(" | ").Append(descs[i]);
            }

            if (fmt.FormatAddons is { Length: > 0 })
            {
                writer.WriteComment(argBuilder.ToString());
                argBuilder.Clear();
                if (fmt.FormatAddons.Length == 1)
                {
                    argBuilder
                        .Append("  Addon: ")
                        .Append(fmt.FormatAddons[0].DisplayName ?? Accessor.Formatter.Format(fmt.FormatAddons[0].GetType()));
                    writer.WriteComment(argBuilder.ToString());
                }
                else
                {
                    argBuilder.Append("  Addons:");
                    writer.WriteComment(argBuilder.ToString());
                    argBuilder.Clear();
                    for (int a = 0; a < fmt.FormatAddons.Length; ++a)
                    {
                        argBuilder
                            .Append("   - ")
                            .Append(fmt.FormatAddons[a].DisplayName ?? Accessor.Formatter.Format(fmt.FormatAddons[a].GetType()));
                        writer.WriteComment(argBuilder.ToString());
                        argBuilder.Clear();
                    }
                }
            }
            else
            {
                writer.WriteComment(argBuilder.ToString());
            }
        }

        // write default value if it doesn't match current value.
        if (!translation.Original.Value.Equals(value, StringComparison.Ordinal))
        {
            writer.WriteComment("Default: " + translation.Original.Value);
        }

        writer.WriteKeyValue(translation.Key, value);
    }
}
