using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Globalization;

namespace Rocket.Unturned.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class CommandMore : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "more";
    public string Help => "Gives more of an item that you have in your hands";
    public string Syntax => "<amount>";
    public List<string> Permissions => new List<string>(1) { "rocket.more" };
    public List<string> Aliases => new List<string>();
    public void Execute(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer player = (UnturnedPlayer)caller;
        if (command.Length != 1 || !byte.TryParse(command[0], NumberStyles.Number, CultureInfo.InvariantCulture, out byte amount) || amount <= 0)
        {
            UnturnedChat.Say(player, U.Translate("command_more_usage"));
            throw new WrongUsageOfCommandException(player, this);
        }

        ItemAsset itemAsset = player.Player.equipment.asset;

        if (itemAsset == null)
        {
            UnturnedChat.Say(caller, U.Translate("command_more_dequipped"));
            return;
        }

        if (U.Settings.Instance.EnableItemBlacklist
            && !player.HasPermission("itemblacklist.bypass")
            && player.HasPermission("item." + itemAsset.id))
        {
            UnturnedChat.Say(player, U.Translate("command_i_blacklisted"));
        }
        else if (U.Settings.Instance.EnableItemSpawnLimit
                 && !player.HasPermission("itemspawnlimit.bypass")
                 && amount > U.Settings.Instance.MaxSpawnAmount)
        {
            UnturnedChat.Say(player, U.Translate("command_i_too_much", U.Settings.Instance.MaxSpawnAmount));
        }
        else
        {
            for (int index = 0; index < amount; ++index)
            {
                Item obj = new Item(itemAsset.id, itemAsset.amount, 100, itemAsset.getState(EItemOrigin.ADMIN));
                player.Player.inventory.forceAddItem(obj, true, index == 0);
            }

            Logger.Log(U.Translate("command_i_giving_console", player.DisplayName, $"{itemAsset.itemName} ({itemAsset.id})", amount));
            UnturnedChat.Say(caller, U.Translate("command_more_give", amount, itemAsset.id));
        }
    }
}