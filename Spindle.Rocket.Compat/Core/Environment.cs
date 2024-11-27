using System;

namespace Rocket.Core;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public static class Environment
{
    public static readonly string PluginsDirectory = "Plugins";
    public static readonly string LibrariesDirectory = "Libraries";
    public static readonly string LogsDirectory = "Logs";
    public static readonly string SettingsFile = "Rocket.config.xml";
    public static readonly string TranslationFile = "Rocket.{0}.translation.xml";
    public static readonly string LogFile = "Rocket.log";
    public static readonly string LogBackupFile = "Rocket.{0}.log";
    public static readonly string PermissionFile = "Permissions.config.xml";
    public static readonly string CommandsFile = "Commands.config.xml";
    public static readonly string PluginTranslationFileTemplate = "{0}.{1}.translation.xml";
    public static readonly string PluginConfigurationFileTemplate = "{0}.configuration.xml";
    public static readonly string WebConfigurationTemplate = "{0}?configuration={1}&instance={2}";

    [Obsolete("Unsupported")]
    public static void Initialize()
    {

    }
}