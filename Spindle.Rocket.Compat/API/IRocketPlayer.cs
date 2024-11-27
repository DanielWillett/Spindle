using System;

namespace Rocket.API;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public interface IRocketPlayer : IComparable
{
    string Id { get; }

    string DisplayName { get; }

    bool IsAdmin { get; }
}