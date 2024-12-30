using DanielWillett.ReflectionTools;
using Microsoft.Extensions.DependencyInjection;
using Spindle.Logging;
using Spindle.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Spindle.Plugins;

/// <summary>
/// Handles loading plugins and core mods for Spindle launcher.
/// </summary>
public class SpindlePluginLoader : IDisposable
{
    private readonly ILogger<SpindlePluginLoader> _logger;
    private readonly List<PluginAssembly> _assemblies = new List<PluginAssembly>();
    private readonly CancellationTokenSource _shutdownToken = new CancellationTokenSource();

    private List<Type> _allTypes;

    private bool _isShuttingDown;
    private bool _isShutDown;

    private readonly List<ISpindlePlugin> _plugins = new List<ISpindlePlugin>(16);
    private readonly List<ISpindleCoreMod> _coreMods = new List<ISpindleCoreMod>(4);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public SpindlePluginLoader(ILogger<SpindlePluginLoader> logger)
    {
        _logger = logger;
        _allTypes = Accessor.GetTypesSafe(SpindleLauncher.AssemblyCore);
    }
    
    internal void DiscoverPluginsFromFiles()
    {
        foreach (string file in Directory.EnumerateFiles(SpindlePaths.PluginsDirectory, "*.dll", SearchOption.AllDirectories))
        {
            try
            {
                DiscoverAssemblyIntl(file);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load plugin assembly \"{0}\".", Path.GetRelativePath(SpindlePaths.PluginsDirectory, file));
            }
        }

