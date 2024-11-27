﻿using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class CommandAdmin : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "admin";
    public string Help => "Give a player admin privileges";
    public string Syntax => string.Empty;
    public List<string> Permissions => new List<string>(1) { "rocket.admin" };
    public List<string> Aliases => new List<string>();
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (R.Settings.Instance.WebPermissions.Enabled)
            return;

        UnturnedPlayer unturnedPlayerParameter = command.GetUnturnedPlayerParameter(0);
        if (unturnedPlayerParameter == null)
        {
            UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
            throw new WrongUsageOfCommandException(caller, this);
        }

        if (unturnedPlayerParameter.IsAdmin)
            return;
        
        UnturnedChat.Say(caller, "Successfully admined " + unturnedPlayerParameter.CharacterName);
        unturnedPlayerParameter.Admin(true);
    }
}