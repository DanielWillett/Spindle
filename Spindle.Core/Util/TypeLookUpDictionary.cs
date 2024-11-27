using DanielWillett.ReflectionTools;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;

namespace Spindle.Util;

/// <summary>
/// Thread-safe dictionary that uses generics to create a fast type-based lookup table.
/// </summary>
/// <typeparam name="TType">The base type to store in the dictionary.</typeparam>
internal class TypeLookUpDictionary<TType> where TType : notnull
{
    private static readonly object Sync = new object();
    private TType[] _values;
    private Type[] _types;

    /// <summary>
    /// List of all values in order of their types.
    /// </summary>
    // ReSharper disable once InconsistentlySynchronizedField
    public TType[] Values => _values;
    public TypeLookUpDictionary(Type[] types, TType[] values)
    {
        _values = values;
        _types = types;
        Type[] typeArgs = [ typeof(TType), null! ];
        for (int i = 0; i < types.Length; ++i)
        {
            SetIndex(types[i], i, typeArgs);
        }
    }

    public static void AddValueTypes(TypeLookUpDictionary<TType>[] dictionaries, IList<Type> types, IList<TType>[] values)
    {
        if (dictionaries.Length == 0 || types.Count == 0)
            return;

        lock (Sync)
        {
            int ct = types.Count;
            
            for (int k = 0; k < dictionaries.Length; ++k)
            {
                TypeLookUpDictionary<TType> d = dictionaries[k];

                TType[] newValues = new TType[ct + d._values.Length];
                Array.Copy(d._values, newValues, d._values.Length);
                values[k].CopyTo(newValues, d._values.Length);
                d._values = newValues;
            }

            TypeLookUpDictionary<TType> d0 = dictionaries[0];

            Type[] newTypes = new Type[ct + d0._types.Length];
            Array.Copy(d0._types, 0, newTypes, 0, d0._types.Length);
            types.CopyTo(newTypes, d0._types.Length);

            Type[] typeArgs = [ typeof(TType), null! ];
            for (int i = d0._types.Length; i < newTypes.Length; ++i)
            {
                SetIndex(newTypes[i], i, typeArgs);
            }

            for (int k = 0; k < dictionaries.Length; ++k)
                dictionaries[k]._types = newTypes;
        }
    }

    public static List<TType>?[]? RemoveTypes(TypeLookUpDictionary<TType>[] dictionaries, IList<Type> types)
    {
        if (dictionaries.Length == 0 || types.Count == 0)
            return null;

        List<TType>?[] values = new List<TType>?[dictionaries.Length];
        lock (Sync)
        {
            TypeLookUpDictionary<TType> d0 = dictionaries[0];
            int length = d0._types.Length;
            Type[] typeArgs = [ typeof(TType), null! ];
            for (int i = length - 1; i >= 0; --i)
            {
                Type type = d0._types[i];

                if (!types.Contains(type))
                    continue;

                for (int k = 0; k < dictionaries.Length; ++k)
                    (values[k] ??= new List<TType>()).Add(dictionaries[k]._values[i]);

                SetIndex(type, -1, typeArgs);

                --length;
                if (length - 1 == i)
                {
                    continue;
                }

                for (int j = i; j <= length; ++j)
                {
                    for (int k = 0; k < dictionaries.Length; ++k)
                    {
                        TypeLookUpDictionary<TType> d = dictionaries[k];
                        Type t = d._types[j + 1];
                        d._types[j] = t;
                        d._values[j] = d._values[j + 1];
                    }
                    SetIndex(d0._types[j], j, typeArgs);
                }
            }

            Type[] newTypes = new Type[length];
            Array.Copy(d0._types, newTypes, length);
            for (int k = 0; k < dictionaries.Length; ++k)
            {
                TypeLookUpDictionary<TType> d = dictionaries[k];
                d._types = newTypes;
                TType[] newValues = new TType[length];
                Array.Copy(d._values, newValues, length);
                d._values = newValues;
            }
        }

        return values;
    }

    private static void SetIndex(Type type, int index, Type[] typeArgsTemp)
    {
        typeArgsTemp[1] = type;
        typeof(IndexCache<>)
            .MakeGenericType(typeArgsTemp)
            .GetField(nameof(IndexCache<object>.Index), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)!
            .SetValue(null, index);
    }

    /// <summary>
    /// Retreives the component using a given type.
    /// </summary>
    /// <exception cref="ComponentNotFoundException">Thrown when the component isn't found.</exception>
    public TValueType Get<TValueType, TContext>(TContext context) where TValueType : TType where TContext : notnull
    {
        // ReSharper disable InconsistentlySynchronizedField
        int index = IndexCache<TValueType>.Index;
        return index < 0 || index >= _values.Length
            ? throw new ComponentNotFoundException(typeof(TValueType), context)
            : (TValueType)_values[index];
        // ReSharper restore InconsistentlySynchronizedField
    }

    /// <summary>
    /// Retreives the component using a given type.
    /// </summary>
    /// <exception cref="ComponentNotFoundException">Thrown when the component isn't found.</exception>
    public object Get<TContext>(Type t, TContext context) where TContext : notnull
    {
        lock (Sync)
        {
            for (int i = 0; i < _types.Length; ++i)
            {
                if (_types[i] == t)
                    return _values[i];
            }

            for (int i = 0; i < _types.Length; ++i)
            {
                if (_types[i].IsInstanceOfType(_values[i]))
                    return _values[i];
            }
        }

        throw new ComponentNotFoundException(t, context);
    }

    /// <summary>
    /// Retreives the component using a given type.
    /// </summary>
    public bool TryGet<TValueType>([NotNullWhen(true)] out TValueType? value) where TValueType : TType
    {
        // ReSharper disable InconsistentlySynchronizedField
        int index = IndexCache<TValueType>.Index;
        if (index < 0 || index >= _values.Length)
        {
            value = default;
            return false;
        }

        value = (TValueType)_values[index];
        return true;
        // ReSharper restore InconsistentlySynchronizedField
    }

    /// <summary>
    /// Retreives the component using a given type.
    /// </summary>
    public bool TryGet(Type t, [NotNullWhen(true)] out object? value)
    {
        lock (Sync)
        {
            for (int i = 0; i < _values.Length; ++i)
            {
                if (_types[i] != t)
                    continue;

                value = _values[i];
                return true;
            }

            for (int i = 0; i < _values.Length; ++i)
            {
                if (!_types[i].IsInstanceOfType(_values[i]))
                    continue;

                value = _values[i];
                return true;
            }
        }

        value = null;
        return false;
    }

    // ReSharper disable once UnusedTypeParameter
    private static class IndexCache<TValueType>
    {
        public static int Index = -1;
    }
}

[Serializable]
public class ComponentNotFoundException : Exception
{
    /// <summary>
    /// The type that wasn't able to be located.
    /// </summary>
    public Type? Type { get; }

    public ComponentNotFoundException(Type type, object context)
        : base(string.Format(Properties.Resources.ExceptionComponentNotFound, Accessor.ExceptionFormatter.Format(type), context))
    {
        Type = type;
    }

    protected ComponentNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        string? typeName = info.GetString("Type");
        Type = typeName != null ? Type.GetType(typeName, throwOnError: false) : null;
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Type", Type?.FullName);
        base.GetObjectData(info, context);
    }
}