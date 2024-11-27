using System;

namespace Rocket.API;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public class NoPermissionsForCommandException : Exception
{
    // ReSharper disable once RedundantOverriddenMember
    public override string Message => base.Message;
    public NoPermissionsForCommandException(IRocketPlayer player, IRocketCommand command)
        : base($"The player {player.DisplayName} has no permission to execute the command {command.Name}") { }
}