using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Unturned.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class CommandTp : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "tp";
    public string Help => "Teleports you to another player or location";
    public string Syntax => "<player | place | x y z>";
    public List<string> Aliases => new List<string>();
    public List<string> Permissions => new List<string>(2) { "rocket.tp", "rocket.teleport" };

    public void Execute(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer unturnedPlayer = (UnturnedPlayer)caller;
        if (command.Length != 1 && command.Length != 3)
        {
            UnturnedChat.Say(unturnedPlayer, U.Translate("command_generic_invalid_parameter"));
            throw new WrongUsageOfCommandException(caller, this);
        }

        if (unturnedPlayer.Stance is EPlayerStance.DRIVING or EPlayerStance.SITTING)
        {
            UnturnedChat.Say(caller, U.Translate("command_generic_teleport_while_driving_error"));
            throw new WrongUsageOfCommandException(caller, this);
        }

        float? x = null, y = null, z = null;
        if (command.Length == 3)
        {
            x = command.GetFloatParameter(0);
            y = command.GetFloatParameter(1);
            z = command.GetFloatParameter(2);
        }

        if (x.HasValue && y.HasValue && z.HasValue)
        {
            unturnedPlayer.Teleport(new Vector3(x.Value, y.Value, z.Value), unturnedPlayer.Rotation);
            Logger.Log(U.Translate("command_tp_teleport_console", unturnedPlayer.CharacterName, $"{x.Value},{y.Value},{z.Value}"));
            UnturnedChat.Say(unturnedPlayer, U.Translate("command_tp_teleport_private", $"{x.Value},{y.Value},{z.Value}"));
            return;
        }

        UnturnedPlayer target = UnturnedPlayer.FromName(command[0]);
        if (target != null && !target.Equals(unturnedPlayer))
        {
            unturnedPlayer.Teleport(target);
            Logger.Log(U.Translate("command_tp_teleport_console", unturnedPlayer.CharacterName, target.CharacterName));
            UnturnedChat.Say(unturnedPlayer, U.Translate("command_tp_teleport_private", target.CharacterName));
            return;
        }

        LocationDevkitNode node = LocationDevkitNodeSystem
            .Get()
            .GetAllNodes()
            .FirstOrDefault(n => n.name.IndexOf(command[0], StringComparison.InvariantCultureIgnoreCase) >= 0);

        if (node == null)
        {
            UnturnedChat.Say(unturnedPlayer, U.Translate("command_tp_failed_find_destination"));
            return;
        }

        Vector3 position = node.transform.position + new Vector3(0.0f, 0.5f, 0.0f);
        unturnedPlayer.Teleport(position, unturnedPlayer.Rotation);

        Logger.Log(U.Translate("command_tp_teleport_console", unturnedPlayer.CharacterName, node.name));
        UnturnedChat.Say(unturnedPlayer, U.Translate("command_tp_teleport_private", node.name));
    }
}