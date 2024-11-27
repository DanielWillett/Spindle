using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Globalization;

namespace Rocket.Unturned.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public static class UnturnedCommandExtensions
{
    public static UnturnedPlayer GetUnturnedPlayerParameter(this string[] array, int index)
    {
        return array.Length > index ? UnturnedPlayer.FromName(array[index]) : null;
    }

    public static RocketPlayer GetRocketPlayerParameter(this string[] array, int index)
    {
        if (array.Length <= index || array[index] is not { Length: > 0 } name)
            return null;

        if (ulong.TryParse(name, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong steam64)
            && new CSteamID(steam64).GetEAccountType() == EAccountType.k_EAccountTypeIndividual)
        {
            return new RocketPlayer(steam64.ToString(CultureInfo.InvariantCulture));
        }

        return null;
    }

    public static ulong? GetCSteamIDParameter(this string[] array, int index)
    {
        if (array.Length <= index || array[index] is not { Length: > 0 } name)
            return null;

        if (ulong.TryParse(name, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong steam64)
            && new CSteamID(steam64).GetEAccountType() == EAccountType.k_EAccountTypeIndividual)
        {
            return steam64;
        }

        return null;
    }

    public static Color? GetColorParameter(this string[] array, int index)
    {
        if (array.Length <= index || array[index] is not { Length: > 0 } name)
            return null;

        Color colorFromName = UnturnedChat.GetColorFromName(name, Color.clear);
        return colorFromName == Color.clear ? null : colorFromName;
    }
}