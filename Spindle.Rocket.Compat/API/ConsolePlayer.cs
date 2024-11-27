using System;

namespace Rocket.API;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public class ConsolePlayer : IRocketPlayer, IComparable
{
    public string Id => "Console";
    public string DisplayName => "Console";
    public bool IsAdmin => true;
    public int CompareTo(object obj)
    {
        return obj switch
        {
            string str => string.Compare(Id, str, StringComparison.OrdinalIgnoreCase),
            ConsolePlayer => 0,
            null => 1,
            _ => string.Compare(Id, obj.ToString(), StringComparison.OrdinalIgnoreCase)
        };
    }
}