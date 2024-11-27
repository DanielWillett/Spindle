using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Assets;
using Rocket.Core.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Rocket.Core.Plugins;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public class RocketPlugin : MonoBehaviour, IRocketPlugin
{
    [TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
    public delegate void PluginUnloading(IRocketPlugin plugin);

    [TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
    public delegate void PluginLoading(IRocketPlugin plugin, ref bool cancelLoading);

    [TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
    public delegate void ExecuteDependencyCodeDelegate(IRocketPlugin plugin);

    public static event PluginUnloading OnPluginUnloading;

    public static event PluginLoading OnPluginLoading;

    public IAsset<TranslationList> Translations { get; }
    public PluginState State { get; private set; }
    public Assembly Assembly { get; }
    public string Directory { get; }
    public string Name { get; }
    public virtual TranslationList DefaultTranslations => new TranslationList();

    public RocketPlugin()
    {
        Assembly = GetType().Assembly;
        Name = Assembly.GetName().Name;
        Directory = Path.Combine(Environment.PluginsDirectory, Name);
        if (!System.IO.Directory.Exists(Directory))
            System.IO.Directory.CreateDirectory(Directory);

        // ReSharper disable VirtualMemberCallInConstructor

        TranslationList defaults = DefaultTranslations;
        if (defaults is not { Count: > 0 })
            return;

        Translations = new XMLFileAsset<TranslationList>(Path.Combine(Directory, string.Format(Environment.PluginTranslationFileTemplate, Name, R.Settings.Instance.LanguageCode)), new Type[]
        {
            typeof (TranslationList),
            typeof (TranslationListEntry)
        }, defaults);
        defaults.AddUnknownEntries(Translations);

        // ReSharper restore VirtualMemberCallInConstructor
    }

    protected virtual void Load() { }

    protected virtual void Unload() { }

    public static bool IsDependencyLoaded(string plugin)
    {
        return R.Plugins.GetPlugin(plugin) != null;
    }

    public static void ExecuteDependencyCode(string plugin, ExecuteDependencyCodeDelegate a)
    {
        IRocketPlugin plugin1 = R.Plugins.GetPlugin(plugin);
        if (plugin1 != null)
        {
            a(plugin1);
        }
    }

    public string Translate(string translationKey, params object[] placeholder)
    {
        return Translations.Instance.Translate(translationKey, placeholder);
    }

    public void ReloadPlugin()
    {
        UnloadPlugin();
        LoadPlugin();
    }

    public virtual void LoadPlugin()
    {
        Logger.Log("\n[loading] " + Name, ConsoleColor.Cyan);
        Translations.Load(null);

        R.Commands.RegisterFromAssembly(Assembly);
        try
        {
            Load();
        }
        catch (Exception ex1)
        {
            Logger.LogError("Failed to load " + Name + ", unloading now... :" + ex1.ToString());
            try
            {
                UnloadPlugin(PluginState.Failure);
                return;
            }
            catch (Exception ex2)
            {
                Logger.LogError("Failed to unload " + Name + ":" + ex2.ToString());
            }
        }
        bool cancelLoading = false;
        if (OnPluginLoading != null)
        {
            foreach (PluginLoading pluginLoading in OnPluginLoading.GetInvocationList().Cast<PluginLoading>())
            {
                try
                {
                    pluginLoading(this, ref cancelLoading);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
                if (cancelLoading)
                {
                    try
                    {
                        UnloadPlugin(PluginState.Cancelled);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Failed to unload " + Name + ":" + ex.ToString());
                    }
                }
            }
        }

        State = PluginState.Loaded;
    }

    public virtual void UnloadPlugin(PluginState state = PluginState.Unloaded)
    {
        Logger.Log("\n[unloading] " + Name, ConsoleColor.Cyan);
        OnPluginUnloading.TryInvoke(this);
        R.Commands.DeregisterFromAssembly(Assembly);
        Unload();
        State = state;
    }

    public T TryAddComponent<T>() where T : Component
    {
        try
        {
            return gameObject.AddComponent<T>();
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occured while adding component {typeof(T).Name}", ex);
        }
    }

    public void TryRemoveComponent<T>() where T : Component
    {
        try
        {
            Destroy(gameObject.GetComponent<T>());
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occured while removing component {typeof(T).Name}", ex);
        }
    }
}