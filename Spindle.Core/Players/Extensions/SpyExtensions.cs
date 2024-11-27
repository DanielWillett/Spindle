using Spindle.Threading;
using System;

namespace Spindle.Players;

public partial struct SpindlePlayer
{
    /// <summary>
    /// Request the player for a screenshot.
    /// </summary>
    /// <param name="timeout">Time before the screenshot request times out. Defaults to 10 seconds.</param>
    /// <returns>An awaitable task that completes when the screenshot is taken.</returns>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    public SpyTask RequestScreenshot(TimeSpan timeout = default)
    {
        GameThread.AssertCurrent();

        AssertOnline();

        if (timeout.Ticks <= 0)
            timeout = TimeSpan.FromSeconds(10d);

        return new SpyTask(SteamPlayer, timeout);
    }

    /// <summary>
    /// Request the player for a screenshot with a callback that gets invoked when the data is returned.
    /// </summary>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    public void RequestScreenshot(SpyCompleted callback)
    {
        if (callback == null)
            throw new ArgumentNullException(nameof(callback));

        GameThread.AssertCurrent();

        AssertOnline();

        SpyResult result = new SpyResult(callback, DateTime.UtcNow, SteamPlayer);
        SteamPlayer.player.sendScreenshot(CSteamID.Nil, result.InvokeCallback);
    }
}