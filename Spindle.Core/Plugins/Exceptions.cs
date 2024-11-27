using System;
using System.Runtime.Serialization;

namespace Spindle.Plugins;

/// <summary>
/// Thrown when a plugin has invalid configuration data in it's attributes or configuration.
/// </summary>
[Serializable]
public class SpindlePluginSettingsException : Exception
{
    /// <summary>
    /// Full type name of the plugin's type.
    /// </summary>
    public string? PluginTypeName { get; set; }

    /// <summary>
    /// Full assembly name of the plugin's type.
    /// </summary>
    public string? PluginAssemblyName { get; set; }

    public SpindlePluginSettingsException(Type pluginType)
        : this (pluginType, Properties.Resources.ExceptionSpindlePluginSettingsException) { }

    public SpindlePluginSettingsException(Type pluginType, string message)
        : this(pluginType, message, null) { }

    public SpindlePluginSettingsException(Type pluginType, string message, Exception? inner)
        : base(message, inner)
    {
        PluginTypeName = pluginType.FullName;
        PluginAssemblyName = pluginType.Assembly.FullName;
    }

    protected SpindlePluginSettingsException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        PluginTypeName = info.GetString("PluginTypeName");
        PluginAssemblyName = info.GetString("PluginAssemblyName");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("PluginTypeName", PluginTypeName);
        info.AddValue("PluginAssemblyName", PluginAssemblyName);
        base.GetObjectData(info, context);
    }
}