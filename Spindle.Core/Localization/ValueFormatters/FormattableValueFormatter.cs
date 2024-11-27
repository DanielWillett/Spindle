using DanielWillett.ReflectionTools;
using System;

namespace Spindle.Localization.ValueFormatters;

[Priority(int.MinValue + 1)]
public class FormattableValueFormatter<TFormattable> : IValueStringConverter<TFormattable> where TFormattable : IFormattable
{
    string IValueStringConverter.Format(IValueStringConvertService formatter, object value, in ValueFormatParameters parameters)
    {
        return Format(formatter, (TFormattable)value, in parameters);
    }

    public string Format(IValueStringConvertService formatter, TFormattable value, in ValueFormatParameters parameters)
    {
        return value.ToString(parameters.Format.UseForToString ? parameters.Format.Format : null, parameters.Culture);
    }
}