using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Rocket.Core.RCON;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
[Obsolete("Not supported")]
public class RCONServer : MonoBehaviour
{
    public static List<RCONConnection> Clients => new List<RCONConnection>(0);

    public void Awake()
    {
        throw new NotSupportedException("RCON not supported by Spindle/Rocket compatability.");
    }

    public static void Broadcast(string message)
    {
        throw new NotSupportedException("RCON not supported by Spindle/Rocket compatability.");
    }
    
    public static string Read(TcpClient client, bool auth)
    {
        throw new NotSupportedException("RCON not supported by Spindle/Rocket compatability.");
    }

    public static void Send(TcpClient client, string text)
    {
        throw new NotSupportedException("RCON not supported by Spindle/Rocket compatability.");
    }
}
