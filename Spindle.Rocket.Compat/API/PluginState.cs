namespace Rocket.API;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public enum PluginState
{
    Loaded,
    Unloaded,
    Failure,
    Cancelled,
}