using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class CommandCompass : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "compass";
    public string Help => "Shows the direction you are facing";
    public string Syntax => "[direction]";
    public List<string> Permissions => new List<string>(1) { "rocket.compass" };
    public List<string> Aliases => new List<string>();
    public void Execute(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer unturnedPlayer = (UnturnedPlayer)caller;
        float rotation = unturnedPlayer.Rotation;
        if (command.Length == 1)
        {
            if (string.Equals(command[0], "north", StringComparison.InvariantCultureIgnoreCase))
            {
                rotation = 0f;
            }
            else if (string.Equals(command[0], "east", StringComparison.InvariantCultureIgnoreCase))
            {
                rotation = 90f;
            }
            else if (string.Equals(command[0], "south", StringComparison.InvariantCultureIgnoreCase))
            {
                rotation = 180f;
            }
            else if (string.Equals(command[0], "west", StringComparison.InvariantCultureIgnoreCase))
            {
                rotation = 270f;
            }
            else
            {
                UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }

            unturnedPlayer.Teleport(unturnedPlayer.Position, rotation);
        }

        string str = rotation switch
        {
            > 30f  and < 60f  => U.Translate("command_compass_northeast"),
            > 60f  and < 120f => U.Translate("command_compass_east"),
            > 120f and < 150f => U.Translate("command_compass_southeast"),
            > 150f and < 210f => U.Translate("command_compass_south"),
            > 210f and < 240f => U.Translate("command_compass_southwest"),
            > 240f and < 300f => U.Translate("command_compass_west"),
            > 300f and < 330f => U.Translate("command_compass_northwest"),
            _ => U.Translate("command_compass_north")
        };

        UnturnedChat.Say(caller, U.Translate("command_compass_facing_private", str));
    }
}