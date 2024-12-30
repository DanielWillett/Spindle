using SDG.Framework.Utilities;
using Spindle.Threading;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace Spindle.Players;

/// <summary>
/// Request a screenshot using <see cref="SpindlePlayer.RequestScreenshot(CancellationToken,TimeSpan,bool)"/>.
/// </summary>
public sealed class PlayerSpyOperation
{
    private readonly SpyTaskAwaiter _awaiter;

    internal PlayerSpyOperation(SteamPlayer target, TimeSpan timeout, bool removeFiles, CancellationToken token)
    {
        GameThread.AssertCurrent();

        _awaiter = new SpyTaskAwaiter(timeout, target, removeFiles, token);
        target.player.sendScreenshot(CSteamID.Nil, OnReceive);
    }

    private void OnReceive(CSteamID id, byte[] jpg)
    {
        if (id.m_SteamID != _awaiter.Player.playerID.steamID.m_SteamID)
            throw new InvalidOperationException("Mismatched spy player with callback.");

        _awaiter.TellReceived(jpg);
    }

    public SpyTaskAwaiter GetAwaiter()
    {
        return _awaiter;
    }

    internal static void RemoveFilesIntl(ulong steam64)
    {
        try
        {
            string spy1 = Path.Combine(UnturnedPaths.RootDirectory.FullName, "Servers", Provider.serverID, "Spy.jpg");
            string spy2 = Path.Combine(UnturnedPaths.RootDirectory.FullName, "Servers", Provider.serverID, "Spy", $"{steam64.ToString(CultureInfo.InvariantCulture)}.jpg");

            if (File.Exists(spy1)) File.Delete(spy1);
            if (File.Exists(spy2)) File.Delete(spy2);
        }
        catch
        {
            // ignored
        }
    }

    public sealed class SpyTaskAwaiter : ICriticalNotifyCompletion
    {
        private readonly TimeSpan _timeout;
        private readonly bool _removeFiles;
        private readonly CancellationToken _token;
        private CancellationTokenRegistration _tokenRegistration;

        private byte[]? _jpgData;
        private bool _didTimeout;
        private bool _didCancel;
        private bool _didDisconnect;
        private Action? _continuation;
        private int _listeningForDisconnect;

        /// <summary>
        /// The player the screenshot is for.
        /// </summary>
        public SteamPlayer Player { get; }

        public bool IsCompleted { get; private set; }

        /// <summary>
        /// If the task errored, timed out, or was canceled.
        /// </summary>
        public bool IsErrored => IsCompleted && _didTimeout;

        public SpyTaskAwaiter(TimeSpan timeout, SteamPlayer player, bool removeFiles, CancellationToken token)
        {
            Player = player;
            _removeFiles = removeFiles;
            Provider.onServerDisconnected += OnDisconnect;
            _listeningForDisconnect = 1;

            if (token.CanBeCanceled)
            {
                _token = token;
                _tokenRegistration = token.Register(static awaiter => ((SpyTaskAwaiter)awaiter).Cancel(), this);
            }

            if (_timeout.Ticks > 0)
            {
                _timeout = timeout;
                TimeUtility.InvokeAfterDelay(Timeout, (float)timeout.TotalSeconds);
            }
            else
            {
                _timeout = System.Threading.Timeout.InfiniteTimeSpan;
            }
        }

        private void Timeout()
        {
            DisposeIntl();

            if (IsCompleted)
                return;

            _didTimeout = true;
            _didCancel = true;
            Complete();
        }
        
        private void Cancel()
        {
            if (!GameThread.IsCurrent)
            {
                GameThread.Dispatch(Cancel);
            }
            else
            {
                DisposeIntl();

                if (IsCompleted)
                    return;

                _didCancel = true;
                Complete();
            }
        }

        private void OnDisconnect(CSteamID steamid)
        {
            if (steamid.m_SteamID != Player.playerID.steamID.m_SteamID || IsCompleted)
                return;

            DisposeIntl();

            _didTimeout = true;
            _didDisconnect = true;
            _didCancel = true;
            Complete();
        }

        internal void TellReceived(byte[] jpg)
        {
            DisposeIntl();

            if (IsCompleted)
                return;

            _jpgData = jpg;
            Complete();
        }

        private void Complete()
        {
            IsCompleted = true;

            try
            {
                _continuation?.Invoke();
            }
            finally
            {
                if (_removeFiles)
                    RemoveFilesIntl(Player.playerID.steamID.m_SteamID);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UnsafeOnCompleted(Action continuation)
        {
            _continuation = continuation;
        }

        public byte[] GetResult()
        {
            DisposeIntl();

            if (_didCancel && !(_didTimeout || _didDisconnect))
            {
                throw new OperationCanceledException(string.Format(Properties.Resources.PlayerSpyOperation_Cancelled, Player.playerID.steamID.m_SteamID.ToString(CultureInfo.InvariantCulture)), _token);
            }

            if (_didDisconnect)
            {
                throw new TimeoutException(string.Format(Properties.Resources.PlayerSpyOperation_Disconnected, Player.playerID.steamID.m_SteamID.ToString(CultureInfo.InvariantCulture)));
            }

            if (_didTimeout)
            {
                throw new TimeoutException(string.Format(Properties.Resources.PlayerSpyOperation_Timeout, Player.playerID.steamID.m_SteamID.ToString(CultureInfo.InvariantCulture), _timeout.ToString("g", CultureInfo.InvariantCulture)));
            }

            return _jpgData ?? Array.Empty<byte>();
        }

        private void DisposeIntl()
        {
            if (Interlocked.Exchange(ref _listeningForDisconnect, 0) != 0)
            {
                Provider.onServerDisconnected -= OnDisconnect;
            }

            _tokenRegistration.Dispose();
            _tokenRegistration = default;
        }
    }
}