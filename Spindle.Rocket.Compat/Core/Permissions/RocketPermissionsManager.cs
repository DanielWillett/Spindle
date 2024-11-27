using Rocket.API;
using Rocket.API.Serialisation;
using System.Collections.Generic;
using Rocket.Core.Assets;

namespace Rocket.Core.Permissions;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public sealed class RocketPermissionsManager : MonoBehaviour, IRocketPermissionsProvider
{
    private IRocketPermissionsProvider helper;
    internal Asset<RocketPermissions> permissions;

    public void Reload()
    {
        permissions.Load(null);
    }

    public bool HasPermission(IRocketPlayer player, List<string> permissions)
    {
        return helper.HasPermission(player, permissions);
    }

    public List<RocketPermissionsGroup> GetGroups(IRocketPlayer player, bool includeParentGroups)
    {
        return helper.GetGroups(player, includeParentGroups);
    }

    public List<Permission> GetPermissions(IRocketPlayer player)
    {
        return helper.GetPermissions(player);
    }

    public List<Permission> GetPermissions(IRocketPlayer player, List<string> requestedPermissions)
    {
        return helper.GetPermissions(player, requestedPermissions);
    }

    public RocketPermissionsProviderResult AddPlayerToGroup(string groupId, IRocketPlayer player)
    {
        return helper.AddPlayerToGroup(groupId, player);
    }

    public RocketPermissionsProviderResult RemovePlayerFromGroup(string groupId, IRocketPlayer player)
    {
        return helper.RemovePlayerFromGroup(groupId, player);
    }

    public RocketPermissionsGroup GetGroup(string groupId)
    {
        return helper.GetGroup(groupId);
    }

    public RocketPermissionsProviderResult SaveGroup(RocketPermissionsGroup group)
    {
        return helper.SaveGroup(group);
    }

    public RocketPermissionsProviderResult AddGroup(RocketPermissionsGroup group)
    {
        return helper.AddGroup(group);
    }

    public RocketPermissionsProviderResult DeleteGroup(RocketPermissionsGroup group)
    {
        return helper.DeleteGroup(group.Id);
    }

    public RocketPermissionsProviderResult DeleteGroup(string groupId)
    {
        return helper.DeleteGroup(groupId);
    }
}
