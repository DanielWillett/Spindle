using System;
using System.Net.Sockets;

namespace Rocket.Core.RCON;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
[Obsolete("Not supported")]
public class RCONConnection
{
    public TcpClient Client;
    public bool Authenticated;
    public bool Interactive;

    public int InstanceID { get; }
    public DateTime ConnectedTime { get; }
    public string Address => !Client.Client.Connected ? "?" : Client.Client.RemoteEndPoint.ToString();

    public RCONConnection(TcpClient client, int instance)
    {
        Client = client;
        Authenticated = false;
        Interactive = true;
        InstanceID = instance;
        ConnectedTime = DateTime.Now;
        Close();
    }

    public void Send(string command, bool nonewline = false)
    {
        throw new NotSupportedException("RCON not supported by Spindle/Rocket compatability.");
    }

    public string Read()
    {
        throw new NotSupportedException("RCON not supported by Spindle/Rocket compatability.");
    }

    public void Close()
    {
        if (!Client.Client.Connected)
            return;
        Client.Close();
        throw new NotSupportedException("RCON not supported by Spindle/Rocket compatability.");
    }
}