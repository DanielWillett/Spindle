using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Spindle.Localization;

/// <summary>
/// Defines another name this language can be queried by.
/// </summary>
[Table("spindle_lang_aliases")]
public class LanguageAlias
{
    /// <summary>
    /// Primary key for each language alias (for database storage).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public uint Id { get; set; }

    /// <summary>
    /// The language this alias belongs to.
    /// </summary>
    /// <remarks>Must be included for database storage.</remarks>
    [Required]
    [JsonIgnore]
    public Language Language { get; set; } = null!;

    /// <summary>
    /// The ID of the language this alias belongs to.
    /// </summary>
    [Required]
    [Column("Language")]
    [ForeignKey(nameof(Language))]
    [JsonIgnore]
    public uint LanguageId { get; set; }

    /// <summary>
    /// Another name this language can be queried by.
    /// </summary>
    [MaxLength(64)]
    [Required]
    [JsonPropertyName("alias")]
    public string Alias { get; set; } = null!;

    public LanguageAlias()
    {
        
    }

    public LanguageAlias(Language language, string alias)
    {
        Id = uint.MaxValue;
        Language = language;
        LanguageId = language.Id;
        Alias = alias;
    }
}