using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Spindle.Localization;

/// <summary>
/// Allows the server to keep track of who contributed to creating translations for a language and credit them.
/// </summary>
[Table("spindle_lang_contributors")]
public class LanguageContributor
{
    /// <summary>
    /// Primary key for each language contributor (for database storage).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public uint Id { get; set; }

    /// <summary>
    /// The language this contributor belongs to.
    /// </summary>
    /// <remarks>Must be included for database storage.</remarks>
    [Required]
    [JsonIgnore]
    public Language Language { get; set; } = null!;

    /// <summary>
    /// The ID of the language this contributor belongs to.
    /// </summary>
    [Required]
    [Column("Language")]
    [ForeignKey(nameof(Language))]
    [JsonIgnore]
    public uint LanguageId { get; set; }

    /// <summary>
    /// Steam64 ID of a user that helped create translations for this language.
    /// </summary>
    [Required]
    [Column("Contributor")]
    [JsonPropertyName("contributor")]
    public ulong Contributor { get; set; }

    public LanguageContributor()
    {
        
    }

    public LanguageContributor(Language language, CSteamID contributor)
    {
        Id = uint.MaxValue;
        Language = language;
        LanguageId = language.Id;
        Contributor = contributor.m_SteamID;
    }
}