using System.Globalization;

namespace Rocket.Core.Steam;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public static class Steam
{
    // ReSharper disable once InconsistentNaming
    public static bool IsValidCSteamID(string CSteamID)
    {
        return ulong.TryParse(CSteamID, NumberStyles.Any, CultureInfo.InvariantCulture, out ulong result)
               && new CSteamID(result).GetEAccountType() == EAccountType.k_EAccountTypeIndividual;
    }
}