using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rocket.Unturned.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class CommandP : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "p";
    public string Help => "Sets a Rocket permission group of a specific player";
    public string Syntax => "<player> [group] | reload";
    public List<string> Permissions => new List<string>(2) { "rocket.p", "rocket.permissions" };
    public List<string> Aliases => new List<string>(1) { "permissions" };
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length == 1 && command[0].Equals("reload", StringComparison.InvariantCultureIgnoreCase))
        {
            if (!HasCommandPermission(caller, "reload"))
            {
                UnturnedChat.Say(caller, R.Translate("command_no_permission"));
                throw new WrongUsageOfCommandException(caller, this);
            }

            R.Permissions.Reload();
            UnturnedChat.Say(caller, U.Translate("command_p_permissions_reload"));
            return;
        }
        
        if (command.Length == 0 && caller is not ConsolePlayer && ulong.TryParse(caller.Id, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong steamId) && new CSteamID(steamId).GetEAccountType() == EAccountType.k_EAccountTypeIndividual)
        {
            UnturnedChat.Say(caller, U.Translate("command_p_groups_private", "Your", string.Join(", ", R.Permissions.GetGroups(caller, true).Select(g => g.DisplayName))));
            UnturnedChat.Say(caller, U.Translate("command_p_permissions_private", "Your", string.Join(", ", R.Permissions.GetPermissions(caller).Select(p => p.Name + (p.Cooldown != 0u ? "(" + p.Cooldown + ")" : "")))));
            return;
        }
        
        if (command.Length == 1)
        {
            IRocketPlayer player = (IRocketPlayer)command.GetUnturnedPlayerParameter(0) ?? command.GetRocketPlayerParameter(0);
            UnturnedChat.Say(caller, U.Translate("command_p_groups_private", player.DisplayName + "'s", string.Join(", ", R.Permissions.GetGroups(player, true).Select(g => g.DisplayName))));
            UnturnedChat.Say(caller, U.Translate("command_p_permissions_private", player.DisplayName + "'s", string.Join(", ", R.Permissions.GetPermissions(player).Select(p => p.Name + (p.Cooldown != 0u ? "(" + p.Cooldown + ")" : "")))));
            return;
        }
        
        if (command.Length != 3)
        {
            UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
            throw new WrongUsageOfCommandException(caller, this);
        }

        string operation = command[0];
        IRocketPlayer target = (IRocketPlayer)command.GetUnturnedPlayerParameter(1) ?? command.GetRocketPlayerParameter(1);
        string permissionGroup = command[2];

        if (target != null && !string.IsNullOrWhiteSpace(operation) && !string.IsNullOrWhiteSpace(permissionGroup))
        {
            if (operation.Equals("add", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!HasCommandPermission(caller, "add"))
                {
                    UnturnedChat.Say(caller, R.Translate("command_no_permission"));
                    throw new WrongUsageOfCommandException(caller, this);
                }

                switch (R.Permissions.AddPlayerToGroup(permissionGroup, target))
                {
                    case RocketPermissionsProviderResult.Success:
                        UnturnedChat.Say(caller, U.Translate("command_p_group_player_added", target.DisplayName, permissionGroup));
                        break;
                    case RocketPermissionsProviderResult.DuplicateEntry:
                        UnturnedChat.Say(caller, U.Translate("command_p_duplicate_entry", target.DisplayName, permissionGroup));
                        break;
                    case RocketPermissionsProviderResult.GroupNotFound:
                        UnturnedChat.Say(caller, U.Translate("command_p_group_not_found", target.DisplayName, permissionGroup));
                        break;
                    case RocketPermissionsProviderResult.PlayerNotFound:
                        UnturnedChat.Say(caller, U.Translate("command_p_player_not_found", target.DisplayName, permissionGroup));
                        break;
                    default:
                        UnturnedChat.Say(caller, U.Translate("command_p_unknown_error", target.DisplayName, permissionGroup));
                        break;
                }

                return;
            }
            
            if (operation.Equals("remove", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!HasCommandPermission(caller, "remove"))
                {
                    UnturnedChat.Say(caller, R.Translate("command_no_permission"));
                    throw new WrongUsageOfCommandException(caller, this);
                }

                switch (R.Permissions.RemovePlayerFromGroup(permissionGroup, target))
                {
                    case RocketPermissionsProviderResult.Success:
                        UnturnedChat.Say(caller, U.Translate("command_p_group_player_removed", target.DisplayName, permissionGroup));
                        return;
                    case RocketPermissionsProviderResult.DuplicateEntry:
                        UnturnedChat.Say(caller, U.Translate("command_p_duplicate_entry", target.DisplayName, permissionGroup));
                        return;
                    case RocketPermissionsProviderResult.GroupNotFound:
                        UnturnedChat.Say(caller, U.Translate("command_p_group_not_found", target.DisplayName, permissionGroup));
                        return;
                    case RocketPermissionsProviderResult.PlayerNotFound:
                        UnturnedChat.Say(caller, U.Translate("command_p_player_not_found", target.DisplayName, permissionGroup));
                        return;
                    default:
                        UnturnedChat.Say(caller, U.Translate("command_p_unknown_error", target.DisplayName, permissionGroup));
                        return;
                }
            }
        }

        UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
        throw new WrongUsageOfCommandException(caller, this);
    }

    private static bool HasCommandPermission(IRocketPlayer caller, string perm)
    {
        return caller.HasPermission("p." + perm) || caller.HasPermission("rocket.p." + perm)
            || caller.HasPermission("permissions." + perm) || caller.HasPermission("rocket.permissions." + perm);
    }
}