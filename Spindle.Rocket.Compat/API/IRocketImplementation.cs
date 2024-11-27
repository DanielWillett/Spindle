namespace Rocket.API;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public interface IRocketImplementation
{
    event RocketImplementationInitialized OnRocketImplementationInitialized;

    IRocketImplementationEvents ImplementationEvents { get; }

    void Shutdown();

    string InstanceId { get; }

    void Reload();
}