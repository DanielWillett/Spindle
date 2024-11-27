namespace Rocket.API;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public interface IAsset<T> where T : class
{
    T Instance { get; set; }

    T Save();

    void Load(AssetLoaded<T> callback = null);

    void Unload(AssetUnloaded<T> callback = null);
}