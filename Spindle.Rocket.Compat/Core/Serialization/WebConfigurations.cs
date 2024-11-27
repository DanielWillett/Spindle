using System.Xml.Serialization;

namespace Rocket.Core.Serialization;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public sealed class WebConfigurations
{
    [XmlAttribute]
    public bool Enabled;

    [XmlAttribute]
    public string Url = string.Empty;
}