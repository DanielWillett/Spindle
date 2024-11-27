using System.IO;

namespace Spindle;

/// <summary>
/// Contains full relevant paths.
/// </summary>
public static class SpindlePaths
{
    /// <summary>
    /// Full path to the folder located at <c>U3DS/Servers/[SERVER ID]/</c>.
    /// </summary>
    public static string ServerDirectory { get; } = Path.Combine(UnturnedPaths.RootDirectory.FullName, "Servers", Provider.serverID);

    /// <summary>
    /// Full path to the folder located at <c>U3DS/Servers/[SERVER ID]/Spindle</c>.
    /// </summary>
    public static string SpindleDirectory { get; } = Path.Combine(ServerDirectory, "Spindle");

    /// <summary>
    /// Full path to the folder located at <c>U3DS/Servers/[SERVER ID]/Spindle/Plugins</c>.
    /// </summary>
    public static string PluginsDirectory { get; } = Path.Combine(SpindleDirectory, "Plugins");

    /// <summary>
    /// Full path to the folder located at <c>U3DS/Servers/[SERVER ID]/Spindle/Libraries</c>.
    /// </summary>
    public static string LibrariesDirectory { get; } = Path.Combine(SpindleDirectory, "Libraries");

    /// <summary>
    /// Full path to the folder located at <c>U3DS/Servers/[SERVER ID]/Spindle/Localization</c>.
    /// </summary>
    public static string LocalizationDirectory { get; } = Path.Combine(SpindleDirectory, "Localization");

    internal static string RocketDirectory { get; } = Path.Combine(ServerDirectory, "Rocket");
}