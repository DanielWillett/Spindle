using System.Xml.Serialization;

namespace Rocket.Unturned.Serialisation;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public sealed class AutomaticSaveSettings
{
    [XmlAttribute]
    public bool Enabled = true;

    [XmlAttribute]
    public int Interval = 1800;
}