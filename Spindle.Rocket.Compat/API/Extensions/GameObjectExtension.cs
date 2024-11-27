using System;

namespace Rocket.API.Extensions;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public static class GameObjectExtension
{
    public static T TryAddComponent<T>(this GameObject gameobject) where T : Component
    {
        return (T)TryAddComponent(gameobject, typeof(T));
    }

    public static object TryAddComponent(this GameObject gameobject, Type T)
    {
        try
        {
            return gameobject.AddComponent(T);
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occured while adding component {T.Name}", ex);
        }
    }

    public static void TryRemoveComponent<T>(this GameObject gameobject) where T : Component
    {
        gameobject.TryRemoveComponent(typeof(T));
    }

    public static void TryRemoveComponent(this GameObject gameobject, Type T)
    {
        try
        {
            Component c = gameobject.GetComponent(T);
            if (c != null)
                Object.Destroy(c);
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occured while removing component {T.Name}", ex);
        }
    }
}