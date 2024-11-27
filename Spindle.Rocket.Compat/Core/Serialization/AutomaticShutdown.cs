using System.Xml.Serialization;

namespace Rocket.Core.Serialization;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public sealed class AutomaticShutdown
{
    [XmlAttribute]
    public bool Enabled;

    [XmlAttribute]
    public int Interval = 86400;
}