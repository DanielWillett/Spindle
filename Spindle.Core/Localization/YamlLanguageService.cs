using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Spindle.Localization;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public class YamlLanguageService : ILanguageService, IDisposable
{
    private readonly ILogger<YamlLanguageService> _logger;
    private IConfigurationRoot? _configurationRoot;

    private string? _filePath;

    /// <inheritdoc />
    public IReadOnlyList<Language> Languages { get; private set; }

    public YamlLanguageService(ILogger<YamlLanguageService> logger)
    {
        _logger = logger;
        Languages = Array.Empty<Language>();
    }

    internal static bool IsAvailable(out string existingFilePath)
    {
        string filePath = Path.Combine(SpindlePaths.SpindleDirectory, "Localization", "languages.yml");
        string filePath2 = Path.Combine(SpindlePaths.SpindleDirectory, "Localization", "Languages.yml");

        if (!File.Exists(filePath))
        {
            filePath = filePath2;
            if (!File.Exists(filePath))
            {
                existingFilePath = filePath;
                return false;
            }
        }

        existingFilePath = filePath;
        return true;
    }

    UniTask ILanguageService.InitializeAsync()
    {
        ReloadIntl();
        return UniTask.CompletedTask;
    }

    UniTask ILanguageService.ReloadAsync()
    {
        ReloadIntl();
        return UniTask.CompletedTask;
    }

    private void ReloadIntl()
    {
        if (!IsAvailable(out string filePath))
        {
            using Stream resxStream = SpindleLauncher.AssemblyCore.GetManifestResourceStream("Spindle.Defaults.Languages.yml")
                                      ?? throw new InvalidProgramException("Missing Languages.yml defaults.");


            using FileStream writeToYml = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            resxStream.CopyTo(writeToYml);
        }

        _filePath = filePath;

        _configurationRoot = new ConfigurationBuilder()
            .AddYamlFile(filePath, optional: false, reloadOnChange: true)
            .Build();

        _configurationRoot.GetReloadToken().RegisterChangeCallback(ReloadList, _configurationRoot);

        ReloadList(_configurationRoot);
    }

    private void ReloadList(object? obj)
    {
        IConfiguration configuration = (IConfiguration)obj!;

        IConfiguration subSection = configuration.GetSection("Languages");
        if (subSection.GetChildren().Any())
            configuration = subSection;

        List<Language> languages = new List<Language>();
        foreach (IConfigurationSection section in configuration.GetChildren())
        {
            languages.Add(new Language(section));
        }

        string defaultLang = this.GetDefaultLanguageName();

        for (int i = 0; i < languages.Count; ++i)
        {
            Language language = languages[i]!;
            for (int j = languages.Count - 1; j > i; --j)
            {
                if (!languages[j]!.Name.Equals(language.Name, StringComparison.OrdinalIgnoreCase))
                    continue;

                _logger.LogWarning("Duplicate language name \"{0}\" in language file: {1}.", language.Name, _filePath);
                languages.RemoveAt(j);
            }
        }

        for (int i = 0; i < languages.Count; ++i)
        {
            Language language = languages[i]!;
            if (language.Name.Equals(defaultLang, StringComparison.OrdinalIgnoreCase))
            {
                language.IsDefault = true;
            }
        }

        Languages = new ReadOnlyCollection<Language>(languages.ToArray());

        // reset logger so languages can take effect
        SpindleLauncher.LoggerFactory.Reset();

        _logger.LogDebug("Read {0} language(s).", Languages.Count);
    }

    public void Dispose()
    {
        (_configurationRoot as IDisposable)?.Dispose();
    }
}
