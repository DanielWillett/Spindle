using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class CommandHeal : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "heal";
    public string Help => "Heals yourself or somebody else";
    public string Syntax => "[player]";
    public List<string> Permissions => new List<string>(1) { "rocket.heal" };
    public List<string> Aliases => new List<string>();
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (caller is UnturnedPlayer pl && command.Length == 0)
        {
            pl.Player.life.sendRevive();
            UnturnedChat.Say(pl, U.Translate("command_heal_success"));
            return;
        }

        UnturnedPlayer target = command.GetUnturnedPlayerParameter(0);
        if (target == null)
        {
            UnturnedChat.Say(caller, U.Translate("command_generic_target_player_not_found"));
            throw new WrongUsageOfCommandException(caller, this);
        }

        target.Player.life.sendRevive();
        if (caller.Equals(target))
        {
            UnturnedChat.Say(caller, U.Translate("command_heal_success_me", target.CharacterName));
            UnturnedChat.Say(target, U.Translate("command_heal_success_other", caller.DisplayName));
        }
        else
        {
            UnturnedChat.Say(caller, U.Translate("command_heal_success"));
        }
    }
}