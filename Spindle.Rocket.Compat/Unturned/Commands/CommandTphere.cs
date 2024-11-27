using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
internal class CommandTphere : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "tphere";
    public string Help => "Teleports another player to you";
    public string Syntax => "<player>";
    public List<string> Aliases => new List<string>();
    public List<string> Permissions => new List<string>(2) { "rocket.tphere", "rocket.teleporthere" };

    public void Execute(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer unturnedPlayer = (UnturnedPlayer)caller;
        if (command.Length != 1)
        {
            UnturnedChat.Say(unturnedPlayer, U.Translate("command_generic_invalid_parameter"));
            throw new WrongUsageOfCommandException(unturnedPlayer, this);
        }

        UnturnedPlayer player = UnturnedPlayer.FromName(command[0]);

        if (player == null || unturnedPlayer.Equals(player))
        {
            UnturnedChat.Say(unturnedPlayer, U.Translate("command_generic_failed_find_player"));
            throw new WrongUsageOfCommandException(unturnedPlayer, this);
        }

        if (player.Stance is EPlayerStance.DRIVING or EPlayerStance.SITTING || player.IsInVehicle)
        {
            UnturnedChat.Say(unturnedPlayer, U.Translate("command_tphere_vehicle"));
            throw new WrongUsageOfCommandException(unturnedPlayer, this);
        }

        player.Teleport(unturnedPlayer);

        Logger.Log(U.Translate("command_tphere_teleport_console", player.CharacterName, unturnedPlayer.CharacterName));
        UnturnedChat.Say(unturnedPlayer, U.Translate("command_tphere_teleport_from_private", player.CharacterName));
        UnturnedChat.Say(player, U.Translate("command_tphere_teleport_to_private", unturnedPlayer.CharacterName));
    }
}