using Microsoft.Extensions.Configuration;
using Spindle.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Spindle.Localization;

/// <summary>
/// Data structure for information about a language for the server.
/// </summary>
[Table("spindle_languages")]
[JsonConverter(typeof(LanguageJsonConverter))]
public class Language
{
    private string? _fileName;

    /// <summary>
    /// Primary key for each language (for database storage).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public uint Id { get; set; }

    /// <summary>
    /// Unique name for this language. Users won't ever see this.
    /// </summary>
    [Required]
    [MaxLength(64)]
    [JsonPropertyName("name")]
    public string Name
    {
        get;
        set
        {
            field = value;
            _fileName = null;
        }
    } = null!;

    /// <summary>
    /// Displayed name for this language.
    /// </summary>
    [Required]
    [MaxLength(64)]
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = null!;

    /// <summary>
    /// This language's name translated for this language. Example: 'Español' for Spanish.
    /// </summary>
    [MaxLength(64)]
    [JsonPropertyName("native_name")]
    public string? NativeName { get; set; }

    /// <summary>
    /// The default .NET culture to use for formatting numbers.
    /// </summary>
    [MaxLength(16)]
    [JsonPropertyName("default_culture_code")]
    public string? DefaultCultureCode { get; set; }
    
    /// <summary>
    /// If translations for this language aren't available but there are some for the fallback language, those will be chosen over the default language.
    /// </summary>
    [Column(TypeName = "char(5)")]
    [JsonPropertyName("fallback_language")]
    public string? FallbackTranslationLanguageName { get; set; }

    /// <summary>
    /// The language's name for Steam's langauge list. This is the same as the name used by Unturned's localization system.
    /// </summary>
    [MaxLength(32)]
    [JsonPropertyName("steam_name")]
    public string? SteamName
    {
        get;
        set
        {
            field = value;
            _fileName = null;
        }
    }

    public IList<LanguageAlias> Aliases { get; set; } = null!;
    public IList<LanguageContributor> Contributors { get; set; } = null!;
    public IList<LanguageCulture> SupportedCultures { get; set; } = null!;

    /// <summary>
    /// If this language is the default language for the server.
    /// </summary>
    [JsonIgnore, NotMapped]
    public bool IsDefault { get; internal set; }

    public string GetFileName()
    {
        if (_fileName != null)
            return _fileName;

        if (SteamName != null)
        {
            return _fileName = string.Create(SteamName.Length, SteamName, (span, state) =>
            {
                span[0] = char.ToUpper(state[0]);
                state.AsSpan(1).CopyTo(span[1..]);
            });
        }

        return _fileName = Name;
    }

    public Language()
    {

    }

    public Language(string name, string displayName, string? nativeName, string? defaultCultureCode, string? fallbackTranslationLanguageName = null)
    {
        Name = name;
        DisplayName = displayName;
        NativeName = nativeName;
        DefaultCultureCode = defaultCultureCode;
        FallbackTranslationLanguageName = fallbackTranslationLanguageName;
        Aliases = new List<LanguageAlias>();
        Contributors = new List<LanguageContributor>();
        SupportedCultures = new List<LanguageCulture>();
    }

    public Language(IConfiguration configuration)
    {
        Name = configuration["Name"] ?? throw new FormatException(Properties.Resources.LanguageParse_NameRequired);
        DisplayName = configuration["DisplayName"] ?? throw new FormatException(Properties.Resources.LanguageParse_DisplayNameRequired);
        NativeName = configuration["NativeName"];
        DefaultCultureCode = configuration["DefaultCultureCode"];
        FallbackTranslationLanguageName = configuration["FallbackTranslationLanguageName"];
        SteamName = configuration["SteamName"];

        Aliases = new List<LanguageAlias>();
        Contributors = new List<LanguageContributor>();
        SupportedCultures = new List<LanguageCulture>();

        foreach (IConfigurationSection section in configuration.GetSection("Aliases").GetChildren())
        {
            string? alias = section["Alias"] ?? section.Value;
            if (!string.IsNullOrEmpty(alias))
                Aliases.Add(new LanguageAlias(this, alias));
        }

        foreach (IConfigurationSection section in configuration.GetSection("Contributors").GetChildren())
        {
            string? contributor = section["Contributor"] ?? section.Value;
            if (!FormattingUtility.TryParseSteamId(contributor, out CSteamID steam64) && steam64.GetEAccountType() == EAccountType.k_EAccountTypeIndividual)
                Contributors.Add(new LanguageContributor(this, steam64));
        }

        foreach (IConfigurationSection section in configuration.GetSection("SupportedCultures").GetChildren())
        {
            string? cultureCode = section["CultureCode"] ?? section.Value;
            if (!string.IsNullOrEmpty(cultureCode))
                SupportedCultures.Add(new LanguageCulture(this, cultureCode));
        }
    }
}

