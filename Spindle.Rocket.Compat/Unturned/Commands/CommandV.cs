using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Framework.Utilities;
using System;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class CommandV : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "v";
    public string Help => "Gives yourself an vehicle";
    public string Syntax => "<id>";
    public List<string> Permissions => new List<string>(2) { "rocket.v", "rocket.vehicle" };
    public List<string> Aliases => new List<string>();
    public void Execute(IRocketPlayer caller, string[] command)
    {
        UnturnedPlayer player = (UnturnedPlayer)caller;
        if (command.Length < 1)
        {
            UnturnedChat.Say(player, U.Translate("command_generic_invalid_parameter"));
            throw new WrongUsageOfCommandException(caller, this);
        }
        
        string itemString = command.GetParameterString(0);

        if (string.IsNullOrEmpty(itemString))
        {
            UnturnedChat.Say(player, U.Translate("command_generic_invalid_parameter"));
            throw new WrongUsageOfCommandException(caller, this);
        }

        VehicleAsset vehicleAsset;
        if (ushort.TryParse(itemString, out ushort id))
        {
            vehicleAsset = Assets.find(EAssetType.VEHICLE, id) as VehicleAsset;
        }
        else if (Guid.TryParse(itemString, out Guid guid))
        {
            vehicleAsset = Assets.find<VehicleAsset>(guid);
        }
        else
        {
            List<VehicleAsset> list = ListPool<VehicleAsset>.claim();
            try
            {
                Assets.find(list);
                list.RemoveAll(x => x.vehicleName == null);
                list.Sort((a, b) => a.vehicleName.Length.CompareTo(b.vehicleName.Length));

                vehicleAsset = list.Find(i => i.vehicleName.IndexOf(itemString, StringComparison.InvariantCultureIgnoreCase) >= 0);
            }
            finally
            {
                ListPool<VehicleAsset>.release(list);
            }
        }

        if (vehicleAsset == null)
        {
            UnturnedChat.Say(player, U.Translate("command_generic_invalid_parameter"));
            throw new WrongUsageOfCommandException(caller, this);
        }

        if (U.Settings.Instance.EnableVehicleBlacklist
            && !player.HasPermission("vehicleblacklist.bypass")
            && player.HasPermission("vehicle." + vehicleAsset.id))
        {
            UnturnedChat.Say(player, U.Translate("command_v_blacklisted"));
        }
        else
        {
            Vector3 positionForVehicle = player.Player.transform.position + player.Player.transform.forward * 6f;
            Physics.Raycast(positionForVehicle + new Vector3(0f, 16f, 0f), Vector3.down, out RaycastHit hitInfo, 32f, RayMasks.BLOCK_VEHICLE);

            if (hitInfo.collider != null)
                positionForVehicle.y = hitInfo.point.y + 16f;

            VehicleManager.spawnVehicleV2(vehicleAsset, positionForVehicle, player.Player.transform.rotation, null);

            Logger.Log(U.Translate("command_v_giving_console", player.CharacterName, vehicleAsset.id));
            UnturnedChat.Say(caller, U.Translate("command_v_giving_private", vehicleAsset.vehicleName, vehicleAsset.id));
        }
    }
}