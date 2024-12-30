using System;

namespace Spindle.Players;

/// <summary>
/// Stores the result from a callback'd 
/// </summary>
public sealed class SpyResult
{
    private readonly SpyCompleted _callback;
    private readonly DateTime _dtStart;
    private readonly SteamPlayer _steamPlayer;
    private readonly bool _removeFiles;

    /// <summary>
    /// The player that requested the screenshot.
    /// </summary>
    public SpindlePlayer Player { get; private set; }

    /// <summary>
    /// Amount of time between when the request was sent and when it was received.
    /// </summary>
    public TimeSpan ResponseTime { get; private set; }

#nullable disable

    /// <summary>
    /// Raw JPEG data for the screenshot.
    /// </summary>
    public byte[] JpegData { get; private set; }
#nullable restore

    internal SpyResult(SpyCompleted callback, DateTime dtStart, SteamPlayer steamPlayer, bool removeFiles)
    {
        _callback = callback;
        _dtStart = dtStart;
        _steamPlayer = steamPlayer;
        _removeFiles = removeFiles;
    }

    internal void InvokeCallback(CSteamID id, byte[] jpg)
    {
        if (id.m_SteamID != _steamPlayer.playerID.steamID.m_SteamID)
            throw new InvalidOperationException("Mismatched spy player with callback.");

        JpegData = jpg;
        ResponseTime = DateTime.UtcNow - _dtStart;
        Player = SpindlePlayer.Create(_steamPlayer);

        if (Player.IsOnline)
            _callback(this);

        if (_removeFiles)
            PlayerSpyOperation.RemoveFilesIntl(id.m_SteamID);
    }
}

public delegate void SpyCompleted(SpyResult result);