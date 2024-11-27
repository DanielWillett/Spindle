using Rocket.API;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Events;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public sealed class UnturnedEvents : MonoBehaviour, IRocketImplementationEvents
{
    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerDisconnected(UnturnedPlayer player);
    
    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void OnPlayerGetDamage(UnturnedPlayer player, ref EDeathCause cause, ref ELimb limb, ref UnturnedPlayer killer, ref Vector3 direction, ref float damage, ref float times, ref bool canDamage);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerConnected(UnturnedPlayer player);

    // todo invoke
    public static event OnPlayerGetDamage OnPlayerDamaged;
    public event PlayerDisconnected OnPlayerDisconnected;
    public event ImplementationShutdown OnShutdown;
    public event PlayerConnected OnPlayerConnected;
    public event PlayerConnected OnBeforePlayerConnected;
}