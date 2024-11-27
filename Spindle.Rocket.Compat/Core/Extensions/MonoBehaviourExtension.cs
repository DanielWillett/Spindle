using System;
using System.Reflection;

namespace Rocket.Core.Extensions;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public static class MonoBehaviourExtension
{
    private static object[] _emptyObjects;

    [Obsolete("Probably shouldn't use this.")]
    public static void Invoke(this MonoBehaviour behaviour, string method, object options, float delay)
    {
        MethodInfo mtd = behaviour.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
        if (mtd == null)
        {
            throw new MissingMethodException($"Unknown method in {behaviour.GetType().FullName}: {method}({options?.GetType().Name}).");
        }

        if (delay <= 0)
        {
            InvokeIntl(mtd, behaviour, options);
            return;
        }

        behaviour.StartCoroutine(InvokeWithDelay(delay, mtd, behaviour, options));
    }

    private static void InvokeIntl(MethodInfo method, MonoBehaviour behavior, object options)
    {
        method.Invoke(method.IsStatic ? null : behavior, options switch
        {
            null => _emptyObjects ??= new object[0],
            object[] a => a,
            _ => new object[] { options }
        });
    }

    private static IEnumerator InvokeWithDelay(float delay, MethodInfo method, MonoBehaviour behavior, object options)
    {
        yield return new WaitForSecondsRealtime(delay);
        InvokeIntl(method, behavior, options);
    }
}