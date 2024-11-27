using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class CommandVanish : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "vanish";
    public string Help => "Are we rushing in or are we goin' sneaky beaky like?";
    public string Syntax => string.Empty;
    public List<string> Permissions => new List<string>(1) { "rocket.vanish" };
    public List<string> Aliases => new List<string>();
    public void Execute(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer unturnedPlayer = (UnturnedPlayer)caller;
        if (unturnedPlayer.Features.VanishMode)
        {
            unturnedPlayer.Features.VanishMode = false;
            Logger.Log(U.Translate("command_vanish_disable_console", unturnedPlayer.CharacterName));
            UnturnedChat.Say(caller, U.Translate("command_vanish_disable_private"));
        }
        else
        {
            unturnedPlayer.Features.VanishMode = true;
            Logger.Log(U.Translate("command_vanish_enable_console", unturnedPlayer.CharacterName));
            UnturnedChat.Say(caller, U.Translate("command_vanish_enable_private"));
        }
    }
}