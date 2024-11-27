using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Rocket.API.Serialisation;

[Serializable]
[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public class RocketPermissions : IDefaultable
{
    [XmlElement("DefaultGroup")]
    public string DefaultGroup = "default";

    [XmlArray("Groups")]
    [XmlArrayItem(ElementName = "Group")]
    public List<RocketPermissionsGroup> Groups = new List<RocketPermissionsGroup>();

    public void LoadDefaults()
    {
        DefaultGroup = "default";
        Groups = new List<RocketPermissionsGroup>
        {
            new RocketPermissionsGroup("default", "Guest", null, new List<string>(), new List<Permission>
            {
                new Permission("p"),
                new Permission("compass"),
                new Permission("rocket")
            }, "white"),
            new RocketPermissionsGroup("vip", "VIP", "default", new List<string>
            {
                "76561198016438091"
            }, new List<Permission>
            {
                new Permission("effect"),
                new Permission("heal", 120U),
                new Permission("v", 30U)
            }, "FF9900")
        };
    }
}