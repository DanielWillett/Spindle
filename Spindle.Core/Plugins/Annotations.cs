using System;

namespace Spindle.Plugins;

/// <summary>
/// Base type for all plugin modifier attributes.
/// </summary>
/// <remarks>This can be overridden by a plugin or library to add their own attributes.</remarks>
[BaseTypeRequired(typeof(ISpindlePlugin))]
public abstract class SpindlePluginAttribute : Attribute
{
    /// <summary>
    /// Applies this attribute's modification the the plugin settings.
    /// </summary>
    public abstract void Apply(SpindlePluginSettings pluginSettings);
}


/// <inheritdoc cref="SpindlePluginLoadTime"/>
[AttributeUsage(AttributeTargets.Class)]
public sealed class LoadTimeAttribute(SpindlePluginLoadTime loadTime = SpindlePluginLoadTime.AllAssetsLoaded) : SpindlePluginAttribute
{
    /// <inheritdoc cref="SpindlePluginLoadTime"/>
    public SpindlePluginLoadTime LoadTime { get; } = loadTime;

    public override void Apply(SpindlePluginSettings pluginSettings)
    {
        if (LoadTime is < SpindlePluginLoadTime.Early or > SpindlePluginLoadTime.Never)
        {
            throw new SpindlePluginSettingsException(pluginSettings.PluginType,
                string.Format(Properties.Resources.ExceptionSpindlePluginSettingsException_LoadTime, LoadTime.ToString()));
        }

        pluginSettings.LoadTime = LoadTime;
    }
}


/// <summary>
/// Defines when a plugin is loaded in relation to the server loading procedure.
/// </summary>
public enum SpindlePluginLoadTime
{
    /// <summary>
    /// Loaded right after the Spindle module loads.
    /// </summary>
    Early,

    /// <summary>
    /// Loaded after all assets have been loaded but before the level starts loading. <see cref="Level.info"/> is populated at this point.
    /// </summary>
    AllAssetsLoaded,

    /// <summary>
    /// Loaded after the level loads but before <see cref="Level.onPreLevelLoaded"/> is invoked.
    /// </summary>
    EarlyLevelLoaded,

    /// <summary>
    /// Loaded after the level loads completely. <see cref="Level.isLoaded"/> is <see langword="true"/>.
    /// </summary>
    LevelLoaded,

    /// <summary>
    /// The plugin will not be loaded automatically and will have to be manually loaded by another plugin.
    /// </summary>
    Never
}