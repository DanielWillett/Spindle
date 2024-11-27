namespace Rocket.Unturned.Permissions;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class UnturnedPermissions : MonoBehaviour
{
    public static event JoinRequested OnJoinRequested;

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void JoinRequested(CSteamID player, ref ESteamRejection? rejectionReason);
}