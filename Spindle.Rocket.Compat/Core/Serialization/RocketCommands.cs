using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Rocket.Core.Serialization;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public sealed class RocketCommands : IDefaultable
{
    [XmlArray("Commands")]
    [XmlArrayItem("Command")]
    public List<CommandMapping> CommandMappings = new List<CommandMapping>();

    public void LoadDefaults() => CommandMappings = new List<CommandMapping>();
}