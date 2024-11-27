using System;

namespace Spindle.Localization;

/// <summary>
/// Handles humanizing any type of value into a string for logging and translations.
/// <para>Can be <see cref="IDisposable"/>.</para>
/// </summary>
/// <remarks>The best way to implement this for a custom type is to implement <see cref="ITranslationArgument"/>. If that's not viable, create an <see cref="IValueStringConverter{T}"/> for the type.</remarks>
public interface IValueStringConvertService
{
    /// <summary>
    /// Format a <paramref name="value"/> into a string.
    /// </summary>
    string Format<T>(T? value, in ValueFormatParameters parameters);

    /// <summary>
    /// Format a <paramref name="value"/> of a given type (<paramref name="formatType"/>) into a string. If no type is provided, <paramref name="value"/>'s type will be used (unless it's <see langword="null"/>).
    /// </summary>
    string Format(object? value, in ValueFormatParameters parameters, Type? formatType = null);

    /// <summary>
    /// Translate an enum into a string.
    /// </summary>
    string FormatEnum<TEnum>(TEnum value, Language? language) where TEnum : unmanaged, Enum;

    /// <summary>
    /// Translate an enum into a string.
    /// </summary>
    string FormatEnum(object value, Type enumType, Language? language);

    /// <summary>
    /// Translate the name of an enum type into a string.
    /// </summary>
    /// <remarks>Enum names are translatable along with all it's values.</remarks>
    string FormatEnumName<TEnum>(Language? language) where TEnum : unmanaged, Enum;

    /// <summary>
    /// Translate the name of an enum type into a string.
    /// </summary>
    /// <remarks>Enum names are translatable along with all it's values.</remarks>
    string FormatEnumName(Type enumType, Language? language);

    /// <summary>
    /// Colorize text using a translation's options.
    /// </summary>
    string Colorize(ReadOnlySpan<char> text, Color32 color, TranslationOptions options);
}