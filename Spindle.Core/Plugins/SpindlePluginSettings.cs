using System;

namespace Spindle.Plugins;

/// <summary>
/// Configures settings for a <see cref="ISpindlePlugin"/>.
/// </summary>
public class SpindlePluginSettings
{
    public Type PluginType { get; }
    public SpindlePluginLoadTime LoadTime { get; set; } = SpindlePluginLoadTime.AllAssetsLoaded;

    public SpindlePluginSettings(Type pluginType)
    {
        PluginType = pluginType;
    }
}