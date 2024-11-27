namespace Rocket.API;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public delegate void AssetUnloaded<T>(IAsset<T> asset) where T : class;