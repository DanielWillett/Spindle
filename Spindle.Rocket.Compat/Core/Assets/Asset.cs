using Rocket.API;

namespace Rocket.Core.Assets;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public class Asset<T> : IAsset<T> where T : class
{
    protected T instance;

    public T Instance
    {
        get
        {
            if ((object)this.instance == null)
                this.Load(null);
            return this.instance;
        }
        set
        {
            if ((object)value == null)
                return;
            this.instance = value;
            this.Save();
        }
    }

    public virtual T Save() => this.instance;

    public virtual void Load(AssetLoaded<T> callback = null) => callback(this);

    public virtual void Unload(AssetUnloaded<T> callback = null) => callback(this);
}
