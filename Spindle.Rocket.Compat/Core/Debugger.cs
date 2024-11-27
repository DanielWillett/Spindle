using System;
using System.Collections.Generic;

namespace Rocket.Core;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
[Obsolete("Unsupported")]
public class Debugger : MonoBehaviour
{
    [Obsolete("Unsupported")]
    public static IDictionary<IntPtr, string> GetOpenWindows() => new Dictionary<IntPtr, string>(0);

    [Obsolete("Unsupported")]
    public void Awake() { }

    [Obsolete("Unsupported")]
    public void FixedUpdate() { }
}