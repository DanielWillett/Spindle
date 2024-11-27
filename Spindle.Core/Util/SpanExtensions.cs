﻿using System;

namespace Spindle.Util;
internal static class SpanExtensions
{
    /// <summary>
    /// Concat 2 spans without allocating extra memory.
    /// </summary>
    public static unsafe string Concat(this ReadOnlySpan<char> span, ReadOnlySpan<char> span2)
    {
        fixed (char* ptr = span)
        fixed (char* ptr2 = span2)
        {
            Concat2SpanState state = default;
            state.Span1Ptr = ptr;
            state.Span1Len = span.Length;
            state.Span2Ptr = ptr2;
            state.Span2Len = span2.Length;
            return string.Create(span.Length + span2.Length, state, (span, state) =>
            {
                new ReadOnlySpan<char>(state.Span1Ptr, state.Span1Len).CopyTo(span);
                new ReadOnlySpan<char>(state.Span2Ptr, state.Span2Len).CopyTo(span[state.Span1Len..]);
            });
        }
    }

    private unsafe struct Concat2SpanState
    {
        public char* Span1Ptr;
        public int Span1Len;
        public char* Span2Ptr;
        public int Span2Len;
    }

    /// <summary>
    /// Concat a span and string without allocating extra memory.
    /// </summary>
    public static unsafe string Concat(this ReadOnlySpan<char> span, string str)
    {
        fixed (char* ptr = span)
        {
            ConcatSpan2StringState state = default;
            state.Span1Ptr = ptr;
            state.Span1Len = span.Length;
            state.String = str;
            return string.Create(span.Length + str.Length, state, (span, state) =>
            {
                new ReadOnlySpan<char>(state.Span1Ptr, state.Span1Len).CopyTo(span);
                state.String.AsSpan().CopyTo(span[state.Span1Len..]);
            });
        }
    }

    private unsafe struct ConcatSpan2StringState
    {
        public char* Span1Ptr;
        public int Span1Len;
        public string String;
    }

