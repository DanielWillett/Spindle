using System;

namespace Rocket.Core.Extensions;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public static class MulticastDelegateExtension
{
    [Obsolete("Probably shouldn't use this.")]
    public static void TryInvoke(this MulticastDelegate theDelegate, params object[] args)
    {
        if (theDelegate == null)
            return;

        foreach (Delegate invocation in theDelegate.GetInvocationList())
        {
            try
            {
                invocation.DynamicInvoke(args);
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in MulticastDelegate " + invocation.GetType().Name + ": " + ex.ToString());
            }
        }
    }
}