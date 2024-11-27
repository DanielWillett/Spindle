namespace Rocket.API;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public interface IRocketImplementationEvents
{
    event ImplementationShutdown OnShutdown;
}