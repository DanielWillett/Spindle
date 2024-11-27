using Rocket.API;
using Rocket.Core.Assets;
using System;
using System.IO;
using System.Linq;

namespace Rocket.Core.Plugins;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public class RocketPlugin<RocketPluginConfiguration> : RocketPlugin, IRocketPlugin<RocketPluginConfiguration> where RocketPluginConfiguration : class, IRocketPluginConfiguration
{
    public IAsset<RocketPluginConfiguration> Configuration { get; }

    public RocketPlugin()
    {
        string str = Path.Combine(Directory, string.Format(Environment.PluginConfigurationFileTemplate, Name));
        string uriString = string.Empty;

        if (R.Settings.Instance.WebConfigurations.Enabled)
            uriString = string.Format(Environment.WebConfigurationTemplate, R.Settings.Instance.WebConfigurations.Url, Name, R.Implementation.InstanceId);
        else if (File.Exists(str))
            uriString = File.ReadAllLines(str).First().Trim();

        if (Uri.TryCreate(uriString, UriKind.Absolute, out Uri result))
            Configuration = new WebXMLFileAsset<RocketPluginConfiguration>(result, callback: asset => base.LoadPlugin());
        else
            Configuration = new XMLFileAsset<RocketPluginConfiguration>(str);
    }

    public override void LoadPlugin()
    {
        Configuration.Load(asset => base.LoadPlugin());
    }
}
