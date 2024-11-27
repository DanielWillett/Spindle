using Spindle.Players;
using System;
using System.Globalization;

namespace Spindle.Localization;

public readonly ref struct ValueFormatParameters
{
    public readonly int Argument;
    public readonly CultureInfo Culture;
    public readonly Language Language;
    public readonly TranslationOptions Options;
    public readonly ArgumentFormat Format;
    public readonly SpindlePlayer Player;
    public readonly Func<int, object?>? ArgumentAccessor;
    public readonly int ArgumentCount;

    /// <summary>
    /// If unity rich text should be used over TMPro rich text.
    /// </summary>
    public bool IMGUI => (Options & TranslationOptions.TranslateWithUnityRichText) != 0;
    public ValueFormatParameters(in ValueFormatParameters parameters, TranslationOptions flags)
        : this (parameters.Argument, parameters.Culture, parameters.Language, flags, in parameters.Format, parameters.Player, parameters.ArgumentAccessor, parameters.ArgumentCount) { }
    public ValueFormatParameters(int argument, in TranslationArguments args, in ArgumentFormat format, Func<int, object?>? argumentAccessor, int argumentCount)
        : this (argument, args.Culture, args.Language, args.Options, in format, args.Player, argumentAccessor, argumentCount) { }
    public ValueFormatParameters(int argument, CultureInfo culture, Language language, TranslationOptions options, in ArgumentFormat format, SpindlePlayer player, Func<int, object?>? argumentAccessor, int argumentCount)
    {
        Argument = argument;
        Culture = culture;
        Language = language;
        Options = options;
        Format = format;
        Player = player;
        ArgumentAccessor = argumentAccessor;
        ArgumentCount = argumentCount;
    }

    public static implicit operator TranslationOptions(in ValueFormatParameters parameters) => parameters.Options;
}
