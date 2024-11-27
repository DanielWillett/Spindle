using System.Xml.Serialization;

namespace Rocket.Core.Serialization;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public sealed class CommandMapping
{
    [XmlAttribute]
    public string Name = string.Empty;

    [XmlAttribute]
    public bool Enabled = true;

    [XmlAttribute]
    public CommandPriority Priority;

    [XmlText]
    public string Class = string.Empty;

    public CommandMapping() { }
    public CommandMapping(string name, string @class, bool enabled = true, CommandPriority priority = CommandPriority.Normal)
    {
        Name = name;
        Enabled = enabled;
        Class = @class;
        Priority = priority;
    }
}