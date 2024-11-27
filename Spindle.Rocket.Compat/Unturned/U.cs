using Rocket.API;
using Rocket.API.Collections;
using Rocket.API.Extensions;
using Rocket.Core;
using Rocket.Core.Assets;
using Rocket.Unturned.Events;
using Rocket.Unturned.Serialisation;
using SDG.Framework.Modules;
using System.Reflection;

namespace Rocket.Unturned;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class U : MonoBehaviour, IRocketImplementation, IModuleNexus
{
    public static U Instance;

    public static XMLFileAsset<UnturnedSettings> Settings;
    public static XMLFileAsset<TranslationList> Translation;
    public static UnturnedEvents Events;
    public IRocketImplementationEvents ImplementationEvents => Events;

    public event RocketImplementationInitialized OnRocketImplementationInitialized;

    public string InstanceId => Provider.serverID;

    public static string Translate(string translationKey, params object[] placeholder)
    {
        return Translation.Instance.Translate(translationKey, placeholder);
    }

    public void Reload()
    {
        Translation.Load(null);
        Settings.Load(null);
    }


    public void initialize()
    {
        if (!Dedicator.isStandaloneDedicatedServer)
            return;
        GameObject rocketGameObject = new GameObject("Rocket");
        DontDestroyOnLoad(rocketGameObject);
        CommandWindow.Log("Rocket Unturned v" + Assembly.GetExecutingAssembly().GetName().Version + " for Unturned v" + Provider.APP_VERSION);
        PluginAdvertising.Get().PluginFrameworkName = "rocket";
        //R.OnRockedInitialized += () => Instance.Initialize();

        Provider.onServerHosted += () =>
        {
            rocketGameObject.TryAddComponent<U>();
            rocketGameObject.TryAddComponent<R>();
        };
    }

    public void shutdown() => Shutdown();

    public void Shutdown()
    {

    }
}
