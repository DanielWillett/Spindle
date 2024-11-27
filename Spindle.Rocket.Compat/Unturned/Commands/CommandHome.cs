using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class CommandHome : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "home";
    public string Help => "Teleports you to your last bed";
    public string Syntax => string.Empty;
    public List<string> Permissions => new List<string>(1) { "rocket.home" };
    public List<string> Aliases => new List<string>();
    public void Execute(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer unturnedPlayer = (UnturnedPlayer)caller;

        if (!BarricadeManager.tryGetBed(unturnedPlayer.CSteamID, out Vector3 vector3, out byte num))
        {
            UnturnedChat.Say(caller, U.Translate("command_bed_no_bed_found_private"));
            throw new WrongUsageOfCommandException(caller, this);
        }

        if (unturnedPlayer.Stance is EPlayerStance.DRIVING or EPlayerStance.SITTING)
        {
            UnturnedChat.Say(caller, U.Translate("command_generic_teleport_while_driving_error"));
            throw new WrongUsageOfCommandException(caller, this);
        }

        vector3.y += 0.5f;
        if (!unturnedPlayer.Player.teleportToLocation(vector3, num))
        {
            UnturnedChat.Say(caller, U.Translate("command_bed_obstructed"));
        }
    }
}