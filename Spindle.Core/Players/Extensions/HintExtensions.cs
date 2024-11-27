using Spindle.Threading;
using System;

namespace Spindle.Players;

public partial struct SpindlePlayer
{
    /// <summary>
    /// Shows a hint on the player's screen that lasts a certain <paramref name="duration"/>.
    /// </summary>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    public void SendHint(string hint, TimeSpan duration = default)
    {
        GameThread.AssertCurrent();

        AssertOnline();

        float seconds = (float)duration.TotalSeconds;
        if (seconds <= 1f / 60f)
            seconds = 10;

        SteamPlayer.player.ServerShowHint(hint, seconds);
    }
}