namespace Rocket.Core.Logging;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public class LogEntry
{
    public ELogType Severity;
    public string Message;
    public bool RCON;
}