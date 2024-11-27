using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Globalization;

namespace Rocket.Unturned.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class CommandInvestigate : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "investigate";
    public string Help => "Shows you the SteamID64 of a player";
    public string Syntax => "<player>";
    public List<string> Permissions => new List<string>(1) { "rocket.investigate" };
    public List<string> Aliases => new List<string>();
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length != 1)
        {
            UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
            throw new WrongUsageOfCommandException(caller, this);
        }

        SteamPlayer steamPlayer = PlayerTool.getSteamPlayer(command[0]);
        if (steamPlayer == null || caller is UnturnedPlayer up && up.CSteamID.m_SteamID == steamPlayer.playerID.steamID.m_SteamID)
        {
            UnturnedChat.Say(caller, U.Translate("command_generic_failed_find_player"));
            throw new WrongUsageOfCommandException(caller, this);
        }

        UnturnedChat.Say(caller, U.Translate("command_investigate_private",
            steamPlayer.playerID.characterName,
            steamPlayer.playerID.steamID.m_SteamID.ToString("D17", CultureInfo.InvariantCulture))
        );
    }
}