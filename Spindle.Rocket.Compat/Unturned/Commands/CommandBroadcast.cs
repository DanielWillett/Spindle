using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Unturned.Chat;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class CommandBroadcast : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "broadcast";
    public string Help => "Broadcast a message";
    public string Syntax => "<color> <message>";
    public List<string> Permissions => new List<string>(1) { "rocket.broadcast" };
    public List<string> Aliases => new List<string>();
    public void Execute(IRocketPlayer caller, string[] command)
    {
        Color? colorParameter = command.GetColorParameter(0);

        int startingIndex = 1;
        if (!colorParameter.HasValue)
            startingIndex = 0;

        string parameterString = command.GetParameterString(startingIndex);
        if (parameterString == null)
        {
            UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
            throw new WrongUsageOfCommandException(caller, this);
        }

        UnturnedChat.Say(parameterString, colorParameter ?? Color.green);
    }
}
