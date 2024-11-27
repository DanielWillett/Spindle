using System;

namespace Rocket.Core.Logging;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
[Obsolete("Redirects to Spindle log.")]
public class AsyncLoggerQueue
{
    public static AsyncLoggerQueue Current = new AsyncLoggerQueue();

    [Obsolete("Redirects to Spindle log.")]
    public void Enqueue(LogEntry le)
    {
        Logger.Log(le.Message, le.Severity switch
        {
            ELogType.Warning => ConsoleColor.Yellow,
            ELogType.Error => ConsoleColor.Red,
            ELogType.Exception => ConsoleColor.Red,
            _ => ConsoleColor.White
        });
    }
}
