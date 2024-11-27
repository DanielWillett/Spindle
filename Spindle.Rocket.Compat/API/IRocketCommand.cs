using System.Collections.Generic;

namespace Rocket.API;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public interface IRocketCommand
{
    AllowedCaller AllowedCaller { get; }

    string Name { get; }

    string Help { get; }

    string Syntax { get; }

    List<string> Aliases { get; }

    List<string> Permissions { get; }

    void Execute(IRocketPlayer caller, string[] command);
}
