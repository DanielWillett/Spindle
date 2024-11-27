namespace Rocket.API;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public interface IRocketPlugin<TConfiguration> : IRocketPlugin where TConfiguration : class
{
    IAsset<TConfiguration> Configuration { get; }
}