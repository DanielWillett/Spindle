using Spindle.Threading;
using Spindle.Util;
using System;

namespace Spindle.Players;

public partial struct SpindlePlayer
{
    /// <summary>
    /// Tell the player to increment their value for a certain <paramref name="stat"/> by 1.
    /// </summary>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="stat"/> is either <see cref="EPlayerStat.NONE"/> or not a valid stat.</exception>
    public readonly void SendIncrementUserStat(EPlayerStat stat)
    {
        if (stat <= EPlayerStat.NONE)
            throw new ArgumentOutOfRangeException(nameof(stat));

        if (stat > EPlayerStat.FOUND_THROWABLES)
        {
            if (!EnumValidationUtility.ValidateValidField(stat))
                throw new ArgumentOutOfRangeException(nameof(stat));
        }

        GameThread.AssertCurrent();

        AssertOnline();

        SteamPlayer.player.sendStat(stat);
    }

    /// <summary>
    /// Tell the player to increment their value for a kill count stat for a <paramref name="target"/> by 1.
    /// </summary>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="target"/> is either <see cref="EPlayerStat.NONE"/> or not a valid stat.</exception>
    public readonly void SendIncrementUserStat(EPlayerKill target)
    {
        if (target <= EPlayerKill.NONE)
            throw new ArgumentOutOfRangeException(nameof(target));

        if (target > EPlayerKill.OBJECT)
        {
            if (!EnumValidationUtility.ValidateValidField(target))
                throw new ArgumentOutOfRangeException(nameof(target));
        }

        GameThread.AssertCurrent();

        AssertOnline();

        SteamPlayer.player.sendStat(target);
    }
}