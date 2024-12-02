using Spindle.Threading;
using System;

namespace Spindle.Players;

public partial struct SpindlePlayer
{
    /// <summary>
    /// Logs a message in this player's log file on client-side. Can be used for debugging.
    /// </summary>
    /// <param name="logValue">The raw text to log.</param>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentException"><paramref name="logValue"/> is longer than 2048 characters.</exception>
    public readonly void SendLogText(string logValue)
    {
        if (logValue == null)
            throw new ArgumentNullException(nameof(logValue));

        if (logValue.Length > 2048)
            throw new ArgumentException(Properties.Resources.ExceptionNetStringTooLong, nameof(logValue));

        GameThread.AssertCurrent();

        AssertOnline();

        SteamPlayer.player.sendTerminalRelay(logValue);
    }

    /// <summary>
    /// Logs a message in this player's log file on client-side. Can be used for debugging.
    /// </summary>
    /// <param name="logValue">The raw text to log.</param>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentException"><paramref name="logValue"/> is longer than 2048 characters or is an invalid format.</exception>
    [StringFormatMethod(nameof(logValue))]
    public readonly void SendLogText(string logValue, params object?[]? formattingArguments)
    {
        if (logValue == null)
            throw new ArgumentNullException(nameof(logValue));

        if (logValue.Length > 2048)
            throw new ArgumentException(Properties.Resources.ExceptionNetStringTooLong, nameof(logValue));

        if (formattingArguments is { Length: > 0 })
        {
            try
            {
                for (int i = 0; i < formattingArguments.Length; ++i)
                {
                    if (formattingArguments[i] != null)
                        continue;

                    formattingArguments[i] = "null";
                    break;
                }

                logValue = string.Format(logValue, formattingArguments);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException(Properties.Resources.ExceptionInvalidFormat, nameof(logValue), ex);
            }
        }

        if (logValue.Length > 2048)
            throw new ArgumentException(Properties.Resources.ExceptionNetStringTooLong, nameof(logValue));

        GameThread.AssertCurrent();

        AssertOnline();

        SteamPlayer.player.sendTerminalRelay(logValue);
    }
}