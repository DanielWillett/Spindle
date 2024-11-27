using System;
using System.ComponentModel;

namespace Rocket.Core.Logging;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
[Obsolete("Redirects to Spindle log.")]
public class Logger
{
    [Obsolete("Log(string message,bool sendToConsole) is obsolete, use Log(string message,ConsoleColor color) instead", true)]
    public static void Log(string message, bool sendToConsole) => Log(message);

    [Obsolete("Redirects to Spindle log.")]
    public static void Log(string message, ConsoleColor color = ConsoleColor.White)
    {
        // todo reroute to spindle log
    }

    [Obsolete("Redirects to Spindle log.")]
    public static void LogWarning(string message)
    {
        // todo reroute to spindle log
    }

    [Obsolete("Redirects to Spindle log.")]
    public static void LogError(string message)
    {
        // todo reroute to spindle log
    }

    [Obsolete("Redirects to Spindle log.")]
    public static void LogException(Exception ex, string message = null)
    {
        // todo reroute to spindle log
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Redirects to Spindle log.")]
    public static void ExternalLog(object message, ConsoleColor color)
    {
        // todo reroute to spindle log
    }
}
