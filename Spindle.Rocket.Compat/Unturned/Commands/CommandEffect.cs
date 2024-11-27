using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class CommandEffect : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "effect";
    public string Help => "Triggers an effect at your position";
    public string Syntax => "<id>";
    public List<string> Permissions => new List<string>(1) { "rocket.effect" };
    public List<string> Aliases => new List<string>();
    public void Execute(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer player = (UnturnedPlayer)caller;
        ushort? id = command.GetUInt16Parameter(0);
        if (!id.HasValue)
        {
            UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
            throw new WrongUsageOfCommandException(caller, this);
        }

        player.TriggerEffect(id.Value);
    }
}