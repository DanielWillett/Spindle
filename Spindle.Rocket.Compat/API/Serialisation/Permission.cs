using System;
using System.Xml.Serialization;

namespace Rocket.API.Serialisation;

[Serializable]
[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public class Permission
{
    [XmlAttribute]
    public uint Cooldown;

    [XmlText]
    public string Name = string.Empty;

    public Permission() { }

    public Permission(string name, uint cooldown = 0)
    {
        Name = name;
        Cooldown = cooldown;
    }

    public override string ToString() => Name;
}