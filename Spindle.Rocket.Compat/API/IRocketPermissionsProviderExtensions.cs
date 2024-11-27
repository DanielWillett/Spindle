using Rocket.API.Serialisation;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.API;

// ReSharper disable once InconsistentNaming
[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public static class IRocketPermissionsProviderExtensions
{
    public static bool HasPermission(this IRocketPermissionsProvider rocketPermissionProvider, IRocketPlayer player, string permission)
    {
        return rocketPermissionProvider.HasPermission(player, new List<string>
        {
            permission
        });
    }

    public static bool HasPermission(this IRocketPermissionsProvider rocketPermissionProvider, IRocketPlayer player, IRocketCommand command)
    {
        List<string> permissions = command.Permissions;
        permissions.Add(command.Name);
        permissions.AddRange(command.Aliases);
        List<string> list = permissions.Select(a => a.ToLower()).ToList();
        return rocketPermissionProvider.HasPermission(player, list);
    }

    public static List<Permission> GetPermissions(this IRocketPermissionsProvider rocketPermissionProvider, IRocketPlayer player, string permission)
    {
        return rocketPermissionProvider.GetPermissions(player, new List<string>
        {
            permission
        });
    }

    public static List<Permission> GetPermissions(this IRocketPermissionsProvider rocketPermissionProvider, IRocketPlayer player, IRocketCommand command)
    {
        List<string> permissions = command.Permissions;
        permissions.Add(command.Name);
        permissions.AddRange(command.Aliases);
        List<string> list = permissions.Select(a => a.ToLower()).ToList();
        return rocketPermissionProvider.GetPermissions(player, list);
    }
}