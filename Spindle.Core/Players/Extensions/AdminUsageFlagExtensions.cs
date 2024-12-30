namespace Spindle.Players;

public partial struct SpindlePlayer
{
    /// <summary>
    /// Currently active admin features, as reported by the client.
    /// </summary>
    public readonly EPlayerAdminUsageFlags ActiveAdminFeatures => SteamPlayer.player.AdminUsageFlags;

    /// <summary>
    /// If the admin workzone is active, as reported by the client.
    /// </summary>
    public readonly bool IsWorkzoneActive => (SteamPlayer.player.AdminUsageFlags & EPlayerAdminUsageFlags.Workzone) != 0;

    /// <summary>
    /// If the admin freecam is active, as reported by the client.
    /// </summary>
    public readonly bool IsFreecamActive => (SteamPlayer.player.AdminUsageFlags & EPlayerAdminUsageFlags.Freecam) != 0;

    /// <summary>
    /// If the spectator overlay (long distance nametags) is active, as reported by the client.
    /// </summary>
    public readonly bool IsSpectatorOverlayActive => (SteamPlayer.player.AdminUsageFlags & EPlayerAdminUsageFlags.SpectatorStatsOverlay) != 0;
}