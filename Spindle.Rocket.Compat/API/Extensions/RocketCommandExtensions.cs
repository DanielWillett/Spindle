using System;

namespace Rocket.API.Extensions;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public static class RocketCommandExtensions
{
    public static string GetStringParameter(this string[] array, int index)
    {
        return array.Length > index && !string.IsNullOrEmpty(array[index]) ? array[index] : null;
    }

    public static int? GetInt32Parameter(this string[] array, int index)
    {
        return array.Length > index && int.TryParse(array[index], out int result) ? result : null;
    }

    public static uint? GetUInt32Parameter(this string[] array, int index)
    {
        return array.Length > index && uint.TryParse(array[index], out uint result) ? result : null;
    }

    public static byte? GetByteParameter(this string[] array, int index)
    {
        return array.Length > index && byte.TryParse(array[index], out byte result) ? result : null;
    }

    public static ushort? GetUInt16Parameter(this string[] array, int index)
    {
        return array.Length > index && ushort.TryParse(array[index], out ushort result) ? result : null;
    }

    public static float? GetFloatParameter(this string[] array, int index)
    {
        return array.Length > index && float.TryParse(array[index], out float result) ? result : null;
    }

    public static string GetParameterString(this string[] array, int startingIndex = 0)
    {
        return GetParameterString(array, startingIndex, array.Length - startingIndex);
    }
    public static string GetParameterString(this string[] array, int startingIndex, int length)
    {
        length = Math.Min(length, array.Length - startingIndex);
        return array.Length - startingIndex <= 0 ? null : string.Join(" ", new ArraySegment<string>(array, startingIndex, length));
    }
}