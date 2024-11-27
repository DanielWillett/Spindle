using SDG.Framework.Utilities;
using Spindle.Threading;
using System;
using System.Globalization;
using System.IO;

namespace Spindle.Players;

/// <summary>
/// Request a screenshot using <see cref="RequestScreenshot"/>.
/// </summary>
public sealed class SpyTask
{
    private readonly SpyTaskAwaiter _awaiter;

    /// <summary>
    /// If the files created should be removed when the screenshot is completed.
    /// </summary>
    public bool RemoveFiles { get; set; } = true;

    internal SpyTask(SteamPlayer target, TimeSpan timeout)
    {
        GameThread.AssertCurrent();

        _awaiter = new SpyTaskAwaiter(timeout, target);
        target.player.sendScreenshot(CSteamID.Nil, OnReceive);
    }

    private void OnReceive(CSteamID player, byte[] jpg)
    {
        _awaiter.TellReceived(jpg);
    }

    public SpyTaskAwaiter GetAwaiter()
    {
        return _awaiter;
    }

    public sealed class SpyTaskAwaiter : INotifyCompletion
    {
        private readonly TimeSpan _timeout;

        private byte[]? _jpgData;
        private bool _didTimeout;
        private bool _didDisconnect;
        private Action? _continuation;
        private bool _subbed = true;

        /// <summary>
        /// The player the screenshot is for.
        /// </summary>
        public SteamPlayer Player { get; }

        public bool IsCompleted { get; private set; }

        public bool IsErrored => IsCompleted && _didTimeout;

        public SpyTaskAwaiter(TimeSpan timeout, SteamPlayer player)
        {
            Player = player;
            _timeout = timeout;
            Provider.onServerDisconnected += OnDisconnect;
            TimeUtility.InvokeAfterDelay(() =>
            {
                if (_subbed)
                {
                    Provider.onServerDisconnected -= OnDisconnect;
                    _subbed = false;
                }
                if (IsCompleted)
                    return;

                _didTimeout = true;
                Complete();
            }, (float)timeout.TotalSeconds);
        }

        private void OnDisconnect(CSteamID steamid)
        {
            if (steamid.m_SteamID != Player.playerID.steamID.m_SteamID)
                return;

            if (_subbed)
            {
                Provider.onServerDisconnected -= OnDisconnect;
                _subbed = false;
            }

            _didTimeout = true;
            _didDisconnect = true;
            Complete();
        }

        internal void TellReceived(byte[] jpg)
        {
            if (_subbed)
            {
                Provider.onServerDisconnected -= OnDisconnect;
                _subbed = false;
            }

            if (IsCompleted)
                return;

            _jpgData = jpg;
            Complete();
        }

        private void Complete()
        {
            IsCompleted = true;
            _continuation?.Invoke();

            try
            {
                string spy1 = Path.Combine(UnturnedPaths.RootDirectory.FullName, "Servers", Provider.serverID, "Spy.jpg");
                string spy2 = Path.Combine(UnturnedPaths.RootDirectory.FullName, "Servers", Provider.serverID, "Spy", Player.playerID.steamID.m_SteamID.ToString(CultureInfo.InvariantCulture) + ".jpg");

                if (File.Exists(spy1)) File.Delete(spy1);
                if (File.Exists(spy2)) File.Delete(spy2);
            }
            catch
            {
                // ignored
            }
        }

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            _continuation = continuation;
        }

        public byte[] GetResult()
        {
            if (_subbed)
            {
                Provider.onServerDisconnected -= OnDisconnect;
                _subbed = false;
            }

            if (!_didTimeout)
            {
                return _jpgData!;
            }

            if (_didDisconnect || Player.player == null)
            {
                throw new TimeoutException(
                    $"Player {Player.playerID.steamID.m_SteamID.ToString("D17", CultureInfo.InvariantCulture)} disconnected before screenshot could be received.");
            }

            throw new TimeoutException($"Screenshot for player {Player.playerID.steamID.m_SteamID.ToString("D17", CultureInfo.InvariantCulture)} timed out after: {_timeout:g}.");
        }
    }
}