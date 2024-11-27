﻿using Spindle.Players;
using System;
using System.Globalization;

namespace Spindle.Localization;

/// <summary>
/// Shared structure of common data for processing translations.
/// </summary>
public readonly ref struct TranslationArguments
{
    /// <summary>
    /// The set of values relating to this translation.
    /// </summary>
    public readonly TranslationValue ValueSet;

    /// <summary>
    /// If Unity Rich Text formatted text should be used instead of TextMeshPro formatted text.
    /// </summary>
    public readonly bool UseIMGUI;

    /// <summary>
    /// If the starting color tag should be removed and returned separately (this allows us to get a few more characters out of chat messages).
    /// </summary>
    public readonly bool UseUncoloredTranslation;

    /// <summary>
    /// The unprocessed translation value based on the language given.
    /// </summary>
    public readonly ReadOnlySpan<char> PreformattedValue;

    /// <summary>
    /// Specific player this translation is being formatted for, if any.
    /// </summary>
    public readonly SpindlePlayer Player;

    /// <summary>
    /// Flag options for this translation.
    /// </summary>
    public readonly TranslationOptions Options;

    /// <summary>
    /// The language to use for this translation.
    /// </summary>
    public readonly Language Language;

    /// <summary>
    /// The format provider to use for this translation.
    /// </summary>
    public readonly CultureInfo Culture;

    public TranslationArguments(TranslationValue valueSet, bool useIMGUI, bool useUncoloredTranslation, SpindlePlayer player, TranslationOptions options)
        : this(valueSet, useIMGUI, useUncoloredTranslation, player.Localization.Language, player, options, player.Localization.Culture)
    {

    }

    public TranslationArguments(TranslationValue valueSet, bool useIMGUI, bool useUncoloredTranslation, Language language, SpindlePlayer player, TranslationOptions options, CultureInfo culture)
    {
        ValueSet = valueSet;
        UseIMGUI = useIMGUI;
        UseUncoloredTranslation = useUncoloredTranslation;
        PreformattedValue = valueSet.GetValueSpan(useIMGUI, useUncoloredTranslation, (options & TranslationOptions.ForTerminal) != 0);
        Language = language;
        Player = player;
        Options = options;
        Culture = culture;
    }
}