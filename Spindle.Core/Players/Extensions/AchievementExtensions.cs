using Spindle.Threading;
using Spindle.Unturned;
using System;

namespace Spindle.Players;

public partial struct SpindlePlayer
{
    /// <summary>
    /// Tell the player to trigger an achievement unlock on Steam if they don't already have it.
    /// </summary>
    /// <remarks>Most achievement IDs can be found in <see cref="KnownAchievementIds"/>.</remarks>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentException">Achievement ID is empty.</exception>
    public readonly void SendUnlockAchievement([ValueProvider("Spindle.Unturned.KnownAchievementIds")] string achievementId)
    {
        if (achievementId == null)
            throw new ArgumentNullException(nameof(achievementId));

        if (achievementId.Length == 0)
            throw new ArgumentException(Properties.Resources.ExceptionEmptyString, nameof(achievementId));

        GameThread.AssertCurrent();

        AssertOnline();

        SteamPlayer.player.sendAchievementUnlocked(achievementId);
    }
}