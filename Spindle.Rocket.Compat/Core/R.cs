using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Assets;
using Rocket.Core.Commands;
using Rocket.Core.Plugins;
using Rocket.Core.Serialization;

namespace Rocket.Core;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public class R : MonoBehaviour
{
    [TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
    public delegate void RockedInitialized();

    public static R Instance;
    public static IRocketImplementation Implementation;
    public static XMLFileAsset<RocketSettings> Settings = null;
    public static XMLFileAsset<TranslationList> Translation = null;
    public static IRocketPermissionsProvider Permissions = null;
    public static RocketPluginManager Plugins = null;
    public static RocketCommandManager Commands = null;

    // no this isn't a typo its like this in Rocket as well.
    public static event RockedInitialized OnRockedInitialized;

    public static string Translate(string translationKey, params object[] placeholder)
    {
        return Translation.Instance.Translate(translationKey, placeholder);
    }

    public static void Reload()
    {
        Settings.Load(null);
        Translation.Load(null);
        Permissions.Reload();
        //Plugins.Reload();
        //Commands.Reload();
        Implementation.Reload();
    }
}