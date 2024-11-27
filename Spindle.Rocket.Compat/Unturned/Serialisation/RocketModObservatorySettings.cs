using System;
using System.Xml.Serialization;

namespace Rocket.Unturned.Serialisation;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
[Obsolete("Observatory is no longer maintained.")]
public sealed class RocketModObservatorySettings
{
    [Obsolete("Observatory is no longer maintained.")]
    [XmlAttribute]
    public bool CommunityBans = true;

    [XmlAttribute]
    [Obsolete("Observatory is no longer maintained.")]
    public bool KickLimitedAccounts = true;

    [XmlAttribute]
    [Obsolete("Observatory is no longer maintained.")]
    public bool KickTooYoungAccounts = true;

    [XmlAttribute]
    [Obsolete("Observatory is no longer maintained.")]
    public long MinimumAge = 604800;
}