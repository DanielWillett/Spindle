using DanielWillett.ReflectionTools;

namespace Spindle.Localization.ValueFormatters;

[Priority(int.MinValue)]
public class ToStringValueFormatter : IValueStringConverter<object>
{
    public string Format(IValueStringConvertService formatter, object value, in ValueFormatParameters parameters) => value.ToString();
}
