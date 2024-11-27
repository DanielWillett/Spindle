using Spindle.Threading;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using UnityEngine.Networking;
using Unturned.SystemEx;

namespace Spindle.Players;

public partial struct SpindlePlayer
{
    /// <summary>
    /// Disconnect this player from this server and transfer them to another server via an IP address.
    /// </summary>
    /// <param name="password">Optional password of the server being connected to.</param>
    /// <param name="showMenu">If the server connect menu should show instead of just putting the player into the server.</param>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    public void SendConnectToServer(IPv4Address ip, ushort port = 27015, string? password = null, bool showMenu = true)
    {
        GameThread.AssertCurrent();

        AssertOnline();

        password ??= string.Empty;
        SteamPlayer.player.sendRelayToServer(ip.value, port, password, showMenu);
    }

    /// <summary>
    /// Disconnect this player from this server and transfer them to another server via a server code.
    /// </summary>
    /// <param name="password">Optional password of the server being connected to.</param>
    /// <param name="showMenu">If the server connect menu should show instead of just putting the player into the server.</param>
    /// <exception cref="ArgumentException"><paramref name="serverCode"/> is not a Game Server SteamID.</exception>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    public void SendConnectToServer(CSteamID serverCode, string? password = null, bool showMenu = true)
    {
        if (!serverCode.BGameServerAccount())
            throw new ArgumentException(Properties.Resources.ExceptionSteamIdNotGameServer);

        GameThread.AssertCurrent();

        AssertOnline();

        password ??= string.Empty;
        SteamPlayer.player.sendRelayToServer(serverCode, password, showMenu);
    }

    /// <summary>
    /// Disconnect this player from this server and transfer them to another server via a server code, IPv4, bookmark URL, or DNS host.
    /// </summary>
    /// <remarks>Highly recommended to not pipe player input directly into this function, as URLs will cause a GET request to be made like how bookmarks work.</remarks>
    /// <param name="host">Can be a server code, IPv4, http/https URL, or DNS host.</param>
    /// <param name="port">Port to use if <paramref name="host"/> doesn't supply one. The port in <paramref name="host"/> will be prioritized.</param>
    /// <param name="password">Optional password of the server being connected to.</param>
    /// <param name="showMenu">If the server connect menu should show instead of just putting the player into the server.</param>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    public void SendConnectToServer(string host, string? password = null, ushort port = 0, bool showMenu = true)
    {
        GameThread.AssertCurrent();

        AssertOnline();

        password ??= string.Empty;
        host = host.Trim();

        if (host.Contains('/') && !host.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !host.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            host = "https://" + host;

        if (Uri.TryCreate(host, UriKind.Absolute, out Uri uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            host = uri.AbsoluteUri;

            UnturnedLog.info($"Requesting host details from {host}...");
            SteamPlayer pl = SteamPlayer;
            UniTask.Create(async () =>
            {
                using UnityWebRequest req = UnityWebRequest.Get(host);
                req.timeout = 2;
                try
                {
                    await req.SendWebRequest();
                    await UniTask.SwitchToMainThread();

                    if (pl.player == null)
                        return;

                    SpindlePlayer spindlePlayer = Create(pl);

                    string text = req.downloadHandler.text.Trim();
                    ConnectIntl(text, spindlePlayer, password, port, showMenu);
                }
                catch
                {
                    await UniTask.SwitchToMainThread();
                    UnturnedLog.warn($"Network error requesting host details: \"{req.error}\"");
                }
            });
        }
        else
        {
            ConnectIntl(host, this, password, port, showMenu);
        }

        return;

        static void ConnectIntl(string text, SpindlePlayer player, string password, ushort port, bool showMenu)
        {
            if (text.Length >= 6 && ulong.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong s64))
            {
                CSteamID serverCode = new CSteamID(s64);
                if (serverCode.BGameServerAccount())
                {
                    player.SendConnectToServer(serverCode, password, showMenu);
                    return;
                }
            }

            if (text.Equals("localhost", StringComparison.OrdinalIgnoreCase))
            {
                if (port == 0)
                    UnturnedLog.info("Cannot connect because port field is empty");
                else
                    player.SendConnectToServer(new IPv4Address("127.0.0.1"), port, password, showMenu);
                return;
            }

            if (text.StartsWith("localhost:") && ushort.TryParse(text.AsSpan(10), NumberStyles.Number, CultureInfo.InvariantCulture, out ushort portOverride))
            {
                player.SendConnectToServer(new IPv4Address("127.0.0.1"), portOverride, password, showMenu);
                return;
            }

            if (IPv4Address.TryParseWithOptionalPort(text, out IPv4Address a, out ushort? optionalPort))
            {
                if (optionalPort.HasValue && optionalPort.Value != 0)
                    port = optionalPort.Value;

                if (port == 0)
                {
                    UnturnedLog.info("Cannot connect because port field is empty");
                    return;
                }

                player.SendConnectToServer(a, port, password, showMenu);
                return;
            }

            SteamPlayer steamPlayer = player.SteamPlayer;
            string t = text;
            string p = password;
            ushort pt = port;
            bool sm = showMenu;
            Task.Run(async () =>
            {
                string? ip;
                try
                {
                    IPAddress[] dns = await Dns.GetHostAddressesAsync(t).ConfigureAwait(false);
                    ip = dns.FirstOrDefault()?.MapToIPv4().ToString();
                }
                catch (Exception ex)
                {
                    await UniTask.SwitchToMainThread();
                    UnturnedLog.exception(ex, $"Caught exception while resolving host string \"{t}\":");
                    return;
                }

                if (string.IsNullOrEmpty(ip))
                {
                    await UniTask.SwitchToMainThread();
                    UnturnedLog.info("Resolved address was empty");
                    return;
                }

                if (!IPv4Address.TryParse(ip, out IPv4Address address))
                {
                    await UniTask.SwitchToMainThread();
                    UnturnedLog.info($"Unable to parse resolved address \"{ip}\" from host string \"{t}\"");
                    return;
                }

                await UniTask.SwitchToMainThread();
                if (steamPlayer.player == null)
                    return;

                SpindlePlayer player = new SpindlePlayer(steamPlayer);
                if (pt == 0)
                {
                    UnturnedLog.info("Cannot connect because port field is empty");
                    return;
                }

                player.SendConnectToServer(address, pt, p, sm);
            });
        }
    }
}