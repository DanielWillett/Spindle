using System;

namespace Spindle.Players;

/// <summary>
/// Stores the result from a callback'd 
/// </summary>
public class SpyResult
{
    private readonly SpyCompleted _callback;
    private readonly DateTime _dtStart;
    private readonly SteamPlayer _steamPlayer;
    public SpindlePlayer Player { get; private set; }
    public TimeSpan ResponseTime { get; private set; }
#nullable disable
    public byte[] JpegData { get; private set; }
#nullable restore

    internal SpyResult(SpyCompleted callback, DateTime dtStart, SteamPlayer steamPlayer)
    {
        _callback = callback;
        _dtStart = dtStart;
        _steamPlayer = steamPlayer;
    }

    internal void InvokeCallback(CSteamID id, byte[] jpg)
    {
        JpegData = jpg;
        ResponseTime = DateTime.UtcNow - _dtStart;
        Player = SpindlePlayer.Create(_steamPlayer);

        if (Player.IsOnline)
            _callback(this);
    }
}

public delegate void SpyCompleted(SpyResult result);