public class LanguageJsonConverter : JsonConverter<Language>
{
    public override void Write(Utf8JsonWriter writer, Language value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();
        writer.WriteString("name", value.Name);
        writer.WriteString("display_name", value.DisplayName);
        writer.WriteString("native_name", value.NativeName);
        writer.WriteString("default_culture_code", value.DefaultCultureCode);
        writer.WriteString("fallback_language", value.FallbackTranslationLanguageName);
        writer.WriteString("steam_name", value.SteamName);

        writer.WritePropertyName("aliases");
        writer.WriteStartArray();
        if (value.Aliases is { Count: > 0 })
        {
            foreach (LanguageAlias alias in value.Aliases)
            {
                if (!string.IsNullOrEmpty(alias.Alias))
                    writer.WriteStringValue(alias.Alias);
            }
        }
        writer.WriteEndArray();

        writer.WritePropertyName("contributors");
        writer.WriteStartArray();
        if (value.Contributors is { Count: > 0 })
        {
            foreach (LanguageContributor contributor in value.Contributors)
            {
                if (new CSteamID(contributor.Contributor).GetEAccountType() == EAccountType.k_EAccountTypeIndividual)
                    writer.WriteNumberValue(contributor.Contributor);
            }
        }
        writer.WriteEndArray();

        writer.WritePropertyName("supported_cultures");
        writer.WriteStartArray();
        if (value.SupportedCultures is { Count: > 0 })
        {
            foreach (LanguageCulture culture in value.SupportedCultures)
            {
                if (!string.IsNullOrEmpty(culture.CultureCode))
                    writer.WriteStringValue(culture.CultureCode);
            }
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }

    public override Language? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException($"Unexpected token parsing Language: {reader.TokenType}.");

        Language language = new Language
        {
            Id = uint.MaxValue,
            Aliases = new List<LanguageAlias>(),
            Contributors = new List<LanguageContributor>(),
            SupportedCultures = new List<LanguageCulture>()
        };

        JsonUtility.ReadTopLevelProperties(ref reader, ref language, (ref Utf8JsonReader reader, string propertyName, ref Language language) =>
        {
            if (propertyName.Equals("name", StringComparison.OrdinalIgnoreCase))
            {
                language.Name = reader.GetString()!;
            }
            else if (propertyName.Equals("display_name", StringComparison.OrdinalIgnoreCase))
            {
                language.DisplayName = reader.GetString()!;
            }
            else if (propertyName.Equals("native_name", StringComparison.OrdinalIgnoreCase))
            {
                language.NativeName = reader.GetString();
            }
            else if (propertyName.Equals("default_culture_code", StringComparison.OrdinalIgnoreCase))
            {
                language.DefaultCultureCode = reader.GetString();
            }
            else if (propertyName.Equals("fallback_language", StringComparison.OrdinalIgnoreCase))
            {
                language.FallbackTranslationLanguageName = reader.GetString();
            }
            else if (propertyName.Equals("steam_name", StringComparison.OrdinalIgnoreCase))
            {
                language.SteamName = reader.GetString();
            }
            else if (propertyName.Equals("aliases", StringComparison.OrdinalIgnoreCase))
            {
                if (reader.TokenType != JsonTokenType.StartArray)
                {
                    if (reader.TokenType == JsonTokenType.Null)
                        return;

                    throw new JsonException($"Unexpected token parsing Language aliases: {reader.TokenType}.");
                }

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;

                    if (reader.TokenType != JsonTokenType.StartObject)
                    {
                        if (reader.TokenType != JsonTokenType.String)
                            throw new JsonException($"Unexpected token parsing Language aliases: {reader.TokenType}.");

                        string? alias = reader.GetString();

                        if (string.IsNullOrWhiteSpace(alias))
                            continue;

                        language.Aliases.Add(new LanguageAlias(language, alias));
                    }

                    JsonUtility.ReadTopLevelProperties(ref reader, ref language, (ref Utf8JsonReader reader, string propertyName, ref Language language) =>
                    {
                        if (!propertyName.Equals("alias", StringComparison.OrdinalIgnoreCase))
                            return;

                        if (reader.TokenType != JsonTokenType.String)
                            throw new JsonException($"Unexpected token parsing Language aliases: {reader.TokenType}.");

                        string? alias = reader.GetString();

                        if (string.IsNullOrWhiteSpace(alias))
                            return;

                        language.Aliases.Add(new LanguageAlias(language, alias));
                    });
                }
            }
            else if (propertyName.Equals("contributors", StringComparison.OrdinalIgnoreCase))
            {
                if (reader.TokenType != JsonTokenType.StartArray)
                {
                    if (reader.TokenType == JsonTokenType.Null)
                        return;

                    throw new JsonException($"Unexpected token parsing Language contributors: {reader.TokenType}.");
                }

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;

                    if (reader.TokenType != JsonTokenType.StartObject)
                    {
                        if (reader.TokenType != JsonTokenType.Number)
                            throw new JsonException($"Unexpected token parsing Language contributors: {reader.TokenType}.");

                        CSteamID contributor = new CSteamID(reader.GetUInt64());

                        if (contributor.GetEAccountType() != EAccountType.k_EAccountTypeIndividual)
                            continue;

                        language.Contributors.Add(new LanguageContributor(language, contributor));
                    }

                    JsonUtility.ReadTopLevelProperties(ref reader, ref language, (ref Utf8JsonReader reader, string propertyName, ref Language language) =>
                    {
                        if (!propertyName.Equals("contributor", StringComparison.OrdinalIgnoreCase))
                            return;

                        if (reader.TokenType != JsonTokenType.Number)
                            throw new JsonException($"Unexpected token parsing Language contributors: {reader.TokenType}.");

                        CSteamID contributor = new CSteamID(reader.GetUInt64());

                        if (contributor.GetEAccountType() != EAccountType.k_EAccountTypeIndividual)
                            return;

                        language.Contributors.Add(new LanguageContributor(language, contributor));
                    });
                }
            }
            else if (propertyName.Equals("supported_cultures", StringComparison.OrdinalIgnoreCase))
            {
                if (reader.TokenType != JsonTokenType.StartArray)
                {
                    if (reader.TokenType == JsonTokenType.Null)
                        return;

                    throw new JsonException($"Unexpected token parsing Language supported_cultures: {reader.TokenType}.");
                }

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;

                    if (reader.TokenType != JsonTokenType.StartObject)
                    {
                        if (reader.TokenType != JsonTokenType.String)
                            throw new JsonException($"Unexpected token parsing Language supported_cultures: {reader.TokenType}.");

                        string? cultureCode = reader.GetString();

                        if (string.IsNullOrWhiteSpace(cultureCode))
                            continue;

                        language.SupportedCultures.Add(new LanguageCulture(language, cultureCode));
                    }

                    JsonUtility.ReadTopLevelProperties(ref reader, ref language, (ref Utf8JsonReader reader, string propertyName, ref Language language) =>
                    {
                        if (!propertyName.Equals("culture_code", StringComparison.OrdinalIgnoreCase))
                            return;

                        if (reader.TokenType != JsonTokenType.String)
                            throw new JsonException($"Unexpected token parsing Language supported_cultures: {reader.TokenType}.");

                        string? cultureCode = reader.GetString();

                        if (string.IsNullOrWhiteSpace(cultureCode))
                            return;

                        language.SupportedCultures.Add(new LanguageCulture(language, cultureCode));
                    });
                }
            }
        });

        if (language.Name == null)
            throw new JsonException(Properties.Resources.LanguageParse_NameRequired);

        if (language.DisplayName == null)
            throw new JsonException(Properties.Resources.LanguageParse_DisplayNameRequired);

        return language;
    }
}