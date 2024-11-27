using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class CommandExit : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "exit";
    public string Help => "Exit the game without cooldown";
    public string Syntax => string.Empty;
    public List<string> Permissions => new List<string>(1) { "rocket.exit" };
    public List<string> Aliases => new List<string>();
    public void Execute(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer player = (UnturnedPlayer)caller;
        player.Kick("you exited");
    }
}