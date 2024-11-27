using System;

namespace Spindle.Localization;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class TranslationDataAttribute : Attribute
{
    /// <summary>
    /// Translation name in the file, defaults to the field/property name.
    /// </summary>
    public string? Key { get; set; }
    
    /// <summary>
    /// Overall description for this translation.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Descriptions for each parameter.
    /// </summary>
    public string?[]? Parameters { get; set; }

    /// <summary>
    /// Whether or not to export this translation in a translation pack.
    /// Would be <see langword="false"/> in cases such as admin commands, when normal players wouldn't see the translations.
    /// </summary>
    public bool IsPriorityTranslation { get; set; } = true;

    public TranslationDataAttribute() { }
    public TranslationDataAttribute(string? description)
    {
        Description = description;
    }

    public TranslationDataAttribute(string? description, params string?[]? parameters)
    {
        Description = description;
        Parameters = parameters;
    }
}