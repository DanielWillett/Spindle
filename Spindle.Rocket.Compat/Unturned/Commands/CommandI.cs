using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Items;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Globalization;

namespace Rocket.Unturned.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class CommandI : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "i";
    public string Help => "Gives yourself an item";
    public string Syntax => "<id> [amount]";
    public List<string> Permissions => new List<string>(2) { "rocket.item", "rocket.i" };
    public List<string> Aliases => new List<string>(1) { "item" };
    public void Execute(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer player = (UnturnedPlayer)caller;
        if (command.Length < 1)
        {
            UnturnedChat.Say(player, U.Translate("command_generic_invalid_parameter"));
            throw new WrongUsageOfCommandException(caller, this);
        }
        
        string itemString;
        if (command.Length > 1 && byte.TryParse(command[^1], NumberStyles.Number, CultureInfo.InvariantCulture, out byte amount) && amount > 0)
        {
            itemString = command.GetParameterString(0, command.Length - 1);
        }
        else
        {
            itemString = command.GetParameterString(0);
            amount = 1;
        }

        if (string.IsNullOrEmpty(itemString))
        {
            UnturnedChat.Say(player, U.Translate("command_generic_invalid_parameter"));
            throw new WrongUsageOfCommandException(caller, this);
        }

        ItemAsset itemAsset = UnturnedItems.GetItemAssetByName(itemString);

        if (itemAsset == null)
        {
            UnturnedChat.Say(player, U.Translate("command_generic_invalid_parameter"));
            throw new WrongUsageOfCommandException(caller, this);
        }

        string itemName = itemAsset.itemName;

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

            Logger.Log(U.Translate("command_i_giving_console", player.DisplayName, $"{itemName} ({itemAsset.id})", amount));
            UnturnedChat.Say(player, U.Translate("command_i_giving_private", amount, itemName, itemAsset.id));
        }
    }
}