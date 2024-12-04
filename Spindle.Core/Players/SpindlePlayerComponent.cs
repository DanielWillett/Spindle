using DanielWillett.ReflectionTools;
using Microsoft.Extensions.DependencyInjection;
using Spindle.Logging;
using Spindle.Threading;
using Spindle.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Spindle.Players;

/// <summary>
/// Manages all player-specific data for Spindle internals.
/// </summary>
[RequireComponent(typeof(Player))]
internal class SpindlePlayerComponent : MonoBehaviour
{
    /// <summary>
    /// List of all instances of <see cref="SpindlePlayerComponent"/> which is much quicker than <see cref="GameObject.GetComponent"/>.
    /// </summary>
    internal static ConcurrentDictionary<ulong, SpindlePlayerComponent> ComponentDictionary = new ConcurrentDictionary<ulong, SpindlePlayerComponent>();

    private static ILogger<SpindlePlayerComponent>? _logger;
    private static Type[] _playerComponentTypes = Type.EmptyTypes;
    private Player? _player;
    private ulong _steam64;

#nullable disable

    /// <summary>
    /// Time at which the player connected in realtime seconds.
    /// </summary>
    internal float ConnectTime;

    /// <summary>
    /// If the player is currently online.
    /// </summary>
    internal bool IsOnline;

    /// <summary>
    /// Look-up dictionary of all player components.
    /// </summary>
    internal TypeLookUpDictionary<IPlayerComponent> Components;

    /// <summary>
    /// Quick reference to the player's names.
    /// </summary>
    internal PlayerNames Names;

#nullable restore
    internal static void Initialize()
    {
        GameThread.AssertCurrent();

        Provider.onEnemyConnected += OnEnemyConnected;
        Provider.onEnemyDisconnected += OnEnemyDisonnected;
    }

    internal static void Shutdown()
    {
        GameThread.AssertCurrent();

        Provider.onEnemyConnected -= OnEnemyConnected;
        Provider.onEnemyDisconnected -= OnEnemyDisonnected;
    }

    private static void OnEnemyConnected(SteamPlayer player)
    {
        if (ComponentDictionary.TryRemove(player.playerID.steamID.m_SteamID, out SpindlePlayerComponent oldComp))
        {
            oldComp.Disconnect();
            Destroy(oldComp);
        }

        SpindlePlayerComponent comp = player.player.gameObject.AddComponent<SpindlePlayerComponent>();
        comp.PreInitialize(player.player);
        ComponentDictionary[player.playerID.steamID.m_SteamID] = comp;
        comp.PostInitialize();
    }

    private static void OnEnemyDisonnected(SteamPlayer player)
    {
        if (ComponentDictionary.TryGetValue(player.playerID.steamID.m_SteamID, out SpindlePlayerComponent comp))
        {
            comp.Disconnect();
        }

        ComponentDictionary.TryRemove(player.playerID.steamID.m_SteamID, out _);
    }

#pragma warning disable IDE0051
    // ReSharper disable UnusedMember.Local
    private void PreInitialize(Player player)
    {
        _logger ??= SpindleLauncher.LoggerFactory.CreateLogger<SpindlePlayerComponent>();
        IsOnline = true;

        _player = player;
        _steam64 = _player.channel.owner.playerID.steamID.m_SteamID;

        Names = new PlayerNames(player);

        Components = new TypeLookUpDictionary<IPlayerComponent>(_playerComponentTypes, CreateComponents(_playerComponentTypes));
    }

    private void PostInitialize()
    {
        SpindlePlayer player = SpindlePlayer.Create(_player!);

        IPlayerComponent[] components = Components.Values;
        for (int i = 0; i < components.Length; ++i)
        {
            IPlayerComponent component = components[i];
            try
            {
                component.Initialize(player);
                if (component.Player != player)
                {
                    _logger!.LogWarning(Properties.Resources.LogPlayerComponentFailedToAssignPlayerProperty, Accessor.Formatter.Format(component.GetType()));
                }
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, Properties.Resources.LogPlayerComponentFailedToInitialize, Accessor.Formatter.Format(component.GetType()), _player!.channel.owner.playerID.steamID.m_SteamID);
            }
        }
    }

    private void Disconnect()
    {
        Destroy(this);
        IsOnline = false;
        _player = null;
        ComponentDictionary.Remove(_steam64, out _);
        DisposeComponents(Components.Values);
    }

    // ReSharper restore UnusedMember.Local
