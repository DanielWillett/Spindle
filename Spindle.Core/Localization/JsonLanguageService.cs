using Microsoft.Extensions.Primitives;
using Spindle.Logging;
using Spindle.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;

namespace Spindle.Localization;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public class JsonLanguageService : ILanguageService, IDisposable
{
    private readonly ILogger<JsonLanguageService> _logger;
    private IDisposable? _changeListener;

    /// <inheritdoc />
    public IReadOnlyList<Language> Languages { get; private set; }

    public JsonLanguageService(ILogger<JsonLanguageService> logger)
    {
        _logger = logger;
        Languages = Array.Empty<Language>();
    }

    internal static bool IsAvailable([MaybeNullWhen(false)] out string existingFilePath)
    {
        string filePath = Path.Combine(SpindlePaths.SpindleDirectory, "Localization", "languages.json");
        string filePath2 = Path.Combine(SpindlePaths.SpindleDirectory, "Localization", "Languages.json");

        if (!File.Exists(filePath))
        {
            filePath = filePath2;
            if (!File.Exists(filePath))
            {
                existingFilePath = null;
                return false;
            }
        }

        existingFilePath = filePath;
        return true;
    }

    UniTask ILanguageService.InitializeAsync()
    {
        ReloadJson();

        return UniTask.CompletedTask;
    }

    UniTask ILanguageService.ReloadAsync()
    {
        ReloadJson();

        return UniTask.CompletedTask;
    }

    private void ReloadJson()
    {
        if (!IsAvailable(out string? filePath))
        {
            throw new FileNotFoundException(Properties.Resources.LanguageFileNotFound, "Languages.json");
        }

        string defaultLang = this.GetDefaultLanguageName();

        string relativePath = filePath;
        if (Path.IsPathRooted(relativePath))
        {
            relativePath = Path.GetRelativePath(SpindleLauncher.SpindleDirectoryFileProvider.Root, relativePath);
        }

        if (_changeListener == null)
        {
            IDisposable? oldListener = Interlocked.Exchange(
                ref _changeListener,
                ChangeToken.OnChange(() => SpindleLauncher.SpindleDirectoryFileProvider.Watch(relativePath), ReloadJson)
            );
            oldListener?.Dispose();
        }

        List<Language?>? languages;

        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            languages = JsonSerializer.Deserialize<List<Language?>>(fs, JsonUtility.JsonSerializerSettings);
        }

        if (languages == null)
        {
            Languages = Array.Empty<Language>();
        }
        else
        {
            while (languages.Remove(null)) ;

            for (int i = 0; i < languages.Count; ++i)
            {
                Language language = languages[i]!;
                for (int j = languages.Count - 1; j > i; --j)
                {
                    if (!languages[j]!.Name.Equals(language.Name, StringComparison.OrdinalIgnoreCase))
                        continue;

                    _logger.LogWarning("Duplicate language name \"{0}\" in language file: {1}.", language.Name, filePath);
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

            if (languages.Count == 0)
                Languages = Array.Empty<Language>();
            else
                Languages = new ReadOnlyCollection<Language>(languages.ToArray()!);
        }

        // reset logger so languages can take effect
        SpindleLauncher.LoggerFactory.Reset();

        _logger.LogDebug("Read {0} language(s) from {1}.", Languages.Count, relativePath);
    }

    public void Dispose()
    {
        Interlocked.Exchange(ref _changeListener, null)?.Dispose();
    }
}