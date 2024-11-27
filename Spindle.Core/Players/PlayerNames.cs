using System;

namespace Spindle.Players;

/// <summary>
/// Stores all 3 of a player's names.
/// </summary>
public readonly struct PlayerNames
{
    /// <summary>
    /// The player's Steam64 ID.
    /// </summary>
    public readonly CSteamID Steam64;

    /// <summary>
    /// The player's Steam name.
    /// </summary>
    public readonly string PlayerName;

    /// <summary>
    /// The public name of the player's character.
    /// </summary>
    public readonly string CharacterName;
    
    /// <summary>
    /// The private name of the player's character that only their group can see.
    /// </summary>
    public readonly string NickName;

    public PlayerNames(in SpindlePlayer player) : this(player.SteamPlayer.playerID) { }
    public PlayerNames(Player player) : this(player.channel.owner.playerID) { }
    public PlayerNames(PlayerCaller player) : this(player.player.channel.owner.playerID) { }
    public PlayerNames(SteamPlayer player) : this(player.playerID) { }
    public PlayerNames(SteamPlayerID playerId) : this(playerId.steamID, playerId.playerName, playerId.characterName, playerId.nickName) { }
    public PlayerNames(CSteamID steam64, string playerName, string characterName, string nickName)
    {
        Steam64 = steam64;
        PlayerName = playerName;
        CharacterName = characterName;
        NickName = nickName;
    }

    /// <summary>
    /// Displays all of a player's available names, excluding duplicates. Includes their Steam64 ID.
    /// </summary>
    /// <remarks>Format: <c>STEAM64 (playerName | charName | nickName)</c>.</remarks>
    public override string ToString() => ToString(true);

    /// <summary>
    /// Displays all of a player's available names, excluding duplicates. Includes their Steam64 ID if <paramref name="steamId"/> is <see langword="true"/>.
    /// </summary>
    /// <remarks>Format: <c>STEAM64 (playerName | charName | nickName)</c> or <c>playerName | charName | nickName</c>.</remarks>
    public string ToString(bool steamId)
    {
        string s64 = Steam64.m_SteamID.ToString("D17");

        string? pn = PlayerName;
        string? cn = CharacterName;
        string? nn = NickName;
        bool pws = string.IsNullOrWhiteSpace(pn);
        bool cws = string.IsNullOrWhiteSpace(cn);
        bool nws = string.IsNullOrWhiteSpace(nn);
        if (pws && cws && nws)
            return s64;

        if (pws)
        {
            if (cws)
                return steamId ? s64 + " (" + nn + ")" : nn;
            if (nws || nn.Equals(cn, StringComparison.Ordinal))
                return steamId ? s64 + " (" + cn + ")" : cn;
            return steamId ? s64 + " (" + cn + " | " + nn + ")" : cn + " | " + nn;
        }
        if (cws)
        {
            if (pws)
                return steamId ? s64 + " (" + nn + ")" : nn;
            if (nws || nn.Equals(pn, StringComparison.Ordinal))
                return steamId ? s64 + " (" + pn + ")" : pn;
            return steamId ? s64 + " (" + pn + " | " + nn + ")" : pn + " | " + nn;
        }
        if (nws)
        {
            if (pws)
                return steamId ? s64 + " (" + cn + ")" : cn;
            if (cws || cn.Equals(pn, StringComparison.Ordinal))
                return steamId ? s64 + " (" + pn + ")" : pn;
            return steamId ? s64 + " (" + pn + " | " + cn + ")" : pn + " | " + cn;
        }

        bool nep = nn.Equals(pn, StringComparison.Ordinal);
        bool nec = nn.Equals(cn, StringComparison.Ordinal);
        bool pec = nec && nep || pn.Equals(cn, StringComparison.Ordinal);
        if (nep && nec)
            return steamId ? s64 + " (" + nn + ")" : nn;
        if (pec || nec)
            return steamId ? s64 + " (" + pn + " | " + nn + ")" : pn + " | " + nn;
        if (nep)
            return steamId ? s64 + " (" + pn + " | " + cn + ")" : pn + " | " + cn;

        return steamId ? s64 + " (" + pn + " | " + cn + " | " + nn + ")" : pn + " | " + cn + " | " + nn;
    }

    public static bool operator ==(PlayerNames left, PlayerNames right) => left.Steam64.m_SteamID == right.Steam64.m_SteamID;
    public static bool operator !=(PlayerNames left, PlayerNames right) => left.Steam64.m_SteamID != right.Steam64.m_SteamID;
    public override bool Equals(object? obj) => obj is PlayerNames pn && Steam64.m_SteamID == pn.Steam64.m_SteamID;
    public override int GetHashCode() => unchecked ( (int)Steam64.m_SteamID );

    public static string SelectPlayerName(PlayerNames names) => names.PlayerName;
    public static string SelectCharacterName(PlayerNames names) => names.CharacterName;
    public static string SelectNickName(PlayerNames names) => names.NickName;
}