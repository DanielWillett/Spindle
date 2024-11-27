using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Chat;
using Spindle;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Unturned.Commands;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class CommandRocket : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "rocket";
    public string Help => "Reloading Rocket or individual plugins";
    public string Syntax => "<plugins | reload> | <reload | unload | load> <plugin>";
    public List<string> Permissions => new List<string>(2) { "rocket.info", "rocket.rocket" };
    public List<string> Aliases => new List<string>();
    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length == 0)
        {
            // todo version info
            UnturnedChat.Say(caller, $"Spindle v{SpindleLauncher.AssemblyCore.GetName().Version.ToString(3)} compatibility layer for Legally Distinct Missile (previously RocketMod) running Unturned v{Provider.APP_VERSION}");
            return;
        }

        if (command.Length == 1)
        {
            switch (command[0].ToLower())
            {
                case "plugins":
                    if (!HasCommandPermission(caller, "plugins"))
                    {
                        UnturnedChat.Say(caller, R.Translate("command_no_permission"));
                        return;
                    }

                    List<IRocketPlugin> plugins = R.Plugins.GetPlugins();
                    UnturnedChat.Say(caller, U.Translate("command_rocket_plugins_loaded",    string.Join(", ", plugins.Where(p => p.State == PluginState.Loaded)   .Select(p => p.GetType().Assembly.GetName().Name))));
                    UnturnedChat.Say(caller, U.Translate("command_rocket_plugins_unloaded",  string.Join(", ", plugins.Where(p => p.State == PluginState.Unloaded) .Select(p => p.GetType().Assembly.GetName().Name))));
                    UnturnedChat.Say(caller, U.Translate("command_rocket_plugins_failure",   string.Join(", ", plugins.Where(p => p.State == PluginState.Failure)  .Select(p => p.GetType().Assembly.GetName().Name))));
                    UnturnedChat.Say(caller, U.Translate("command_rocket_plugins_cancelled", string.Join(", ", plugins.Where(p => p.State == PluginState.Cancelled).Select(p => p.GetType().Assembly.GetName().Name))));
                    return;
            }
        }

        if (!HasCommandPermission(caller, command[0]))
        {
            UnturnedChat.Say(caller, R.Translate("command_no_permission"));
            return;
        }

        UnturnedChat.Say(caller, U.Translate("command_rocket_reload_disabled"));
    }

    private static bool HasCommandPermission(IRocketPlayer caller, string perm)
    {
        return caller.HasPermission("rocket." + perm) || caller.HasPermission("rocket.rocket." + perm)
            || caller.HasPermission("info." + perm) || caller.HasPermission("rocket.info." + perm);
    }
}