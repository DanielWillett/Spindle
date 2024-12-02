using System;

namespace Spindle.Util;
internal static class EnumValidationUtility
{
    public static bool ValidateValidField<TEnum>(TEnum stat) where TEnum : unmanaged, Enum
    {
        return Enum.GetName(typeof(TEnum), stat) != null;
    }
}
