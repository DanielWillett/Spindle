using SDG.NetTransport;
using Spindle.Interaction;
using Spindle.Localization;
using Spindle.Threading;
using Spindle.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Spindle.Players;

/// <summary>
/// Light-weight wrapper for the Unturned player classes that can be created easily from any player class using <see cref="Create(Player)"/> or it's overloads.
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 24)]
public partial struct SpindlePlayer :
    IFormattable,

    IComparable,
    IComparable<Player>,
    IComparable<SteamPlayer>,
    IComparable<SteamPlayerID>,
    IComparable<PlayerCaller>,
    IComparable<SpindlePlayer>,
    IComparable<CSteamID>,
    IComparable<PlayerNames>,
    IComparable<ulong>,
    IComparable<IInteractionUser>,

    IEquatable<Player>,
    IEquatable<SteamPlayer>,
    IEquatable<SteamPlayerID>,
    IEquatable<PlayerCaller>,
    IEquatable<SpindlePlayer>,
    IEquatable<CSteamID>,
    IEquatable<PlayerNames>,
    IEquatable<ulong>,
    IEquatable<IInteractionUser>
{
    public static readonly SpindlePlayer None = default;

#nullable disable
    [FieldOffset(16)]
    private readonly SpindlePlayerComponent _playerComponent;

    /// <summary>
    /// The player's steam ID as a <see cref="CSteamID"/>.
    /// </summary>
    [FieldOffset(8)]
    public readonly CSteamID Steam64;

    /// <summary>
    /// The <see cref="SDG.Unturned.SteamPlayer"/> object for this player.
    /// </summary>
    /// <remarks>Usually won't be null unless the player was offline.</remarks>
    [FieldOffset(0)]
    public readonly SteamPlayer SteamPlayer;

    /// <summary>
    /// If the player is currently online.
    /// </summary>
    /// <remarks>While this property is thread-safe, it shouldn't be assumed that a player is still online after checking this unless the caller is on the main thread.</remarks>
    public readonly bool IsOnline => _playerComponent is { IsOnline: true };
#nullable restore

    /// <summary>
    /// Provides a reference to the player's 3 different names.
    /// </summary>
    public readonly ref readonly PlayerNames Names
    {
        get
        {
            if (_playerComponent is null)
                throw new PlayerOfflineException(Steam64);

            return ref _playerComponent.Names;
        }
    }

    /// <summary>
    /// Server-unique Net ID for the this player's <see cref="Player"/> component.
    /// </summary>
    public readonly NetId NetId => SteamPlayer.player.GetNetId();

    /// <summary>
    /// The base player component for Unturned's vanilla player.
    /// </summary>
    public readonly Player UnturnedPlayer => SteamPlayer.player;
    
    /// <summary>
    /// Handles player gestures and animations.
    /// </summary>
    public readonly PlayerAnimator UnturnedAnimator => SteamPlayer.player.animator;
    
    /// <summary>
    /// Handles the player's equipped clothes.
    /// </summary>
    public readonly PlayerClothing UnturnedClothing => SteamPlayer.player.clothing;
    
    /// <summary>
    /// Handles the player's inventory pages and slots.
    /// </summary>
    public readonly PlayerInventory UnturnedInventory => SteamPlayer.player.inventory;
    
    /// <summary>
    /// Handles the player's held item, useables, and hotkeys.
    /// </summary>
    public readonly PlayerEquipment UnturnedEquipment => SteamPlayer.player.equipment;
    
    /// <summary>
    /// Handles the player's vitals and life state.
    /// </summary>
    public readonly PlayerLife UnturnedLife => SteamPlayer.player.life;
    
    /// <summary>
    /// Handles the player's crafting recipes.
    /// </summary>
    public readonly PlayerCrafting UnturnedCrafting => SteamPlayer.player.crafting;
    
    /// <summary>
    /// Handles the player's skill levels.
    /// </summary>
    public readonly PlayerSkills UnturnedSkills => SteamPlayer.player.skills;
    
    /// <summary>
    /// Handles the player's regions and movement.
    /// </summary>
    public readonly PlayerMovement UnturnedMovement => SteamPlayer.player.movement;
    
    /// <summary>
    /// Handles the player's ability to look around and raycasting what they're looking at.
    /// </summary>
    public readonly PlayerLook UnturnedLook => SteamPlayer.player.look;
    
    /// <summary>
    /// Handles the player's current stance and height clearance checking.
    /// </summary>
    public readonly PlayerStance UnturnedStance => SteamPlayer.player.stance;
    
    /// <summary>
    /// Handles the player's key input, including plugin keys.
    /// </summary>
    public readonly PlayerInput UnturnedInput => SteamPlayer.player.input;
    
    /// <summary>
    /// Handles the player's voice chat.
    /// </summary>
    public readonly PlayerVoice UnturnedVoice => SteamPlayer.player.voice;
    
    /// <summary>
    /// Handles the player's interaction with interactable objects.
    /// </summary>
    public readonly PlayerInteract UnturnedInteract => SteamPlayer.player.interact;
    
    /// <summary>
    /// Handles the player's TPV placeable editor.
    /// </summary>
    public readonly PlayerWorkzone UnturnedWorkzone => SteamPlayer.player.workzone;
    
    /// <summary>
    /// Handles the player's quests and group.
    /// </summary>
    public readonly PlayerQuests UnturnedQuests => SteamPlayer.player.quests;

    /// <summary>
    /// Stores information about the player's current localization settings.
    /// </summary>
    public readonly LocalizationPlayerComponent Localization => Component<LocalizationPlayerComponent>();

    /// <summary>
    /// The transport connection used to send data directly to the player.
    /// </summary>
    public readonly ITransportConnection TransportConnection => SteamPlayer.transportConnection;

    /// <summary>
    /// The world position of the player.
    /// </summary>
    /// <exception cref="GameThreadException">Setter must be on main thread.</exception>
    /// <exception cref="PlayerOfflineException"/>
    public readonly Vector3 Position
    {
        get
        {
            AssertOnline();

            return SteamPlayer.player.transform.position;
        }
        set
        {
            GameThread.AssertCurrent();

            AssertOnline();

            Player player = SteamPlayer.player;
            player.teleportToLocationUnsafe(value - new Vector3(0f, 0.5f, 0f), player.transform.eulerAngles.y);
        }
    }

    /// <summary>
    /// The global rotation of the player.
    /// </summary>
    /// <remarks>Only the yaw will actually be replicated.</remarks>
    /// <exception cref="GameThreadException">Setter must be on main thread.</exception>
    /// <exception cref="PlayerOfflineException"/>
    public readonly Quaternion Rotation
    {
        get
        {
            AssertOnline();

            return SteamPlayer.player.transform.rotation;
        }
        set
        {
            GameThread.AssertCurrent();

            AssertOnline();

            Vector3 eulerAngles = value.eulerAngles;
            Player player = SteamPlayer.player;
            player.teleportToLocationUnsafe(player.transform.position - new Vector3(0f, 0.5f, 0f), eulerAngles.y);
        }
    }

    /// <summary>
    /// The global rotation (Y-axis) of the player.
    /// </summary>
    /// <exception cref="GameThreadException">Setter must be on main thread.</exception>
    /// <exception cref="PlayerOfflineException"/>
    public readonly float Yaw
    {
        get
        {
            AssertOnline();

            return SteamPlayer.player.transform.eulerAngles.y;
        }
        set
        {
            GameThread.AssertCurrent();

            AssertOnline();

            Player player = SteamPlayer.player;
            player.teleportToLocationUnsafe(player.transform.position - new Vector3(0f, 0.5f, 0f), value);
        }
    }

    private SpindlePlayer(SteamPlayer player)
    {
        SteamPlayer = player ?? throw new ArgumentNullException(nameof(player));
        Steam64 = player.playerID.steamID;
        _playerComponent = SpindlePlayerComponent.Get(player);
    }

    private SpindlePlayer(CSteamID steam64)
    {
        Steam64 = steam64;
    }

    /// <summary>
    /// Creates a <see cref="SpindlePlayer"/> from a <see cref="Player"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    public static SpindlePlayer Create(Player player)
    {
        if (player is null)
            throw new ArgumentNullException(nameof(player));

        return new SpindlePlayer(player.channel.owner);
    }

    /// <summary>
    /// Creates a <see cref="SpindlePlayer"/> from a <see cref="Player"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    public static SpindlePlayer Create(PlayerCaller player)
    {
        if (player is null)
            throw new ArgumentNullException(nameof(player));

        return new SpindlePlayer(player.player.channel.owner);
    }

    /// <summary>
    /// Creates a <see cref="SpindlePlayer"/> from a <see cref="SDG.Unturned.SteamPlayer"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    public static SpindlePlayer Create(SteamPlayer player)
    {
        if (player == null)
            throw new ArgumentNullException(nameof(player));

        return new SpindlePlayer(player);
    }

    /// <summary>
    /// Creates a <see cref="SpindlePlayer"/> from a <see cref="CSteamID"/>.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="steam64"/> is not a valid individual Steam64 ID.</exception>
    /// <remarks>If the player is not online, an offline <see cref="SpindlePlayer"/> will be returned.</remarks>
    public static SpindlePlayer Create(CSteamID steam64)
    {
        if (GameThread.IsCurrent)
        {
            List<SteamPlayer> players = Provider.clients;
            int c = players.Count;
            ulong s64 = steam64.m_SteamID;
            for (int i = 0; i < c; ++i)
            {
                SteamPlayer player = players[i];
                if (player.playerID.steamID.m_SteamID == s64)
                {
                    return new SpindlePlayer(player);
                }
            }
        }
        else if (ThreadSafePlayerList.OnlinePlayers.TryGetValue(steam64.m_SteamID, out SteamPlayer pl))
        {
            return new SpindlePlayer(pl);
        }

        if (steam64.GetEAccountType() != EAccountType.k_EAccountTypeIndividual)
            throw new ArgumentException(Properties.Resources.ExceptionSteamIdNotIndividual, nameof(steam64));

        return new SpindlePlayer(steam64);
    }

    /// <summary>
    /// Creates a <see cref="SpindlePlayer"/> from a <see cref="ulong"/>.
    /// </summary>
    /// <remarks>If the player is not online, an offline <see cref="SpindlePlayer"/> will be returned.</remarks>
    public static SpindlePlayer Create(ulong steam64)
    {
        if (GameThread.IsCurrent)
        {
            List<SteamPlayer> players = Provider.clients;
            int c = players.Count;
            for (int i = 0; i < c; ++i)
            {
                SteamPlayer player = players[i];
                if (player.playerID.steamID.m_SteamID == steam64)
                {
                    return new SpindlePlayer(player);
                }
            }
        }
        else if (ThreadSafePlayerList.OnlinePlayers.TryGetValue(steam64, out SteamPlayer pl))
        {
            return new SpindlePlayer(pl);
        }

        CSteamID steamId = Unsafe.As<ulong, CSteamID>(ref steam64);

        if (steamId.GetEAccountType() != EAccountType.k_EAccountTypeIndividual)
            throw new ArgumentException(Properties.Resources.ExceptionSteamIdNotIndividual, nameof(steam64));

        return new SpindlePlayer(steamId);
    }

    /// <summary>
    /// Get a component type linked to this player.
    /// </summary>
    /// <exception cref="ComponentNotFoundException">The component is not present on this player.</exception>
    public readonly TComponent Component<TComponent>() where TComponent : IPlayerComponent
    {
        if (_playerComponent is null)
            throw new PlayerOfflineException(Steam64);
        return _playerComponent.Components.Get<TComponent, SpindlePlayer>(this);
    }

    /// <summary>
    /// Get a component type linked to this player, or <see langword="null"/> if it isn't available.
    /// </summary>
    public readonly TComponent? ComponentOrNull<TComponent>() where TComponent : IPlayerComponent
    {
        if (_playerComponent is null)
            throw new PlayerOfflineException(Steam64);
        _playerComponent.Components.TryGet(out TComponent? comp);
        return comp;
    }

    /// <summary>
    /// Throws an exception if this player is offline.
    /// </summary>
    /// <exception cref="PlayerOfflineException"/>
    public readonly void AssertOnline()
    {
        if (!IsOnline || SteamPlayer.player == null)
            throw new PlayerOfflineException(Steam64);
    }

    public static implicit operator Player(SpindlePlayer player) => player.UnturnedPlayer;
    public static implicit operator SteamPlayer(SpindlePlayer player) => player.SteamPlayer;
    public static implicit operator CSteamID(SpindlePlayer player) => player.Steam64;

    public static implicit operator SpindlePlayer(Player player) => new SpindlePlayer(player.channel.owner);
    public static implicit operator SpindlePlayer(SteamPlayer player) => new SpindlePlayer(player);

    /// <summary>
    /// Compare two players by Steam64 ID.
    /// </summary>
    public static bool operator ==(SpindlePlayer left, SpindlePlayer right) => left.Steam64.m_SteamID == right.Steam64.m_SteamID;

    /// <summary>
    /// Compare two players by Steam64 ID.
    /// </summary>
    public static bool operator !=(SpindlePlayer left, SpindlePlayer right) => left.Steam64.m_SteamID != right.Steam64.m_SteamID;

    /// <summary>
    /// If the player is currently online.
    /// </summary>
    /// <remarks>While this operator is thread-safe, it shouldn't be assumed that a player is still online after checking this unless the caller is on the main thread.</remarks>
    public static bool operator true(SpindlePlayer player) => player.IsOnline;

    /// <summary>
    /// If the player is currently online.
    /// </summary>
    /// <remarks>While this operator is thread-safe, it shouldn't be assumed that a player is still online after checking this unless the caller is on the main thread.</remarks>
    public static bool operator false(SpindlePlayer player) => !player.IsOnline;

    /// <summary>
    /// Gets the SteamID as a 17-digit string.
    /// </summary>
    public readonly override string ToString() => Steam64.m_SteamID.ToString("D17", CultureInfo.InvariantCulture);

    /// <summary>
    /// Gets the SteamID as a 17-digit string.
    /// </summary>
    public readonly string ToString(IFormatProvider formatProvider) => Steam64.m_SteamID.ToString("D17", formatProvider ?? CultureInfo.InvariantCulture);

    /// <summary>
    /// Gets the SteamID as a string.
    /// </summary>
    public readonly string ToString(string? format, IFormatProvider? formatProvider) => Steam64.m_SteamID.ToString(format ?? "D17", formatProvider ?? CultureInfo.InvariantCulture);

    /// <inheritdoc />
    public readonly override int GetHashCode() => unchecked ( (int)Steam64.m_SteamID );

    /// <inheritdoc />
    public readonly bool Equals(Player other) => other.channel.owner.playerID.steamID.m_SteamID == Steam64.m_SteamID;

    /// <inheritdoc />
    public readonly bool Equals(SteamPlayer other) => other.playerID.steamID.m_SteamID == Steam64.m_SteamID;

    /// <inheritdoc />
    public readonly bool Equals(SteamPlayerID other) => other.steamID.m_SteamID == Steam64.m_SteamID;

    /// <inheritdoc />
    public readonly bool Equals(PlayerCaller other) => other.player.channel.owner.playerID.steamID.m_SteamID == Steam64.m_SteamID;

    /// <inheritdoc />
    public readonly bool Equals(SpindlePlayer other) => other.Steam64.m_SteamID == Steam64.m_SteamID;

    /// <inheritdoc />
    public readonly bool Equals(IInteractionUser other) => other.Steam64.m_SteamID == Steam64.m_SteamID;

    /// <inheritdoc />
    public readonly bool Equals(CSteamID other) => other.m_SteamID == Steam64.m_SteamID;

    /// <inheritdoc />
    public readonly bool Equals(ulong other) => other == Steam64.m_SteamID;

    /// <inheritdoc />
    public readonly bool Equals(PlayerNames other) => other.Steam64.m_SteamID == Steam64.m_SteamID;

    /// <inheritdoc />
    public readonly int CompareTo(Player other) => Steam64.m_SteamID.CompareTo(other.channel.owner.playerID.steamID.m_SteamID);

    /// <inheritdoc />
    public readonly int CompareTo(SteamPlayer other) => Steam64.m_SteamID.CompareTo(other.playerID.steamID.m_SteamID);

    /// <inheritdoc />
    public readonly int CompareTo(SteamPlayerID other) => Steam64.m_SteamID.CompareTo(other.steamID.m_SteamID);

    /// <inheritdoc />
    public readonly int CompareTo(PlayerCaller other) => Steam64.m_SteamID.CompareTo(other.player.channel.owner.playerID.steamID.m_SteamID);

    /// <inheritdoc />
    public readonly int CompareTo(SpindlePlayer other) => Steam64.m_SteamID.CompareTo(other.Steam64.m_SteamID);

    /// <inheritdoc />
    public readonly int CompareTo(CSteamID other) => Steam64.m_SteamID.CompareTo(other.m_SteamID);

    /// <inheritdoc />
    public readonly int CompareTo(PlayerNames other) => Steam64.m_SteamID.CompareTo(other.Steam64.m_SteamID);

    /// <inheritdoc />
    public readonly int CompareTo(IInteractionUser other) => Steam64.m_SteamID.CompareTo(other.Steam64.m_SteamID);

    /// <inheritdoc />
    public readonly int CompareTo(ulong other) => Steam64.m_SteamID.CompareTo(other);

    /// <inheritdoc />
    public readonly int CompareTo(object obj)
    {
        return obj switch
        {
            SpindlePlayer p => Steam64.m_SteamID.CompareTo(p.Steam64.m_SteamID),
            Player p => Steam64.m_SteamID.CompareTo(p.channel.owner.playerID.steamID.m_SteamID),
            SteamPlayer p => Steam64.m_SteamID.CompareTo(p.playerID.steamID.m_SteamID),
            SteamPlayerID p => Steam64.m_SteamID.CompareTo(p.steamID.m_SteamID),
            PlayerCaller p => Steam64.m_SteamID.CompareTo(p.player.channel.owner.playerID.steamID.m_SteamID),
            IInteractionUser p => Steam64.m_SteamID.CompareTo(p.Steam64.m_SteamID),
            CSteamID s => Steam64.m_SteamID.CompareTo(s.m_SteamID),
            ulong s => Steam64.m_SteamID.CompareTo(s),
            PlayerNames s => Steam64.m_SteamID.CompareTo(s.Steam64.m_SteamID),
            _ => 1
        };
    }


    /// <inheritdoc />
    public readonly override bool Equals(object? obj)
    {
        return obj switch
        {
            SpindlePlayer p => p.Steam64.m_SteamID == Steam64.m_SteamID,
            Player p => p.channel.owner.playerID.steamID.m_SteamID == Steam64.m_SteamID,
            SteamPlayer p => p.playerID.steamID.m_SteamID == Steam64.m_SteamID,
            SteamPlayerID p => p.steamID.m_SteamID == Steam64.m_SteamID,
            PlayerCaller p => p.player.channel.owner.playerID.steamID.m_SteamID == Steam64.m_SteamID,
            IInteractionUser p => p.Steam64.m_SteamID == Steam64.m_SteamID,
            CSteamID s => s.m_SteamID == Steam64.m_SteamID,
            ulong s => s == Steam64.m_SteamID,
            PlayerNames s => s.Steam64.m_SteamID == Steam64.m_SteamID,
            _ => false
        };
    }
}