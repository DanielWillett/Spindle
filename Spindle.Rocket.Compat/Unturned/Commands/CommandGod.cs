using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class CommandGod : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "god";
    public string Help => "Cause you ain't givin a shit";
    public string Syntax => string.Empty;
    public List<string> Permissions => new List<string>(1) { "rocket.god" };
    public List<string> Aliases => new List<string>();
    public void Execute(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer unturnedPlayer = (UnturnedPlayer)caller;
        if (unturnedPlayer.Features.GodMode)
        {
            Logger.Log(U.Translate("command_god_disable_console", unturnedPlayer.CharacterName));
            UnturnedChat.Say(caller, U.Translate("command_god_disable_private"));
            unturnedPlayer.Features.GodMode = false;
        }
        else
        {
            Logger.Log(U.Translate("command_god_enable_console", unturnedPlayer.CharacterName));
            UnturnedChat.Say(caller, U.Translate("command_god_enable_private"));
            unturnedPlayer.Features.GodMode = true;
        }
    }
}