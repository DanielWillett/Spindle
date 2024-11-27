using System;

namespace Rocket.Core.Commands;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public class RocketCommandPermissionAttribute : Attribute
{
    public string Name { get; set; }

    // ReSharper disable once InconsistentNaming
    public RocketCommandPermissionAttribute(string Name)
    {
        this.Name = Name;
    }
}