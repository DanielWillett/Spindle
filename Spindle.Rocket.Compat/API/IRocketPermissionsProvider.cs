using Rocket.API.Serialisation;
using System.Collections.Generic;

namespace Rocket.API;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public interface IRocketPermissionsProvider
{
    bool HasPermission(IRocketPlayer player, List<string> requestedPermissions);

    List<RocketPermissionsGroup> GetGroups(IRocketPlayer player, bool includeParentGroups);

    List<Permission> GetPermissions(IRocketPlayer player);

    List<Permission> GetPermissions(IRocketPlayer player, List<string> requestedPermissions);

    RocketPermissionsProviderResult AddPlayerToGroup(string groupId, IRocketPlayer player);

    RocketPermissionsProviderResult RemovePlayerFromGroup(string groupId, IRocketPlayer player);

    RocketPermissionsGroup GetGroup(string groupId);

    RocketPermissionsProviderResult AddGroup(RocketPermissionsGroup group);

    RocketPermissionsProviderResult SaveGroup(RocketPermissionsGroup group);

    RocketPermissionsProviderResult DeleteGroup(string groupId);

    void Reload();
}