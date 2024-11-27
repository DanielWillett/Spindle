using Rocket.API;
using Rocket.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Rocket.Core.Plugins;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public sealed class RocketPluginManager : MonoBehaviour
{
    [TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
    public delegate void PluginsLoaded();

    public event PluginsLoaded OnPluginsLoaded;

    public List<IRocketPlugin> GetPlugins()
    {
        return new List<IRocketPlugin>();
    }

    public IRocketPlugin GetPlugin(Assembly assembly)
    {
        return new List<GameObject>().Select(g => g.GetComponent<IRocketPlugin>()).Where(p => p != null && p.GetType().Assembly == assembly).FirstOrDefault();
    }

    public IRocketPlugin GetPlugin(string name)
    {
        return new List<GameObject>().Select(g => g.GetComponent<IRocketPlugin>()).Where(p => p != null && p.Name == name).FirstOrDefault();
    }

    public Type GetMainTypeFromAssembly(Assembly assembly)
    {
        return RocketHelper.GetTypesFromInterface(assembly, "IRocketPlugin").FirstOrDefault<Type>();
    }

    public static Dictionary<string, string> GetAssembliesFromDirectory(string directory, string extension = "*.dll")
    {
        Dictionary<string, string> assembliesFromDirectory = new Dictionary<string, string>();
        foreach (FileInfo file in new DirectoryInfo(directory).GetFiles(extension, SearchOption.AllDirectories))
        {
            try
            {
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(file.FullName);
                assembliesFromDirectory.Add(assemblyName.FullName, file.FullName);
            }
            catch
            {
            }
        }

        return assembliesFromDirectory;
    }

    public static List<Assembly> LoadAssembliesFromDirectory(string directory, string extension = "*.dll")
    {
        List<Assembly> assemblyList = new List<Assembly>();
        foreach (FileInfo file in new DirectoryInfo(directory).GetFiles(extension, SearchOption.AllDirectories))
        {
            try
            {
                Assembly assembly = Assembly.LoadFile(file.FullName);
                if (RocketHelper.GetTypesFromInterface(assembly, "IRocketPlugin").FindAll(x => !x.IsAbstract).Count == 1)
                {
                    Logger.Log("Loading " + assembly.GetName().Name + " from " + assembly.Location);
                    assemblyList.Add(assembly);
                }
                else
                    Logger.LogError("Invalid or outdated plugin assembly: " + assembly.GetName().Name);
            }
            catch (Exception ex)
            {
                string v = "Could not load plugin assembly: " + file.Name;
                Logger.LogException(ex, v);
            }
        }
        return assemblyList;
    }
}
