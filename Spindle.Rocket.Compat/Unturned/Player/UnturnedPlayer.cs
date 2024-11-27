using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Steam;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Skills;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rocket.Unturned.Player;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public sealed class UnturnedPlayer : IRocketPlayer
{
    private readonly SDG.Unturned.Player _player;

    [Obsolete("Not used")]
    public Exception PlayerIsConsoleException;

    public string Id => CSteamID.ToString();

    public string DisplayName => CharacterName;

    public bool IsAdmin => _player.channel.owner.isAdmin;

    public Profile SteamProfile => new Profile(CSteamID.m_SteamID);

    public SDG.Unturned.Player Player => _player;

    public CSteamID CSteamID => _player.channel.owner.playerID.steamID;

    public float Ping => _player.channel.owner.ping;

    public bool VanishMode
    {
        get => Features.VanishMode;
        set => Features.VanishMode = value;
    }

    public bool GodMode
    {
        get => Features.GodMode;
        set => Features.GodMode = value;
    }

    public Vector3 Position => _player.transform.position;

    public EPlayerStance Stance => _player.stance.stance;

    public float Rotation => _player.transform.eulerAngles.y;

    public byte Stamina => _player.life.stamina;

    public string CharacterName => _player.channel.owner.playerID.characterName;

    public string SteamName => _player.channel.owner.playerID.playerName;

    public byte Infection
    {
        get => _player.life.virus;
        set
        {
            _player.life.askDisinfect(100);
            _player.life.askInfect(value);
        }
    }

    public uint Experience
    {
        get => _player.skills.experience;
        set => _player.skills.ServerSetExperience(value);
    }

    public int Reputation
    {
        get => _player.skills.reputation;
        set => _player.skills.askRep(value);
    }

    public byte Health => _player.life.health;

    public byte Hunger
    {
        get => _player.life.food;
        set
        {
            _player.life.askEat(100);
            _player.life.askStarve(value);
        }
    }

    public byte Thirst
    {
        get => _player.life.water;
        set
        {
            _player.life.askDrink(100);
            _player.life.askDehydrate(value);
        }
    }

    public bool Broken
    {
        get => _player.life.isBroken;
        set => _player.life.serverSetLegsBroken(value);
    }

    public bool Bleeding
    {
        get => _player.life.isBleeding;
        set => _player.life.serverSetBleeding(value);
    }

    public bool Dead => _player.life.isDead;


    public bool IsPro => _player.channel.owner.isPro;

    public InteractableVehicle CurrentVehicle => _player.movement.getVehicle();

    public bool IsInVehicle => _player.movement.getVehicle() != null;

    public Color Color
    {
        get
        {
            if (IsAdmin && !Provider.hideAdmins)
                return Palette.ADMIN;
            RocketPermissionsGroup permissionsGroup = R.Permissions.GetGroups(this, false).FirstOrDefault(g => g.Color != null && g.Color != "white");
            string colorName = string.Empty;
            if (permissionsGroup != null)
                colorName = permissionsGroup.Color;
            return UnturnedChat.GetColorFromName(colorName, Palette.COLOR_W);
        }
        set
        {
            // todo implement
        }
    }

    public UnturnedPlayerFeatures Features => _player == null ? null : _player.gameObject.transform.GetComponent<UnturnedPlayerFeatures>();
    public UnturnedPlayerEvents Events => _player == null ? null : _player.gameObject.transform.GetComponent<UnturnedPlayerEvents>();

    public string IP
    {
        get
        {
            if (_player == null)
                return "0.0.0.0";

            string addressString = _player.channel.owner.getAddressString(false);
            return !string.IsNullOrEmpty(addressString) ? addressString : "0.0.0.0";
        }
    }

    public PlayerInventory Inventory => _player.inventory;

    public CSteamID SteamGroupID => _player.channel.owner.playerID.group;


    private UnturnedPlayer(SteamPlayer player)
    {
        _player = player.player;
    }


    public bool Equals(UnturnedPlayer otherPlayer)
    {
        return otherPlayer != null && CSteamID.m_SteamID == otherPlayer.CSteamID.m_SteamID;
    }

    public override bool Equals(object obj) => Equals(obj as UnturnedPlayer);

    public override int GetHashCode() => CSteamID.GetHashCode();

    public T GetComponent<T>() => Player.GetComponent<T>();

    public static UnturnedPlayer FromName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        SteamPlayer player;
        if (ulong.TryParse(name, NumberStyles.Number, CultureInfo.InvariantCulture, out ulong steam64))
        {
            CSteamID steamId = new CSteamID(steam64);
            if (steamId.GetEAccountType() == EAccountType.k_EAccountTypeIndividual)
            {
                player = PlayerTool.getSteamPlayer(steamId);
                if (player != null)
                    return new UnturnedPlayer(player);
            }
        }

        player = PlayerTool.getSteamPlayer(name);
        return player == null ? null : new UnturnedPlayer(player);
    }

    public static UnturnedPlayer FromCSteamID(CSteamID cSteamID)
    {
        if (cSteamID.GetEAccountType() != EAccountType.k_EAccountTypeIndividual)
            return null;

        SteamPlayer pl = PlayerTool.getSteamPlayer(cSteamID);
        return pl == null ? null : new UnturnedPlayer(pl);
    }

    public static UnturnedPlayer FromPlayer(SDG.Unturned.Player player)
    {
        return player == null ? null : new UnturnedPlayer(player.channel.owner);
    }

    public static UnturnedPlayer FromSteamPlayer(SteamPlayer player)
    {
        return player.player == null ? null : new UnturnedPlayer(player);
    }

    public void TriggerEffect(ushort effectID)
    {
        if (_player == null || Assets.find(EAssetType.EFFECT, effectID) is not EffectAsset asset)
            return;

        TriggerEffectParameters effectParameters = new TriggerEffectParameters(asset)
        {
            position = _player.transform.position
        };

        effectParameters.SetRelevantPlayer(_player.channel.owner);
        EffectManager.triggerEffect(effectParameters);
    }

    public void MaxSkills() => _player.skills.ServerUnlockAllSkills();

    public string SteamGroupName()
    {
        FriendsGroupID_t friendsGroupIdT;
        friendsGroupIdT.m_FriendsGroupID = (short)SteamGroupID.m_SteamID;
        return SteamFriends.GetFriendsGroupName(friendsGroupIdT);
    }

    public int SteamGroupMembersCount()
    {
        FriendsGroupID_t friendsGroupIdT;
        friendsGroupIdT.m_FriendsGroupID = (short)SteamGroupID.m_SteamID;
        return SteamFriends.GetFriendsGroupMembersCount(friendsGroupIdT);
    }

    public SteamPlayer SteamPlayer()
    {
        return _player.channel.owner;
    }

    public bool GiveItem(ushort itemId, byte amount)
    {
        return ItemTool.tryForceGiveItem(_player, itemId, amount);
    }

    public bool GiveItem(Item item) => _player.inventory.tryAddItem(item, false);

    public bool GiveVehicle(ushort vehicleId) => VehicleTool.giveVehicle(_player, vehicleId);

    public void Kick(string reason)
    {
        Provider.kick(CSteamID, reason);
    }

    public void Ban(string reason, uint duration)
    {
        Ban(CSteamID.Nil, reason, duration);
    }

    public void Ban(CSteamID instigator, string reason, uint duration)
    {
        CSteamID csteamId = CSteamID;
        uint ip;
        IEnumerable<byte[]> hwids;
        if (_player == null)
        {
            ip = 0u;
            hwids = Enumerable.Empty<byte[]>();
        }
        else
        {
            ip = _player.channel.owner.getIPv4AddressOrZero();
            hwids = _player.channel.owner.playerID.GetHwids();
        }

        Provider.requestBanPlayer(instigator, csteamId, ip, hwids, reason, duration);
    }

    public void Admin(bool admin)
    {
        Admin(admin, null);
    }

    public void Admin(bool admin, UnturnedPlayer issuer)
    {
        if (admin)
        {
            CSteamID id = issuer?.CSteamID ?? CSteamID.Nil;
            SteamAdminlist.admin(CSteamID, id);
        }
        else
        {
            SteamAdminlist.unadmin(_player.channel.owner.playerID.steamID);
        }
    }

    public void Teleport(UnturnedPlayer target)
    {
        Vector3 position = target._player.transform.position;
        Quaternion rotation = target._player.transform.rotation;
        Vector3 eulerAngles = rotation.eulerAngles;

        Teleport(position, eulerAngles.y);
    }

    public void Teleport(Vector3 position, float rotation)
    {
        _player.teleportToLocation(position, rotation);
    }

    public bool Teleport(string nodeName)
    {
        IReadOnlyList<LocationDevkitNode> allNodes = LocationDevkitNodeSystem.Get().GetAllNodes();

        LocationDevkitNode node = allNodes.FirstOrDefault(n => n.name.Equals(nodeName, StringComparison.InvariantCultureIgnoreCase))
                                  ?? allNodes.FirstOrDefault(n => n.name.IndexOf(nodeName, StringComparison.InvariantCultureIgnoreCase) >= 0);

        if (node == null)
        {
            return false;
        }

        _player.teleportToLocation(node.transform.position + new Vector3(0.0f, 0.5f, 0.0f), Rotation);
        return true;
    }

    public void Heal(byte amount) => Heal(amount, null, null);

    public void Heal(byte amount, bool? bleeding, bool? broken)
    {
        _player.life.askHeal(amount, bleeding ?? _player.life.isBleeding, broken ?? _player.life.isBroken);
    }

    public void Suicide()
    {
        if (_player.life.IsAlive)
            _player.life.askDamage(100, Vector3.up * 10f, EDeathCause.SUICIDE, ELimb.SKULL, _player.channel.owner.playerID.steamID, out EPlayerKill _, bypassSafezone: true);
    }

    public EPlayerKill Damage(byte amount, Vector3 direction, EDeathCause cause, ELimb limb, CSteamID damageDealer)
    {
        _player.life.askDamage(amount, direction, cause, limb, damageDealer, out EPlayerKill eplayerKill);
        return eplayerKill;
    }

    public void SetSkillLevel(UnturnedSkill skill, byte level)
    {
        _player.skills.ServerSetSkillLevel((int)skill.Speciality, (int)skill.Skill, level);
    }

    public byte GetSkillLevel(UnturnedSkill skill)
    {
        return GetSkill(skill).level;
    }

    public Skill GetSkill(UnturnedSkill skill)
    {
        return _player.skills.skills[(int)skill.Speciality][(int)skill.Skill];
    }

    public int CompareTo(object obj)
    {
        return Id.CompareTo(obj);
    }

    public override string ToString() => CSteamID.ToString();
}