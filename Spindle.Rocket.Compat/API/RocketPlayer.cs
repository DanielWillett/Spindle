using System;

namespace Rocket.API;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public class RocketPlayer : IRocketPlayer
{
    public string Id { get; }

    public string DisplayName { get; }

    public bool IsAdmin { get; }

    public RocketPlayer(string Id, string DisplayName = null, bool IsAdmin = false)
    {
        this.Id = Id;
        this.DisplayName = DisplayName ?? Id;
        this.IsAdmin = IsAdmin;
    }

    public int CompareTo(object obj)
    {
        return obj switch
        {
            string str => string.Compare(Id, str, StringComparison.OrdinalIgnoreCase),
            ConsolePlayer => -1,
            IRocketPlayer rp => string.Compare(Id, rp.Id, StringComparison.OrdinalIgnoreCase),
            null => 1,
            _ => string.Compare(Id, obj.ToString(), StringComparison.OrdinalIgnoreCase)
        };
    }
}