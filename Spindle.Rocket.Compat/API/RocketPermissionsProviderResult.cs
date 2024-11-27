namespace Rocket.API;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public enum RocketPermissionsProviderResult
{
    Success,
    UnspecifiedError,
    DuplicateEntry,
    GroupNotFound,
    PlayerNotFound,
}