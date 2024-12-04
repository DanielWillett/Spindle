using System;
using System.Runtime.Serialization;

namespace Spindle.Players;

/// <summary>
/// Thrown when a player is not online.
/// </summary>
/// <remarks>Subclass of <see cref="InvalidOperationException"/>.</remarks>
[Serializable]
public class PlayerOfflineException : InvalidOperationException
{
    public CSteamID Player { get; }
    public PlayerOfflineException(CSteamID player) : this(player, Properties.Resources.ExceptionPlayerOffline) { }

    public PlayerOfflineException(CSteamID player, string message) : base(message)
    {
        Player = player;
    }

    public PlayerOfflineException(CSteamID player, string message, Exception inner) : base(message, inner)
    {
        Player = player;
    }

    protected PlayerOfflineException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Player = new CSteamID(info.GetUInt64("Player"));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Player", Player.m_SteamID);
        base.GetObjectData(info, context);
    }
}
