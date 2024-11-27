using System;

namespace Rocket.API;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public class WrongUsageOfCommandException : Exception
{
    // ReSharper disable once RedundantOverriddenMember
    public override string Message => base.Message;
    public WrongUsageOfCommandException(IRocketPlayer player, IRocketCommand command)
        : base($"The player {player.DisplayName} did not correctly use the command {command.Name}") { }
}