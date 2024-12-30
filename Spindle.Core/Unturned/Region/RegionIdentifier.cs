using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Spindle.Util;

namespace Spindle.Unturned.Region;

[Serializable]
public readonly struct RegionIdentifier :
    IEquatable<RegionIdentifier>,
    IComparable<RegionIdentifier>,
    IFormattable
{
    public static readonly RegionIdentifier Invalid;
    static RegionIdentifier()
    {
        int num = -1;
        Invalid = Unsafe.As<int, RegionIdentifier>(ref num);
    }

    [SerializeField]
    private readonly int _data;

    /// <summary>
    /// The X coordinate of the region.
    /// </summary>
    public byte X => (byte)(_data >> 24);

    /// <summary>
    /// The Y coordinate of the region.
    /// </summary>
    public byte Y => (byte)(_data >> 16);

    /// <summary>
    /// The index of this object in the region.
    /// </summary>
    public ushort Index => (ushort)_data;

    public bool IsInvalid => _data == -1;

    /// <summary>
    /// Raw integer packing X, Y, and Index into 4 bytes.
    /// <code>
    /// X = (byte)(Raw &gt;&gt; 24)
    /// Y = (byte)(Raw &gt;&gt; 16)
    /// Index = (ushort)Raw
    /// Hi [ X ][ Y ][ Index ] Lo
    /// </code>
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public int Raw => _data;

    internal RegionIdentifier(int data)
    {
        _data = data;
    }

    public RegionIdentifier(RegionCoord coord, ushort index) : this((coord.x << 24) | (coord.y << 16) | index) { }
    public RegionIdentifier(RegionCoord coord, int index) : this((coord.x << 24) | (coord.y << 16) | (ushort)index) { }
    public RegionIdentifier(RegionCoordinate coord, ushort index) : this((coord.x << 24) | (coord.y << 16) | index) { }
    public RegionIdentifier(RegionCoordinate coord, int index) : this((coord.x << 24) | (coord.y << 16) | (ushort)index) { }
    public RegionIdentifier(byte x, byte y, ushort index) : this((x << 24) | (y << 16) | index) { }
    public RegionIdentifier(int x, int y, int index) : this(((byte)x << 24) | ((byte)y << 16) | (ushort)index) { }

    /// <summary>
    /// Create an itendifier from a raw integer packing X, Y, and Index into 4 bytes.
    /// <code>
    /// X = (byte)(Raw &gt;&gt; 24)
    /// Y = (byte)(Raw &gt;&gt; 16)
    /// Index = (ushort)Raw
    /// Hi [ X ][ Y ][ Index ] Lo
    /// </code>
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static unsafe RegionIdentifier CreateUnsafe(int raw) => *(RegionIdentifier*)&raw;
    
    public bool IsSameRegionAs(RegionIdentifier other) => (other._data & unchecked((int)0xFFFF0000)) == (_data & unchecked((int)0xFFFF0000));
    public bool IsSameRegionAs(byte x, byte y) => X == x && Y == y;
    public override bool Equals(object? other) => other is RegionIdentifier id && Equals(id);
    public bool Equals(RegionIdentifier other) => other._data == _data;
    public override int GetHashCode() => _data;
    public static bool operator ==(RegionIdentifier left, RegionIdentifier right) => left._data == right._data;
    public static bool operator !=(RegionIdentifier left, RegionIdentifier right) => left._data != right._data;
    public static bool operator <(RegionIdentifier left, RegionIdentifier right) => left._data < right._data;
    public static bool operator >(RegionIdentifier left, RegionIdentifier right) => left._data > right._data;
    public static bool operator <=(RegionIdentifier left, RegionIdentifier right) => left._data <= right._data;
    public static bool operator >=(RegionIdentifier left, RegionIdentifier right) => left._data >= right._data;
    /// <exception cref="OverflowException"/>
    public static RegionIdentifier operator +(RegionIdentifier obj, int amt)
    {
        if (checked((obj._data & 0xFFFF) + amt) <= ushort.MaxValue)
            return new RegionIdentifier(obj._data + amt);
        throw new OverflowException("Index can not go over " + ushort.MaxValue + ".");
    }
    /// <exception cref="OverflowException"/>
    public static RegionIdentifier operator -(RegionIdentifier obj, int amt)
    {
        if (checked((obj._data & 0xFFFF) - amt) >= 0)
            return new RegionIdentifier(obj._data - amt);
        throw new OverflowException("Index can not go under " + ushort.MinValue + ".");
    }

    public int CompareTo(RegionIdentifier other) => other._data.CompareTo(_data);
    public override string ToString() => ToString(null, null);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        formatProvider ??= CultureInfo.CurrentCulture;

        int data = _data;
        int x = (data >> 24) & 0xFF,
            y = (data >> 16) & 0xFF;
        int index = data & 0xFFFF;
        if (format == null)
        {
            int l = 5 + FormattingUtility.CountDigits(x) + FormattingUtility.CountDigits(y) +
                    FormattingUtility.CountDigits(index);
            return string.Create(l, this, static (span, state) =>
            {
                int data = state._data;
                int x = (data >> 24) & 0xFF,
                    y = (data >> 16) & 0xFF;
                int index = data & 0xFFFF;
                span[0] = '(';
                CultureInfo fmt = CultureInfo.InvariantCulture;
                x.TryFormat(span[1..], out int ind, provider: fmt);
                ++ind;
                span[ind] = ',';
                ++ind;
                y.TryFormat(span[ind..], out int amt2, provider: fmt);
                ind += amt2;
                span[ind] = ')';
                span[++ind] = ' ';
                span[++ind] = '#';
                span[++ind] = ' ';
                ++ind;
                index.TryFormat(span[ind..], out _, provider: fmt);
            });
        }

        string xStr = x.ToString(format, formatProvider);
        string yStr = y.ToString(format, formatProvider);
        string indexStr = index.ToString(format, formatProvider);
        return string.Create(5 + xStr.Length + yStr.Length + indexStr.Length, new ValueTuple<string, string, string>(xStr, yStr, indexStr), static (span, state) =>
        {
            span[0] = '(';
            state.Item1.AsSpan().CopyTo(span[1..]);
            int ind = 1 + state.Item1.Length;
            span[ind] = ',';
            ++ind;
            state.Item2.AsSpan().CopyTo(span[ind..]);
            ind += state.Item2.Length;
            span[ind] = ')';
            span[++ind] = ' ';
            span[++ind] = '#';
            span[++ind] = ' ';
            ++ind;
            state.Item3.AsSpan().CopyTo(span[ind..]);
        });
    }

    /// <summary>
    /// Get the element from a region list array (<see cref="Regions.WORLD_SIZE"/> sqr) without bounds checks.
    /// </summary>
    /// <remarks>Use <see cref="CheckSafe{T}"/> for bounds checks.</remarks>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public T FromList<T>(List<T>[,] regionList)
    {
        int data = _data;
        return regionList[(data >> 24) & 0xFF, (data >> 16) & 0xFF][data & 0xFFFF];
    }

    /// <summary>
    /// Get the list from a region list array (<see cref="Regions.WORLD_SIZE"/> sqr) without bounds checks.
    /// </summary>
    /// <remarks>Use <see cref="CheckSafe{T}"/> for bounds checks.</remarks>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public List<T> GetList<T>(List<T>[,] regionList)
    {
        int data = _data;
        return regionList[(data >> 24) & 0xFF, (data >> 16) & 0xFF];
    }

    /// <summary>
    /// Check if the region coordinates of this identifier are a valid region.
    /// </summary>
    public bool CheckSafe()
    {
        int wrldSize = Regions.WORLD_SIZE;
        return (byte)(_data >> 24) < wrldSize && (byte)(_data >> 16) < wrldSize;
    }

    /// <summary>
    /// Check if this <see cref="RegionIdentifier"/> exists in the given region list array (<see cref="Regions.WORLD_SIZE"/> sqr).
    /// </summary>
    /// <remarks>Use <see cref="FromList{T}"/> to actually get the item.</remarks>
    public bool CheckSafe<T>(List<T>[,] regionList)
    {
        int wrldSize = Regions.WORLD_SIZE;
        int data = _data;
        int x = (data >> 24) & 0xFF,
            y = (data >> 16) & 0xFF;
        return x < wrldSize && y < wrldSize && (data & 0xFFFF) < regionList[x, y].Count;
    }

    /// <summary>
    /// Check if this <see cref="RegionIdentifier"/> exists in the given region list array (<see cref="Regions.WORLD_SIZE"/> sqr) and output the element at this region and index.
    /// </summary>
    public bool TryFromList<T>(List<T>[,] regionList, out T element)
    {
        int wrldSize = Regions.WORLD_SIZE;
        int data = _data;
        int x = (data >> 24) & 0xFF,
            y = (data >> 16) & 0xFF,
            index = data & 0xFFFF;

        if (x < wrldSize && y < wrldSize && index < regionList[x, y].Count)
        {
            element = regionList[x, y][index];
            return true;
        }

        element = default!;
        return false;
    }

}