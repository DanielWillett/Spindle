using System;

namespace Rocket.Unturned.Player;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class PlayerIsConsoleException : Exception
{
    public PlayerIsConsoleException() : base("This player is a ConsolePlayer. Expected a valid player ID.") { }
}