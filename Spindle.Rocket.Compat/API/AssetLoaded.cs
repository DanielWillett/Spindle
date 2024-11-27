namespace Rocket.API;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public delegate void AssetLoaded<T>(IAsset<T> asset) where T : class;