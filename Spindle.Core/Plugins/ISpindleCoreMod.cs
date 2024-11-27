using System.ComponentModel.Design;

namespace Spindle.Plugins;

/// <summary>
/// Core mods are loaded on startup and can modify the Spindle service provider and perform patches before any plugins are loaded.
/// </summary>
public interface ISpindleCoreMod
{
    void Initialize(IServiceContainer spindleServiceCollection);

    void Shutdown();
}