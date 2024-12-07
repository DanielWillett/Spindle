using System;

namespace Spindle.Util;
internal static class EnumValidationUtility
{
    public static bool ValidateValidField<TEnum>(TEnum stat) where TEnum : unmanaged, Enum
    {
        return Enum.GetName(typeof(TEnum), stat) != null;
    }

    /// <summary>
    /// Gets the highest numerical enum value in the given enum type.
    /// </summary>
    public static TEnum GetMaximumValue<TEnum>() where TEnum : unmanaged, Enum
    {
        TEnum[] values = (TEnum[])Enum.GetValues(typeof(TEnum));
        if (values.Length == 0)
            return default;

        // this is jitted away
        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(sbyte))
        {
            sbyte max = Unsafe.As<TEnum, sbyte>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                sbyte v = Unsafe.As<TEnum, sbyte>(ref values[i]);
                if (v > max)
                    max = v;
            }

            return Unsafe.As<sbyte, TEnum>(ref max);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(byte))
        {
            byte max = Unsafe.As<TEnum, byte>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                byte v = Unsafe.As<TEnum, byte>(ref values[i]);
                if (v > max)
                    max = v;
            }

            return Unsafe.As<byte, TEnum>(ref max);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(short))
        {
            short max = Unsafe.As<TEnum, short>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                short v = Unsafe.As<TEnum, short>(ref values[i]);
                if (v > max)
                    max = v;
            }

            return Unsafe.As<short, TEnum>(ref max);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(ushort))
        {
            ushort max = Unsafe.As<TEnum, ushort>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                ushort v = Unsafe.As<TEnum, ushort>(ref values[i]);
                if (v > max)
                    max = v;
            }

            return Unsafe.As<ushort, TEnum>(ref max);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(int))
        {
            int max = Unsafe.As<TEnum, int>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                int v = Unsafe.As<TEnum, int>(ref values[i]);
                if (v > max)
                    max = v;
            }

            return Unsafe.As<int, TEnum>(ref max);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(uint))
        {
            uint max = Unsafe.As<TEnum, uint>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                uint v = Unsafe.As<TEnum, uint>(ref values[i]);
                if (v > max)
                    max = v;
            }

            return Unsafe.As<uint, TEnum>(ref max);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(long))
        {
            long max = Unsafe.As<TEnum, long>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                long v = Unsafe.As<TEnum, long>(ref values[i]);
                if (v > max)
                    max = v;
            }

            return Unsafe.As<long, TEnum>(ref max);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(ulong))
        {
            ulong max = Unsafe.As<TEnum, ulong>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                ulong v = Unsafe.As<TEnum, ulong>(ref values[i]);
                if (v > max)
                    max = v;
            }

            return Unsafe.As<ulong, TEnum>(ref max);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(nint))
        {
            nint max = Unsafe.As<TEnum, nint>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                nint v = Unsafe.As<TEnum, nint>(ref values[i]);
                if (v > max)
                    max = v;
            }

            return Unsafe.As<nint, TEnum>(ref max);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(nuint))
        {
            nuint max = Unsafe.As<TEnum, nuint>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                nuint v = Unsafe.As<TEnum, nuint>(ref values[i]);
                if (v > max)
                    max = v;
            }

            return Unsafe.As<nuint, TEnum>(ref max);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(float))
        {
            float max = Unsafe.As<TEnum, float>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                float v = Unsafe.As<TEnum, float>(ref values[i]);
                if (v > max)
                    max = v;
            }

            return Unsafe.As<float, TEnum>(ref max);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(double))
        {
            double max = Unsafe.As<TEnum, double>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                double v = Unsafe.As<TEnum, double>(ref values[i]);
                if (v > max)
                    max = v;
            }

            return Unsafe.As<double, TEnum>(ref max);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(bool))
        {
            bool max = Unsafe.As<TEnum, bool>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                bool v = Unsafe.As<TEnum, bool>(ref values[i]);
                if (!v)
                    continue;

                max = v;
                break;
            }

            return Unsafe.As<bool, TEnum>(ref max);
        }

        return default;
    }

    /// <summary>
    /// Gets the lowest numerical enum value in the given enum type.
    /// </summary>
    public static TEnum GetMinimumValue<TEnum>() where TEnum : unmanaged, Enum
    {
        TEnum[] values = (TEnum[])Enum.GetValues(typeof(TEnum));
        if (values.Length == 0)
            return default;

        // this is jitted away
        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(sbyte))
        {
            sbyte min = Unsafe.As<TEnum, sbyte>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                sbyte v = Unsafe.As<TEnum, sbyte>(ref values[i]);
                if (v < min)
                    min = v;
            }

            return Unsafe.As<sbyte, TEnum>(ref min);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(byte))
        {
            byte min = Unsafe.As<TEnum, byte>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                byte v = Unsafe.As<TEnum, byte>(ref values[i]);
                if (v < min)
                    min = v;
            }

            return Unsafe.As<byte, TEnum>(ref min);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(short))
        {
            short min = Unsafe.As<TEnum, short>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                short v = Unsafe.As<TEnum, short>(ref values[i]);
                if (v < min)
                    min = v;
            }

            return Unsafe.As<short, TEnum>(ref min);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(ushort))
        {
            ushort min = Unsafe.As<TEnum, ushort>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                ushort v = Unsafe.As<TEnum, ushort>(ref values[i]);
                if (v < min)
                    min = v;
            }

            return Unsafe.As<ushort, TEnum>(ref min);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(int))
        {
            int min = Unsafe.As<TEnum, int>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                int v = Unsafe.As<TEnum, int>(ref values[i]);
                if (v < min)
                    min = v;
            }

            return Unsafe.As<int, TEnum>(ref min);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(uint))
        {
            uint min = Unsafe.As<TEnum, uint>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                uint v = Unsafe.As<TEnum, uint>(ref values[i]);
                if (v < min)
                    min = v;
            }

            return Unsafe.As<uint, TEnum>(ref min);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(long))
        {
            long min = Unsafe.As<TEnum, long>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                long v = Unsafe.As<TEnum, long>(ref values[i]);
                if (v < min)
                    min = v;
            }

            return Unsafe.As<long, TEnum>(ref min);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(ulong))
        {
            ulong min = Unsafe.As<TEnum, ulong>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                ulong v = Unsafe.As<TEnum, ulong>(ref values[i]);
                if (v < min)
                    min = v;
            }

            return Unsafe.As<ulong, TEnum>(ref min);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(nint))
        {
            nint min = Unsafe.As<TEnum, nint>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                nint v = Unsafe.As<TEnum, nint>(ref values[i]);
                if (v < min)
                    min = v;
            }

            return Unsafe.As<nint, TEnum>(ref min);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(nuint))
        {
            nuint min = Unsafe.As<TEnum, nuint>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                nuint v = Unsafe.As<TEnum, nuint>(ref values[i]);
                if (v < min)
                    min = v;
            }

            return Unsafe.As<nuint, TEnum>(ref min);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(float))
        {
            float min = Unsafe.As<TEnum, float>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                float v = Unsafe.As<TEnum, float>(ref values[i]);
                if (v < min)
                    min = v;
            }

            return Unsafe.As<float, TEnum>(ref min);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(double))
        {
            double min = Unsafe.As<TEnum, double>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                double v = Unsafe.As<TEnum, double>(ref values[i]);
                if (v < min)
                    min = v;
            }

            return Unsafe.As<double, TEnum>(ref min);
        }

        if (typeof(TEnum).GetEnumUnderlyingType() == typeof(bool))
        {
            bool min = Unsafe.As<TEnum, bool>(ref values[0]);
            for (int i = 1; i < values.Length; ++i)
            {
                bool v = Unsafe.As<TEnum, bool>(ref values[i]);
                if (v)
                    continue;

                min = v;
                break;
            }

            return Unsafe.As<bool, TEnum>(ref min);
        }

        return default;
    }
}
