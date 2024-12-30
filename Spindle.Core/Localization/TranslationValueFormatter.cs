using DanielWillett.ReflectionTools;
using Microsoft.Extensions.DependencyInjection;
using Spindle.Localization.ValueFormatters;
using Spindle.Logging;
using Spindle.Plugins;
using Spindle.Util;
using StackCleaner;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;

namespace Spindle.Localization;

/// <summary>
/// Handles humanizing any type of value into a string for logging and translations.
/// </summary>
/// <remarks>The best way to implement this for a custom type is to implement <see cref="ITranslationArgument"/>. If that's not viable, create an <see cref="IValueStringConverter{T}"/> for the type.</remarks>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public class ValueStringConvertService : IValueStringConvertService, IDisposable
{
    private struct ConverterInfo
    {
        public object Converter;
        public int Priority;
    }

    private readonly ILanguageService _languageService;
    private readonly ILogger<ValueStringConvertService> _logger;

    private const string NullNoColor = "null";
    private const string NullColorUnity = "<color=#569cd6><b>null</b></color>";
    private const string NullColorTMPro = "<#569cd6><b>null</b></color>";
    private const string NullANSI = "\e[94mnull\e[39m";
    private const string NullExtendedANSI = "\e[38;2;86;156;214mnull\e[39m";

    private readonly List<ConverterInfo> _enumFormatters;
    private readonly List<ConverterInfo> _converters;

    private readonly ConcurrentDictionary<Type, object> _valueFormatters = new ConcurrentDictionary<Type, object>();
    public ValueStringConvertService(ILanguageService languageService, ILogger<ValueStringConvertService> logger, SpindlePluginLoader plugins)
    {
        _languageService = languageService;
        _logger = logger;

        List<Type> enumFormatters = plugins.GetTypesOfBaseType<IEnumStringConverter>(removeIgnored: true);
        if (enumFormatters.Count == 0)
            enumFormatters.Add(typeof(ToStringEnumStringValueConverter<>));

        _enumFormatters = new List<ConverterInfo>(enumFormatters.Count);
        foreach (Type type in enumFormatters)
        {
            ConverterInfo info = default;
            info.Converter = type;
            info.Priority = type.GetPriority();
            _enumFormatters.Add(info);
        }

        List<Type> types = plugins.GetTypesOfBaseType<IValueStringConverter>(removeIgnored: true);
        types.RemoveAll(typeof(IEnumStringConverter).IsAssignableFrom);

        _converters = new List<ConverterInfo>(types.Count);
        foreach (Type type in types)
        {
            AddConverter(type, type.GetPriority());
        }
    }

    /// <summary>
    /// Manually add an enum converter.
    /// </summary>
    public void AddEnumConverter(Type converterType, int priority = 0)
    {
        if (converterType.IsGenericTypeDefinition)
        {
            Type[] args = converterType.GetGenericArguments();
            if (args.Length != 1)
                throw new ArgumentException(Properties.Resources.ExceptionConverterInvalidGenericType, nameof(converterType));

            Type? interfaceType = Array.Find(converterType.GetInterfaces(), x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumStringConverter<>));
            if (interfaceType != null && !interfaceType.GetGenericArguments()[0].IsAssignableFrom(args[0]))
                throw new ArgumentException(Properties.Resources.ExceptionConverterInvalidGenericType, nameof(converterType));
        }

        ConverterInfo converterInfo = default;
        converterInfo.Converter = converterType;
        converterInfo.Priority = priority;

        lock (_enumFormatters)
        {
            bool inserted = false;
            for (int i = 0; i < _enumFormatters.Count; ++i)
            {
                if (_enumFormatters[i].Priority >= priority)
                    continue;

                _enumFormatters.Insert(i, converterInfo);
                inserted = true;
                break;
            }

            if (!inserted)
                _enumFormatters.Add(converterInfo);
        }

        ClearFormatters();
    }

    /// <summary>
    /// Manually add a type of value converter.
    /// </summary>
    /// <remarks>Open generic types are valid here so long as there's only one generic argument (which would be the type being formatted).</remarks>
    public void AddConverter(Type converterType, int priority = 0)
    {
        if (typeof(IEnumStringConverter).IsAssignableFrom(converterType))
            throw new ArgumentException(Properties.Resources.ExceptionEnumConverterNotValid, nameof(converterType));

        if (converterType.IsGenericTypeDefinition)
        {
            Type[] args = converterType.GetGenericArguments();
            if (args.Length != 1)
                throw new ArgumentException(Properties.Resources.ExceptionConverterInvalidGenericType, nameof(converterType));

            Type? interfaceType = Array.Find(converterType.GetInterfaces(), x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IValueStringConverter<>));
            if (interfaceType != null && !interfaceType.GetGenericArguments()[0].IsAssignableFrom(args[0]))
                throw new ArgumentException(Properties.Resources.ExceptionConverterInvalidGenericType, nameof(converterType));
        }

        AddConverterIntl(converterType, priority);

        ClearFormatters();
    }

    /// <summary>
    /// Manually add a value converter object.
    /// </summary>
    public void AddConverter(IValueStringConverter converter, int priority = 0)
    {
        if (converter is IEnumStringConverter)
            throw new ArgumentException(Properties.Resources.ExceptionEnumConverterNotValid, nameof(converter));

        ConverterInfo converterInfo = default;
        converterInfo.Priority = priority;
        converterInfo.Converter = converter;

        lock (_converters)
        {
            bool inserted = false;
            for (int i = 0; i < _converters.Count; ++i)
            {
                if (_converters[i].Priority >= priority)
                    continue;

                _converters.Insert(i, converterInfo);
                inserted = true;
                break;
            }

            if (!inserted)
                _converters.Add(converterInfo);
        }

        ClearFormatters();
    }

    private void AddConverterIntl(Type converterType, int priority)
    {
        object? obj = null;
        try
        {
            if (!converterType.IsGenericTypeDefinition && converterType.GetConstructor(Type.EmptyTypes) is { } ctor)
            {
                obj = ctor.Invoke([]);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error creating stateless converter: {0}.", converterType);
        }

        obj ??= converterType;
        ConverterInfo converterInfo = default;
        converterInfo.Priority = priority;
        converterInfo.Converter = obj;

        lock (_converters)
        {
            bool inserted = false;
            for (int i = 0; i < _converters.Count; ++i)
            {
                if (_converters[i].Priority >= priority)
                    continue;

                _converters.Insert(i, converterInfo);
                inserted = true;
                break;
            }

            if (!inserted)
                _converters.Add(converterInfo);
        }
    }


    /// <inheritdoc />
    public string Format<T>(T? value, in ValueFormatParameters parameters)
    {
        string formattedValue = FormatIntl(value, in parameters);

        IArgumentAddon[]? addons = parameters.Format.FormatAddons;
        TypedReference typeRef = __makeref(value);
        if (addons != null)
        {
            for (int i = 0; i < addons.Length; ++i)
            {
                formattedValue = addons[i].ApplyAddon(this, formattedValue, typeRef, in parameters);
            }
        }

        return formattedValue;
    }

    /// <inheritdoc />
    public string Format(object? value, in ValueFormatParameters parameters, Type? formatType = null)
    {
        string formattedValue = FormatIntl(value, in parameters, formatType);

        IArgumentAddon[]? addons = parameters.Format.FormatAddons;
        TypedReference typeRef = __makeref(value);

        if (addons != null)
        {
            for (int i = 0; i < addons.Length; ++i)
            {
                formattedValue = addons[i].ApplyAddon(this, formattedValue, typeRef, in parameters);
            }
        }

        return formattedValue;
    }

    public string FormatEnum<TEnum>(TEnum value, Language? language) where TEnum : unmanaged, Enum
    {
        return ((IEnumStringConverter<TEnum>)GetValueFormatter<TEnum>()).GetValue(value, language ?? _languageService.GetDefaultLanguage());
    }

    public string FormatEnum(object value, Type enumType, Language? language)
    {
        return ((IEnumStringConverter)GetValueFormatter(enumType)).GetValue(value, language ?? _languageService.GetDefaultLanguage());
    }

    public string FormatEnumName<TEnum>(Language? language) where TEnum : unmanaged, Enum
    {
        return ((IEnumStringConverter<TEnum>)GetValueFormatter<TEnum>()).GetName(language ?? _languageService.GetDefaultLanguage());
    }

    public string FormatEnumName(Type enumType, Language? language)
    {
        return ((IEnumStringConverter)GetValueFormatter(enumType)).GetName(language ?? _languageService.GetDefaultLanguage());
    }

    private string FormatIntl<T>(T? value, in ValueFormatParameters parameters)
    {
        if (Equals(value, null))
        {
            return FormatNull(in parameters);
        }

        if (value is ITranslationArgument preDefined)
        {
            return preDefined.Translate(this, in parameters);
        }

        IValueStringConverter valueFormatter = GetValueFormatter<T>();

        if (valueFormatter is IValueStringConverter<T> v)
            return v.Format(this, value, in parameters);

        return valueFormatter.Format(this, value, in parameters);
    }
    
    private string FormatIntl(object? value, in ValueFormatParameters parameters, Type? formatType)
    {
        if (Equals(value, null))
        {
            return FormatNull(in parameters);
        }

        if (value is ITranslationArgument preDefined)
        {
            return preDefined.Translate(this, in parameters);
        }

        formatType ??= value.GetType() ?? typeof(object);
        IValueStringConverter valueFormatter = GetValueFormatter(formatType);

        return valueFormatter.Format(this, value, in parameters);
    }

    private IValueStringConverter GetValueFormatter<T>() => GetValueFormatter(typeof(T));
    private IValueStringConverter GetValueFormatter(Type type)
    {
        return (IValueStringConverter)_valueFormatters.GetOrAdd(type, static (type, vf) =>
        {
            if (type.IsEnum)
            {
                lock (vf._enumFormatters)
                {
                    for (int i = 0; i < vf._enumFormatters.Count; ++i)
                    {
                        try
                        {
                            Type t = (Type)vf._enumFormatters[i].Converter;
                            return ActivatorUtilities.CreateInstance(SpindleLauncher.ServiceProvider, t.IsGenericTypeDefinition ? t.MakeGenericType(type) : t);
                        }
                        catch (Exception ex)
                        {
                            vf._logger.LogWarning(ex, "Failed to instantiate enum string converter {0}.", type);
                        }
                    }
                }
            }

            lock (vf._converters)
            {
                Type lookingForFormatterType = typeof(IValueStringConverter<>).MakeGenericType(type);
                foreach (ConverterInfo valueFormatter in vf._converters)
                {
                    if (valueFormatter.Converter is not Type actualType)
                    {
                        if (!lookingForFormatterType.IsInstanceOfType(valueFormatter))
                            continue;

                        return valueFormatter;
                    }

                    if (actualType.IsGenericTypeDefinition)
                    {
                        Type[] genericArgs = actualType.GetGenericArguments();
                        if (genericArgs.Length != 1)
                            continue;

                        // this is 100x easier and cleaner than checking every single generic type constraint manually, sue me
                        try
                        {
                            actualType = actualType.MakeGenericType(type);
                        }
                        catch (ArgumentException)
                        {
                            continue;
                        }
                    }

                    if (!lookingForFormatterType.IsAssignableFrom(actualType))
                        continue;

                    try
                    {
                        return ActivatorUtilities.CreateInstance(SpindleLauncher.ServiceProvider, actualType);
                    }
                    catch (Exception ex)
                    {
                        vf._logger.LogWarning(ex, "Failed to instantiate value string converter {0} for {1}.", actualType, type);
                    }
                }
            }

            return new ToStringValueFormatter();
        }, this);
    }

    public string Colorize(ReadOnlySpan<char> text, Color32 color, TranslationOptions options)
    {
        return TranslationFormattingUtility.Colorize(text, color, options, TerminalColorHelper.ColorSetting);
    }

    private static string FormatNull(in ValueFormatParameters parameters)
    {
        if ((parameters.Options & TranslationOptions.NoRichText) != 0)
        {
            return NullNoColor;
        }

        if ((parameters.Options & TranslationOptions.TranslateWithTerminalRichText) != 0)
        {
            return TerminalColorHelper.ColorSetting switch
            {
                StackColorFormatType.ExtendedANSIColor => NullExtendedANSI,
                StackColorFormatType.ANSIColor => NullANSI,
                _ => NullNoColor
            };
        }

        return (parameters.Options & TranslationOptions.TranslateWithUnityRichText) != 0 ? NullColorUnity : NullColorTMPro;
    }

    public void Dispose()
    {
        ClearFormatters();
    }

    private void ClearFormatters()
    {
        while (_valueFormatters.Count > 0)
        {
            KeyValuePair<Type, object>[] kvp = _valueFormatters.ToArray();
            for (int i = 0; i < kvp.Length; ++i)
            {
                if (_valueFormatters.TryRemove(kvp[i].Key, out object val) && val is IDisposable disp)
                {
                    disp.Dispose();
                }
            }
        }
    }
}