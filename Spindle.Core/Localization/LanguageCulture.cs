using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Spindle.Localization;

/// <summary>
/// Defines a culture this language supports (a culture that makes sense for someone using this language to have).
/// </summary>
[Table("lang_cultures")]
public class LanguageCulture
{
    /// <summary>
    /// Primary key for each language culture (for database storage).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("pk")]
    [JsonIgnore]
    public uint Id { get; set; }

    /// <summary>
    /// The language this culture belongs to.
    /// </summary>
    /// <remarks>Must be included for database storage.</remarks>
    [Required]
    [JsonIgnore]
    public Language Language { get; set; } = null!;

    /// <summary>
    /// The ID of the language this culture belongs to.
    /// </summary>
    [Required]
    [Column("Language")]
    [ForeignKey(nameof(Language))]
    [JsonIgnore]
    public uint LanguageId { get; set; }

    /// <summary>
    /// A .NET culture to use for formatting numbers.
    /// </summary>
    [MaxLength(16)]
    [Required]
    [JsonPropertyName("culture_code")]
    public string CultureCode { get; set; } = null!;

    public LanguageCulture()
    {

    }

    public LanguageCulture(Language language, string cultureCode)
    {
        Id = uint.MaxValue;
        Language = language;
        LanguageId = language.Id;
        CultureCode = cultureCode;
    }
}