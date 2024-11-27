using Rocket.Unturned.Player;

namespace Rocket.Unturned.Extensions;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public static class SteamPlayerExtension
{
    public static UnturnedPlayer ToUnturnedPlayer(this SteamPlayer player)
    {
        return UnturnedPlayer.FromSteamPlayer(player);
    }
}