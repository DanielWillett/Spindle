using System;

namespace Spindle.Localization;

/// <summary>
/// Defines a defualt value for the English translation for an enum value or type.
/// </summary>
[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
public sealed class TranslatableAttribute(string defaultValue, string? description = null) : Attribute
{
    /// <summary>
    /// The defualt value for the English translation.
    /// </summary>
    public string DefaultValue { get; } = defaultValue;

    /// <summary>
    /// Overall description for this translation.
    /// </summary>
    public string? Description { get; } = description;

    /// <summary>
    /// Whether or not to export this translation in a translation pack.
    /// Would be <see langword="false"/> in cases such as admin commands, when normal players wouldn't see the translations.
    /// </summary>
    public bool IsPrioritizedTranslation { get; set; } = true;
}