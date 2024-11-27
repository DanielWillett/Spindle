using System;
using System.Net;

namespace Rocket.Core.Utils;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public class RocketWebClient : WebClient
{
    public int Timeout { get; set; }

    public RocketWebClient() : this(30000) { }

    public RocketWebClient(int timeout) => Timeout = timeout;

    protected override WebRequest GetWebRequest(Uri address)
    {
        WebRequest webRequest = base.GetWebRequest(address);
        if (webRequest != null)
            webRequest.Timeout = Timeout;
        return webRequest;
    }
}