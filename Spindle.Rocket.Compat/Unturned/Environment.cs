using System.IO;

namespace Rocket.Unturned;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public static class Environment
{
    public static string RocketDirectory;
    public static readonly string SettingsFile = "Rocket.Unturned.config.xml";
    public static readonly string TranslationFile = "Rocket.Unturned.{0}.translation.xml";
    public static readonly string ConsoleFile = "{0}.console";

    public static void Initialize()
    {
        RocketDirectory = Path.Combine(UnturnedPaths.RootDirectory.FullName, "Servers", Provider.serverID, "Rocket");

        if (!Directory.Exists(RocketDirectory))
        {
            Directory.CreateDirectory(RocketDirectory);
        }

        Directory.SetCurrentDirectory(RocketDirectory);
    }
}