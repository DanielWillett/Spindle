using System.Xml.Serialization;

namespace Rocket.Core.Serialization;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public sealed class RemoteConsole
{
    [XmlAttribute]
    public bool Enabled;

    [XmlAttribute]
    public ushort Port = 27115;

    [XmlAttribute]
    public string Password = "changeme";

    [XmlAttribute]
    public bool EnableMaxGlobalConnections = true;

    [XmlAttribute]
    public ushort MaxGlobalConnections = 10;

    [XmlAttribute]
    public bool EnableMaxLocalConnections = true;

    [XmlAttribute]
    public ushort MaxLocalConnections = 3;
}