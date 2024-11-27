using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Player;

namespace Rocket.Unturned.Events;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public sealed class UnturnedPlayerEvents : UnturnedPlayerComponent
{
    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerUpdatePosition(UnturnedPlayer player, Vector3 position);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerUpdateBleeding(UnturnedPlayer player, bool bleeding);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerUpdateBroken(UnturnedPlayer player, bool broken);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerDead(UnturnedPlayer player, Vector3 position);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerUpdateLife(UnturnedPlayer player, byte life);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerUpdateFood(UnturnedPlayer player, byte food);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerUpdateHealth(UnturnedPlayer player, byte health);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerUpdateVirus(UnturnedPlayer player, byte virus);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerUpdateWater(UnturnedPlayer player, byte water);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public enum PlayerGesture
    {
        None,
        InventoryOpen,
        InventoryClose,
        Pickup,
        PunchLeft,
        PunchRight,
        SurrenderStart,
        SurrenderStop,
        Point,
        Wave,
        Salute,
        Arrest_Start,
        Arrest_Stop,
        Rest_Start,
        Rest_Stop,
        Facepalm,
    }

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerUpdateGesture(UnturnedPlayer player, PlayerGesture gesture);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerUpdateStance(UnturnedPlayer player, byte stance);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerRevive(UnturnedPlayer player, Vector3 position, byte angle);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerUpdateStat(UnturnedPlayer player, EPlayerStat stat);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerUpdateExperience(UnturnedPlayer player, uint experience);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerUpdateStamina(UnturnedPlayer player, byte stamina);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerInventoryUpdated(UnturnedPlayer player, InventoryGroup inventoryGroup, byte inventoryIndex, ItemJar P);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerInventoryResized(UnturnedPlayer player, InventoryGroup inventoryGroup, byte O, byte U);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerInventoryRemoved(UnturnedPlayer player, InventoryGroup inventoryGroup, byte inventoryIndex, ItemJar P);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerInventoryAdded(UnturnedPlayer player, InventoryGroup inventoryGroup, byte inventoryIndex, ItemJar P);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerChatted(UnturnedPlayer player, ref Color color, string message, EChatMode chatMode, ref bool cancel);

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public enum Wearables
    {
        Hat,
        Mask,
        Vest,
        Pants,
        Shirt,
        Glasses,
        Backpack,
    }

    [TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
    public delegate void PlayerWear(UnturnedPlayer player, Wearables wear, ushort id, byte? quality);

    // todo invoke
    protected override void Load() { }

    public static event PlayerUpdatePosition OnPlayerUpdatePosition;
    public static event PlayerUpdateBleeding OnPlayerUpdateBleeding;
    public event PlayerUpdateBleeding OnUpdateBleeding;
    public static event PlayerUpdateBroken OnPlayerUpdateBroken;
    public event PlayerUpdateBroken OnUpdateBroken;
    public static event PlayerDeath OnPlayerDeath;
    public event PlayerDeath OnDeath;
    public static event PlayerDead OnPlayerDead;
    public event PlayerDead OnDead;
    public static event PlayerUpdateLife OnPlayerUpdateLife;
    public event PlayerUpdateLife OnUpdateLife;
    public static event PlayerUpdateFood OnPlayerUpdateFood;
    public event PlayerUpdateFood OnUpdateFood;
    public static event PlayerUpdateHealth OnPlayerUpdateHealth;
    public event PlayerUpdateHealth OnUpdateHealth;
    public static event PlayerUpdateVirus OnPlayerUpdateVirus;
    public event PlayerUpdateVirus OnUpdateVirus;
    public static event PlayerUpdateWater OnPlayerUpdateWater;
    public event PlayerUpdateWater OnUpdateWater;
    public static event PlayerUpdateGesture OnPlayerUpdateGesture;
    public event PlayerUpdateGesture OnUpdateGesture;
    public static event PlayerUpdateStance OnPlayerUpdateStance;
    public event PlayerUpdateStance OnUpdateStance;
    public static event PlayerRevive OnPlayerRevive;
    public event PlayerRevive OnRevive;
    public static event PlayerUpdateStat OnPlayerUpdateStat;
    public event PlayerUpdateStat OnUpdateStat;
    public static event PlayerUpdateExperience OnPlayerUpdateExperience;
    public event PlayerUpdateExperience OnUpdateExperience;
    public static event PlayerUpdateStamina OnPlayerUpdateStamina;
    public event PlayerUpdateStamina OnUpdateStamina;
    public static event PlayerInventoryUpdated OnPlayerInventoryUpdated;
    public event PlayerInventoryUpdated OnInventoryUpdated;
    public static event PlayerInventoryResized OnPlayerInventoryResized;
    public event PlayerInventoryResized OnInventoryResized;
    public static event PlayerInventoryRemoved OnPlayerInventoryRemoved;
    public event PlayerInventoryRemoved OnInventoryRemoved;
    public static event PlayerInventoryAdded OnPlayerInventoryAdded;
    public event PlayerInventoryAdded OnInventoryAdded;
    public static event PlayerChatted OnPlayerChatted;
    public static event PlayerWear OnPlayerWear;
}