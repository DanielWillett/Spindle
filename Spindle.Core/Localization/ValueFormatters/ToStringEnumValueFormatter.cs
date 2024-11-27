using System;
using DanielWillett.ReflectionTools;

namespace Spindle.Localization.ValueFormatters;

[Priority(int.MinValue)]
public class ToStringEnumStringValueConverter<TEnum> : IEnumStringConverter<TEnum> where TEnum : unmanaged, Enum
{
    /// <inheritdoc />
    public string Format(IValueStringConvertService formatter, TEnum value, in ValueFormatParameters parameters)
    {
        return value.ToString();
    }

    /// <inheritdoc />
    public string GetValue(TEnum value, Language language)
    {
        return value.ToString();
    }

    /// <inheritdoc />
    public string Format(IValueStringConvertService formatter, object value, in ValueFormatParameters parameters)
    {
        return value.ToString();
    }

    /// <inheritdoc />
    public string GetValue(object value, Language language)
    {
        return value.ToString();
    }

    /// <inheritdoc />
    public string GetName(Language language)
    {
        return typeof(TEnum).Name;
    }
}