        UpdateTypeList();
    }

    private void DiscoverAssemblyIntl(string file)
    {
        Assembly asm = Assembly.Load(file);

        AssemblyName asmName = asm.GetName();

        List<Type>? plugins = null, coreMods = null;
        List<Type> types = Accessor.GetTypesSafe(asm, removeIgnored: false);

        bool anyCoreMods = false, anyPlugins = false;
        int ct = 0;

        foreach (Type type in types)
        {
            if (type.IsAbstract || type.IsIgnored())
            {
                continue;
            }

            ++ct;
            if (typeof(ISpindleCoreMod).IsAssignableFrom(type))
            {
                (coreMods ??= new List<Type>(1)).Add(type);
                anyCoreMods = true;
            }
            else if (typeof(ISpindlePlugin).IsAssignableFrom(type))
            {
                (plugins ??= new List<Type>(1)).Add(type);
                anyPlugins = true;
            }
        }


        switch (ct)
        {
            case 0:
                _logger.LogWarning("Skipped assembly \"{0}\". No plugins or core-mods available.", asmName.Name);
                break;

            case 1 when anyPlugins:
                _logger.LogInformation("Found plugin \"{0}\" from assembly \"{1}\".", plugins![0], asmName.Name);
                break;

            case 1 when anyCoreMods:
                _logger.LogInformation("Found core mod \"{0}\" from assembly {1}.", coreMods![0], asmName.Name);
                break;

            default:
                _logger.LogInformation("Found {0} plugins and/or core-mods in assembly {1}.", (plugins?.Count ?? 0) + (coreMods?.Count ?? 0), asmName.Name);
                break;
        }

        PluginAssembly assembly = new PluginAssembly
        {
            Assembly = asm,
            Path = Path.Combine(SpindlePaths.PluginsDirectory, Path.GetFileNameWithoutExtension(file)),
            CoreModTypes = coreMods,
            PluginTypes = plugins
        };

        _assemblies.Add(assembly);
    }

    /// <summary>
    /// Gets the folder for an assembly where plugin data is stored.
    /// </summary>
    /// <returns>The path to the folder, or <see langword="null"/> if the assembly wasn't loaded as a plugin.</returns>
    public string? GetAssemblyFolder(Assembly assembly, SpecialPluginFolder folderType = SpecialPluginFolder.Base)
    {
        PluginAssembly assemblyInfo = _assemblies.Find(x => x.Assembly == assembly);
        string? path = assemblyInfo.Path;
        if (path == null)
        {
            return null;
        }

        return folderType switch
        {
            SpecialPluginFolder.Localization => Path.Combine(path, "Localization"),
            _ => path
        };
    }

    private struct PluginAssembly
    {
        public Assembly Assembly;
        public string Path;
        public List<Type>? PluginTypes;
        public List<Type>? CoreModTypes;
    }

    private void UpdateTypeList()
    {
        IEnumerable<Assembly> allAssemblies = Enumerable.Repeat(SpindleLauncher.AssemblyCore, 1).Concat(_assemblies.Select(x => x.Assembly));

        _allTypes = Accessor.GetTypesSafe(allAssemblies, removeIgnored: false);
    }

    internal void RunCoreMods(ServiceContainer collection)
    {
        if (_isShuttingDown)
            throw new InvalidOperationException("Already shutting down.");

        int startIndex = _coreMods.Count;

        foreach (Type t in _assemblies.SelectMany(x => x.CoreModTypes).OrderByDescending(x => x.GetPriority()))
        {
            if (_coreMods.Any(x => x.GetType() == t))
                continue;

            object[] args;
            ConstructorInfo? ctor = t.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
            {
                ctor = t.GetConstructor([ typeof(ILogger<>).MakeGenericType(t) ]);
                if (ctor == null)
                {
                    ctor = t.GetConstructor([ typeof(ILogger) ]);
                    if (ctor == null)
                    {
                        _logger.LogError("Core mod {0} does not have a parameterless constructor (or constructor injecting a logger).", t);
                        continue;
                    }

                    args = [ new SimpleCommandWindowLogger(t) ];
                }
                else
                {
                    args = [ Activator.CreateInstance(typeof(SimpleCommandWindowLogger<>).MakeGenericType(t)) ];
                }
            }
            else
            {
                args = Array.Empty<object>();
            }

            try
            {
                ISpindleCoreMod coreMod = (ISpindleCoreMod)ctor.Invoke(args);
                _coreMods.Add(coreMod);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating core mod {0}.", t);
            }
        }

        for (int i = startIndex; i < _coreMods.Count; ++i)
        {
            ISpindleCoreMod coreMod = _coreMods[i];
            try
            {
                coreMod.Initialize(collection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting core mod {0}.", coreMod.GetType());
            }
        }
    }

    internal async UniTask RunPluginsAsync(IServiceProvider serviceProvider, CancellationToken token = default)
    {
        if (_isShuttingDown)
            throw new InvalidOperationException("Already shutting down.");

        using CancellationTokenSource srcCombined = CancellationTokenSource.CreateLinkedTokenSource(token, _shutdownToken.Token);

        token = srcCombined.Token;

        int startIndex = _plugins.Count;

        foreach (Type t in _assemblies.SelectMany(x => x.PluginTypes).OrderByDescending(x => x.GetPriority()))
        {
            if (_plugins.Any(x => x.GetType() == t))
                continue;

            try
            {
                ISpindlePlugin plugin = (ISpindlePlugin)ActivatorUtilities.CreateInstance(serviceProvider, t);
                _plugins.Add(plugin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting plugin {0}.", t);
            }
        }

        int i = startIndex;
        for (; i < _plugins.Count; ++i)
        {
            ISpindlePlugin plugin = _plugins[i];
            try
            {
                await plugin.StartAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading plugin {0}.", plugin.GetType());
                break;
            }
        }

        if (i < _plugins.Count)
        {
            _shutdownToken.Cancel();
            await ShutdownAsync(i, CancellationToken.None);
            throw new OperationCanceledException("Cancelling load because a plugin failed.");
        }
    }

    private class SimpleCommandWindowLogger<T> : SimpleCommandWindowLogger, ILogger<T>
    {
        public SimpleCommandWindowLogger() : base(Accessor.Formatter.Format<T>()) { }
    }

    private class SimpleCommandWindowLogger : ILogger, Microsoft.Extensions.Logging.ILogger
    {
        private readonly string _categoryName;
        public SimpleCommandWindowLogger(Type type) : this(Accessor.Formatter.Format(type)) { }

        protected SimpleCommandWindowLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            string message = "[" + _categoryName + "] " + formatter(state, exception);
            switch (logLevel)
            {
                case Microsoft.Extensions.Logging.LogLevel.Warning:
                    CommandWindow.LogWarning(message);
                    break;

                case Microsoft.Extensions.Logging.LogLevel.Error:
                case Microsoft.Extensions.Logging.LogLevel.Critical:
                    CommandWindow.LogError(message);
                    break;

                default:
                    CommandWindow.Log(message);
                    break;
            }
        }

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) => true;
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => throw new NotSupportedException();
    }

    public List<Type> GetTypesOfBaseType<T>(bool removeIgnored, bool allowAbstractTypes = false, bool allowValueTypes = true, bool allowSealedTypes = true, bool allowNestedTypes = true)
    {
        return GetTypesOfBaseType(typeof(T), removeIgnored, allowAbstractTypes, allowValueTypes, allowSealedTypes, allowNestedTypes);
    }

    public List<Type> GetTypesOfBaseType(Type baseType, bool removeIgnored, bool allowAbstractTypes = false, bool allowValueTypes = true, bool allowSealedTypes = true, bool allowNestedTypes = true)
    {
        List<Type> types = new List<Type>(32);
        foreach (Type t in _allTypes)
        {
            if (!allowAbstractTypes && t.IsAbstract)
                continue;

            if (!allowValueTypes && t.IsValueType)
                continue;

            if (!allowSealedTypes && t.IsSealed)
                continue;

            if (!allowNestedTypes && t.IsNested)
                continue;

            if (!removeIgnored || t.IsIgnored())
                continue;

            if (!baseType.IsAssignableFrom(t))
                continue;

            types.Add(t);
        }

        return types;
    }

    public Type? GetHighestPriorityTypeOfBaseType<T>(bool removeIgnored, bool allowAbstractTypes = false, bool allowValueTypes = true, bool allowSealedTypes = true, bool allowNestedTypes = true)
    {
        return GetHighestPriorityTypeOfBaseType(typeof(T), removeIgnored, allowAbstractTypes, allowValueTypes, allowSealedTypes, allowNestedTypes);
    }

    public Type? GetHighestPriorityTypeOfBaseType(Type baseType, bool removeIgnored, bool allowAbstractTypes = false, bool allowValueTypes = true, bool allowSealedTypes = true, bool allowNestedTypes = true)
    {
        foreach (Type t in _allTypes)
        {
            if (!allowAbstractTypes && t.IsAbstract)
                continue;

            if (!allowValueTypes && t.IsValueType)
                continue;

            if (!allowSealedTypes && t.IsSealed)
                continue;

            if (!allowNestedTypes && t.IsNested)
                continue;

            if (!removeIgnored || t.IsIgnored())
                continue;

            if (!baseType.IsAssignableFrom(t))
                continue;

            return t;
        }

        return null;
    }

    internal async UniTask ShutdownAsync(int startIndex = 0, CancellationToken token = default)
    {
        await UniTask.SwitchToMainThread(token);

        _isShuttingDown = true;

        UniTask[] tasks = new UniTask[_plugins.Count - startIndex];
        for (int i = startIndex; i < _plugins.Count; ++i)
        {
            tasks[i] = _plugins[i].EndAsync(CancellationToken.None).Preserve();
        }

        Task whenAllTask = UniTask.WhenAll(tasks).AsTask();

        try
        {
            await whenAllTask.ConfigureAwait(false);
            await UniTask.SwitchToMainThread();
            _logger.LogInformation(" {0} plugin(s) shut down successfully.", _plugins.Count - startIndex);
        }
        catch
        {
            LogFailedPluginUnload(startIndex, tasks);
        }
        finally
        {
            _isShutDown = true;
        }

        ShutdownCoreMods();
    }

    internal void HandleNonGracefulShutdownRequest()
    {
        if (_isShutDown)
            return;

        GameThread.AssertCurrent();

        UniTask[] tasks = new UniTask[_plugins.Count];
        for (int i = 0; i < _plugins.Count; ++i)
        {
            tasks[i] = _plugins[i].EndAsync(CancellationToken.None).Preserve();
        }

        Task whenAllTask = UniTask.WhenAll(tasks).AsTask();
        Task delayTask = Task.Delay(TimeSpan.FromSeconds(10d));

        Task.WhenAny(whenAllTask, delayTask).GetAwaiter().GetResult();

        if (!whenAllTask.IsCompletedSuccessfully)
        {
            LogFailedPluginUnload(0, tasks);
        }
        else
        {
            _logger.LogInformation(" {0} plugin(s) shut down successfully.", _plugins.Count);
        }

        ShutdownCoreMods();
    }

    private void ShutdownCoreMods()
    {
        int c = 0;
        for (int i = 0; i < _coreMods.Count; ++i)
        {
            ISpindleCoreMod coreMod = _coreMods[i];

            try
            {
                coreMod.Shutdown();
                ++c;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Core mod {0} failed to shutdown.", coreMod.GetType());
            }
        }

        if (c != 0)
        {
            _logger.LogInformation(" {0} core mod(s) shut down successfully.", c);
        }
    }

    private void LogFailedPluginUnload(int startIndex, UniTask[] tasks)
    {
        _logger.LogWarning("Shutting down plugins errored or timed out:");
        for (int i = 0; i < tasks.Length; ++i)
        {
            Type type = _plugins[startIndex + i].GetType();
            UniTask task = tasks[i];

            switch (task.Status)
            {
                case UniTaskStatus.Pending:
                    _logger.LogWarning(" {0} ({1}) - timed out", type, type.Assembly.GetName().Name);
                    break;

                case UniTaskStatus.Succeeded:
                    _logger.LogInformation(" {0} ({1}) - successfully shut down", type, type.Assembly.GetName().Name);
                    break;

                default:
                    _logger.LogError(task.AsTask().Exception, " {0} ({1}) - errored:", type, type.Assembly.GetName().Name);
                    break;
            }
        }
    }

    public void Dispose()
    {
        _shutdownToken.Dispose();
    }
}

public enum SpecialPluginFolder
{
    Base,
    Localization
}