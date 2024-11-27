using System;

namespace Rocket.Core.Commands;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public class RocketCommandAliasAttribute : Attribute
{
    public string Name { get; set; }

    // ReSharper disable once InconsistentNaming
    public RocketCommandAliasAttribute(string Name)
    {
        this.Name = Name;
    }
}