#pragma warning restore IDE0051

    private IPlayerComponent[] CreateComponents(ArraySegment<Type> componentTypes)
    {
        IPlayerComponent[] array = new IPlayerComponent[componentTypes.Count];

        IServiceProvider serviceProvider = SpindleLauncher.ServiceProvider;

        for (int i = 0; i < array.Length; ++i)
        {
            Type t = componentTypes[i];
            IPlayerComponent component;
            try
            {
                if (t.IsSubclassOf(typeof(Component)))
                {
                    component = (IPlayerComponent)_player!.gameObject.AddComponent(t);
                }
                else
                {
                    component = (IPlayerComponent)ActivatorUtilities.CreateInstance(serviceProvider, t);
                }
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, Properties.Resources.LogPlayerComponentFailedToCreate, Accessor.Formatter.Format(t), _player!.channel.owner.playerID.steamID.m_SteamID);
                continue;
            }

            array[i] = component;
        }

        return array;
    }

    internal void RemoveComponentTypes(IList<Type> types)
    {
        TypeLookUpDictionary<IPlayerComponent>[] dicts = new TypeLookUpDictionary<IPlayerComponent>[ComponentDictionary.Count];

        int index = 0;
        foreach (SpindlePlayerComponent comp in ComponentDictionary.Values)
        {
            dicts[index] = comp.Components;
            ++index;
        }

        List<IPlayerComponent>?[]? removedComponents = TypeLookUpDictionary<IPlayerComponent>.RemoveTypes(dicts, types);

        int length = _playerComponentTypes.Length;

        for (int i = length - 1; i >= 0; --i)
        {
            Type type = _playerComponentTypes[i];

            if (!types.Contains(type))
                continue;

            --length;
            if (length - 1 == i)
            {
                continue;
            }

            for (int j = i; j <= length; ++j)
            {
                _playerComponentTypes[j] = _playerComponentTypes[j + 1];
            }
        }

        Type[] newTypes = new Type[length];
        Array.Copy(_playerComponentTypes, newTypes, length);
        _playerComponentTypes = newTypes;

        if (removedComponents == null)
            return;

        for (int i = 0; i < removedComponents.Length; ++i)
        {
            List<IPlayerComponent>? comps = removedComponents[i];
            if (comps is not { Count: > 0 })
                continue;

            DisposeComponents(comps);
        }
    }

    private static void DisposeComponents(IEnumerable<IPlayerComponent> comps)
    {
        foreach (IPlayerComponent comp in comps)
        {
            if (comp is Object o)
            {
                try
                {
                    Destroy(o);
                }
                catch (Exception ex)
                {
                    _logger!.LogError(ex, Properties.Resources.LogPlayerComponentFailedToDispose, Accessor.Formatter.Format(comp.GetType()), comp.Player.Steam64.m_SteamID);
                }
            }
            
            if (comp is IDisposable disp)
            {
                try
                {
                    disp.Dispose();
                }
                catch (Exception ex)
                {
                    _logger!.LogError(ex, Properties.Resources.LogPlayerComponentFailedToDispose, Accessor.Formatter.Format(comp.GetType()), comp.Player.Steam64.m_SteamID);
                }
            }
        }
    }

    internal void AddComponentTypes(IList<Type> types)
    {
        int oldTypeCount = _playerComponentTypes.Length;
        Type[] newArray = new Type[oldTypeCount + types.Count];

        Array.Copy(_playerComponentTypes, newArray, oldTypeCount);
        types.CopyTo(newArray, oldTypeCount);
        _playerComponentTypes = newArray;

        IPlayerComponent[][] allComps = new IPlayerComponent[ComponentDictionary.Count][];
        TypeLookUpDictionary<IPlayerComponent>[] dicts = new TypeLookUpDictionary<IPlayerComponent>[ComponentDictionary.Count];
        int index = 0;
        ArraySegment<Type> newTypes = new ArraySegment<Type>(newArray, oldTypeCount, newArray.Length - oldTypeCount);

        foreach (SpindlePlayerComponent comp in ComponentDictionary.Values)
        {
            dicts[index] = comp.Components;
            allComps[index] = CreateComponents(newTypes);
            ++index;
        }

        // ReSharper disable once CoVariantArrayConversion
        TypeLookUpDictionary<IPlayerComponent>.AddValueTypes(dicts, newTypes, allComps);
    }

    internal static SpindlePlayerComponent? Get(SteamPlayer pl)
    {
        ComponentDictionary.TryGetValue(pl.playerID.steamID.m_SteamID, out SpindlePlayerComponent comp);
        return comp;
    }
}
