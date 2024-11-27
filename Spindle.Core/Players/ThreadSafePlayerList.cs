using System.Collections.Concurrent;
using Spindle.Threading;

namespace Spindle.Players;

internal static class ThreadSafePlayerList
{
    public static readonly ConcurrentDictionary<ulong, SteamPlayer> OnlinePlayers = new ConcurrentDictionary<ulong, SteamPlayer>();

    internal static void Initialize()
    {
        GameThread.AssertCurrent();

        // enemy connected is invoked almost immediately after adding the player to Provider.clients, better to use this for on join since patches may require it later
        OnlinePlayers.Clear();

        Provider.onEnemyConnected += ServerConnected;
        Provider.onServerDisconnected += ServerDisconnected;
    }

    internal static void Shutdown()
    {
        GameThread.AssertCurrent();

        Provider.onEnemyConnected -= ServerConnected;
        Provider.onServerDisconnected -= ServerDisconnected;

        OnlinePlayers.Clear();
    }

    private static void ServerDisconnected(CSteamID steamid)
    {
        OnlinePlayers.TryRemove(steamid.m_SteamID, out _);
    }

    private static void ServerConnected(SteamPlayer player)
    {
        OnlinePlayers[player.playerID.steamID.m_SteamID] = player;
    }
}
