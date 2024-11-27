using System.Xml.Serialization;

namespace Rocket.Core.Serialization;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public sealed class WebPermissions
{
    [XmlAttribute]
    public bool Enabled;

    [XmlAttribute]
    public string Url = string.Empty;

    [XmlAttribute]
    public int Interval = 180;
}