    /// <summary>
    /// Gets the index of an object in a span with an offset.
    /// </summary>
    public static int IndexOf<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value, int startIndex) where T : IEquatable<T>
    {
        int index = span[startIndex..].IndexOf(value);
        if (index < 0)
            return -1;
        return index + startIndex;
    }

    /// <summary>
    /// Gets the index of an object in a span with an offset.
    /// </summary>
    public static int IndexOf<T>(this ReadOnlySpan<T> span, T value, int startIndex) where T : IEquatable<T>
    {
        int index = span[startIndex..].IndexOf(value);
        if (index < 0)
            return -1;
        return index + startIndex;
    }

    /// <summary>
    /// Counts the number of occurences of <paramref name="value"/> in <paramref name="span"/>.
    /// </summary>
    public static int Count<T>(
#if !NET8_0_OR_GREATER
        this
#endif
            ReadOnlySpan<T> span, ReadOnlySpan<T> value) where T : IEquatable<T>
    {
        int amt = 0;
        int lastIndex = -value.Length;
        while ((lastIndex = span.IndexOf(value, lastIndex + value.Length)) >= 0)
        {
            ++amt;
            if (lastIndex + value.Length >= span.Length)
                break;
        }

        return amt;
    }

    /// <summary>
    /// Counts the number of occurences of <paramref name="value"/> in <paramref name="span"/>.
    /// </summary>
    public static int Count<T>(
#if !NET8_0_OR_GREATER
        this
#endif
            ReadOnlySpan<T> span, T value) where T : IEquatable<T>
    {
        int amt = 0;
        int lastIndex = -1;
        while ((lastIndex = span.IndexOf(value, lastIndex + 1)) >= 0)
        {
            ++amt;
            if (lastIndex + 1 >= span.Length)
                break;
        }

        return amt;
    }

    /// <summary>
    /// Span implementation of <see cref="string.Split(char,StringSplitOptions)"/>. (this exists but only in .net 8+)
    /// </summary>
    /// <param name="trimOuter">Trim white-space off the input string before splitting.</param>
    /// <param name="trimEachEntry">Trim white-space off each split entry.</param>
    /// <remarks>Will return as many ranges as <paramref name="ranges"/> can fit. To get a maximum, use <c>span.Count(<paramref name="separator"/>) + 1</c>.</remarks>
    public static int Split(this ReadOnlySpan<char> span, Span<Range> ranges, char separator, bool trimOuter = false, bool trimEachEntry = false, StringSplitOptions options = StringSplitOptions.None)
    {
        int startIndex = 0;
        int endIndex = span.Length - 1;

        // higher .NET versions have a StringSplitOptions.TrimEntries = 2 value.
        trimEachEntry |= (options & (StringSplitOptions)2) != 0;

        if (trimOuter)
        {
            for (int i = 0; i < span.Length; ++i)
            {
                if (char.IsWhiteSpace(span[i]))
                    continue;

                startIndex = i;
                break;
            }

            for (int i = span.Length - 1; i >= 0; --i)
            {
                if (char.IsWhiteSpace(span[i]))
                    continue;

                endIndex = i;
                break;
            }
        }

        int rangeInd = -1;
        int startInd, endInd;
        int lastEndInd = startIndex - 1;
        for (int i = startIndex; i <= endIndex; ++i)
        {
            if (span[i] != separator)
                continue;

            startInd = lastEndInd + 1;
            endInd = i - 1;
            lastEndInd = i;

            if (startInd > endInd && (startInd != endInd + 1 || (options & StringSplitOptions.RemoveEmptyEntries) != 0))
                continue;

            if (!trimEachEntry)
            {
                if ((options & StringSplitOptions.RemoveEmptyEntries) != 0 && startInd == endInd + 1)
                    continue;

                if (ranges.Length <= rangeInd + 1)
                    return ranges.Length;

                ranges[++rangeInd] = new Range(new Index(startInd), new Index(endInd + 1));
                continue;
            }

            for (int j = startInd; j <= endInd; ++j)
            {
                if (char.IsWhiteSpace(span[j]))
                    continue;

                startInd = j;
                break;
            }

            for (int j = endInd; j >= startIndex; --j)
            {
                if (char.IsWhiteSpace(span[j]))
                    continue;

                endInd = j;
                break;
            }

            if (startInd > endInd && (startInd != endInd + 1 || (options & StringSplitOptions.RemoveEmptyEntries) != 0))
                continue;

            if (ranges.Length <= rangeInd + 1)
                return ranges.Length;

            ranges[++rangeInd] = new Range(new Index(startInd), new Index(endInd + 1));
        }

        startInd = lastEndInd + 1;
        endInd = endIndex;

        if (!trimEachEntry)
        {
            if ((options & StringSplitOptions.RemoveEmptyEntries) != 0 && startInd == endInd + 1)
                return rangeInd + 1;

            if (ranges.Length <= rangeInd + 1)
                return ranges.Length;

            ranges[++rangeInd] = new Range(new Index(startInd), new Index(endInd + 1));
            return rangeInd + 1;
        }

        for (int j = startInd; j <= endInd; ++j)
        {
            if (char.IsWhiteSpace(span[j]))
                continue;

            startInd = j;
            break;
        }

        for (int j = endInd; j >= startInd; --j)
        {
            if (char.IsWhiteSpace(span[j]))
                continue;

            endInd = j;
            break;
        }

        if (startInd > endInd && (startInd != endInd + 1 || (options & StringSplitOptions.RemoveEmptyEntries) != 0))
            return rangeInd + 1;

        if (ranges.Length <= rangeInd + 1)
            return ranges.Length;

        ranges[++rangeInd] = new Range(new Index(startInd), new Index(endInd + 1));
        return rangeInd + 1;
    }
}
