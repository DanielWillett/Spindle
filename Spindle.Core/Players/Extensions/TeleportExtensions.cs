using Spindle.Threading;
using System;

namespace Spindle.Players;

public partial struct SpindlePlayer
{
    /// <summary>
    /// Teleports a player to a position and optional Y-axis rotation.
    /// </summary>
    /// <remarks>Use <see cref="TryTeleportTo"/> for an obstruction test.</remarks>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    public void TeleportTo(Vector3 position, float yaw = float.NaN)
    {
        GameThread.AssertCurrent();

        AssertOnline();

        if (!float.IsFinite(yaw))
            yaw = SteamPlayer.player.transform.eulerAngles.y;

        SteamPlayer.player.teleportToLocationUnsafe(position, yaw);
    }

    /// <summary>
    /// Teleports a player to an unobstructed position and optional Y-axis rotation.
    /// </summary>
    /// <returns><see langword="true"/> if the destination was unobstructed and the player was teleported, otherwise <see langword="false"/>.</returns>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    public bool TryTeleportTo(Vector3 position, float yaw = float.NaN)
    {
        GameThread.AssertCurrent();

        AssertOnline();

        if (!float.IsFinite(yaw))
            yaw = SteamPlayer.player.transform.eulerAngles.y;

        return SteamPlayer.player.teleportToLocation(position, yaw);
    }

    /// <summary>
    /// Teleport this player to their bed if they have one.
    /// </summary>
    /// <returns><see langword="true"/> if the player had an unobstructed bed claimed, otherwise <see langword="false"/>.</returns>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    public bool TryTeleportToBed()
    {
        GameThread.AssertCurrent();

        AssertOnline();

        if (!BarricadeManager.tryGetBed(Steam64, out Vector3 position, out byte packedAngle))
        {
            return false;
        }

        float angle = MeasurementTool.byteToAngle(packedAngle);
        return TryTeleportTo(position, angle);
    }

    /// <summary>
    /// Teleports a player to a position and optional Y-axis rotation.
    /// </summary>
    /// <remarks>Use <see cref="TryTeleportTo"/> for an obstruction test.</remarks>
    /// <exception cref="InvalidOperationException">No non-alt spawns available.</exception>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    public void TeleportToRandomPlayerSpawn()
    {
        GameThread.AssertCurrent();

        AssertOnline();

        if (!SteamPlayer.player.teleportToRandomSpawnPoint())
        {
            throw new InvalidOperationException(Properties.Resources.ExceptionNoPlayerSpawnsAvailable);
        }
    }
}