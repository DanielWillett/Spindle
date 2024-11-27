using Spindle.Threading;
using System;
using System.Globalization;

namespace Spindle.Players;

public partial struct SpindlePlayer
{
    /// <summary>
    /// Sends an 'open URL' prompt to this player.
    /// </summary>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    public void SendOpenUrl(string message, Uri url)
    {
        if (url == null)
            throw new ArgumentNullException(nameof(url));

        if (url.Scheme != Uri.UriSchemeHttp && url.Scheme != Uri.UriSchemeHttps)
            throw new ArgumentException(Properties.Resources.ExceptionBrowserRequestNotHttp, nameof(url));

        GameThread.AssertCurrent();

        AssertOnline();

        SteamPlayer.player.sendBrowserRequest(message, url.AbsoluteUri);
    }

    /// <summary>
    /// Sends an 'open URL' prompt to this player for another player's steam profile.
    /// </summary>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    public void SendOpenSteamProfile(string message, CSteamID player)
    {
        if (player.GetEAccountType() != EAccountType.k_EAccountTypeIndividual)
            throw new ArgumentException(Properties.Resources.ExceptionSteamIdNotIndividual, nameof(player));

        GameThread.AssertCurrent();

        AssertOnline();

        SteamPlayer.player.sendBrowserRequest(message, $"https://steamcommunity.com/profiles/{player.m_SteamID.ToString("D17", CultureInfo.InvariantCulture)}/");
    }
}