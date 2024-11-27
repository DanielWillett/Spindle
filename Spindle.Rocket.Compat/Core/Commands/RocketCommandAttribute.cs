using Rocket.API;
using System;

namespace Rocket.Core.Commands;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public class RocketCommandAttribute : Attribute
{
    public string Name { get; }
    public string Help { get; }
    public string Syntax { get; }
    public AllowedCaller AllowedCaller { get; }

    // ReSharper disable InconsistentNaming
    public RocketCommandAttribute(string Name, string Help, string Syntax = "", AllowedCaller AllowedCaller = AllowedCaller.Both)
    {
        this.Name = Name;
        this.Help = Help;
        this.Syntax = Syntax;
        this.AllowedCaller = AllowedCaller;
    }
    // ReSharper restore InconsistentNaming
}