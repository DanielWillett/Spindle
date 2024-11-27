using Rocket.API;
using System.Xml.Serialization;

namespace Rocket.Core.Serialization;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public sealed class RocketSettings : IDefaultable
{
    [XmlElement("RCON")]
    public RemoteConsole RCON = new RemoteConsole();

    [XmlElement("AutomaticShutdown")]
    public AutomaticShutdown AutomaticShutdown = new AutomaticShutdown();

    [XmlElement("WebConfigurations")]
    public WebConfigurations WebConfigurations = new WebConfigurations();

    [XmlElement("WebPermissions")]
    public WebPermissions WebPermissions = new WebPermissions();

    [XmlElement("LanguageCode")]
    public string LanguageCode = "en";

    [XmlElement("MaxFrames")]
    public int MaxFrames = 60;

    public void LoadDefaults()
    {
        RCON = new RemoteConsole();
        AutomaticShutdown = new AutomaticShutdown();
        WebConfigurations = new WebConfigurations();
        WebPermissions = new WebPermissions();
        LanguageCode = "en";
        MaxFrames = 60;
    }
}