using DanielWillett.ReflectionTools;
using System;

namespace Spindle.Localization;

/// <summary>
/// Converts values of type <typeparamref name="TFormattable"/> to strings. <see cref="IValueStringConverter"/> instances can be <see cref="IDisposable"/>.
/// </summary>
/// <remarks>Use the <see cref="IgnoreAttribute"/> to keep your converter from getting auto-registered and use the <see cref="PriorityAttribute"/> to order a converter above others.</remarks>
public interface IValueStringConverter<in TFormattable> : IValueStringConverter
{
    string Format(IValueStringConvertService formatter, TFormattable value, in ValueFormatParameters parameters);
}

/// <summary>
/// Converts values to strings. Any converter implementing only this should work on all objects. Otherwise implement <see cref="IValueStringConverter{TFormattable}"/> instead. <see cref="IValueStringConverter"/> instances can be <see cref="IDisposable"/>.
/// </summary>
/// <remarks>Use the <see cref="IgnoreAttribute"/> to keep your converter from getting auto-registered and use the <see cref="PriorityAttribute"/> to order a converter above others.</remarks>
public interface IValueStringConverter
{
    string Format(IValueStringConvertService formatter, object value, in ValueFormatParameters parameters);
}

/// <summary>
/// Converts specifically enums to strings of type <typeparamref name="TEnum"/>. <see cref="IEnumStringConverter"/> instances can be <see cref="IDisposable"/>.
/// </summary>
/// <remarks>Use the <see cref="IgnoreAttribute"/> to keep your converter from getting auto-registered and use the <see cref="PriorityAttribute"/> to order a converter above others.</remarks>
public interface IEnumStringConverter<in TEnum> : IEnumStringConverter, IValueStringConverter<TEnum> where TEnum : unmanaged, Enum
{
    string GetValue(TEnum value, Language language);
}

/// <summary>
/// Converts specifically enums to strings. Any converter implementing only this should work on all enums. Otherwise implement <see cref="IEnumStringConverter{TEnum}"/> instead. <see cref="IEnumStringConverter"/> instances can be <see cref="IDisposable"/>.
/// </summary>
/// <remarks>Use the <see cref="IgnoreAttribute"/> to keep your converter from getting auto-registered and use the <see cref="PriorityAttribute"/> to order a converter above others.</remarks>
public interface IEnumStringConverter : IValueStringConverter
{
    string GetValue(object value, Language language);
    string GetName(Language language);
}