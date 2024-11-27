using DanielWillett.ReflectionTools;
using DanielWillett.ReflectionTools.Formatting;
using HarmonyLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using SDG.Framework.Modules;
using Spindle.Localization;
using Spindle.Logging;
using Spindle.Players;
using Spindle.Plugins;
using Spindle.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using Spindle.Interaction.Commands;

namespace Spindle;

/// <summary>
/// Spindle is a simple plugin loader with full compatibility with Legally Distinct Missile (RocketMod) plugins which can run alongside OpenMod if it's installed.
/// </summary>
public sealed class SpindleLauncher
{
#nullable disable

    internal static readonly Harmony Patcher = new Harmony("spindle");

    private static ILogger<SpindleLauncher> _logger;
    private static IConfigurationRoot _spindleConfiguration;
    private static ServiceContainer _serviceProvider;
    private static CancellationTokenSource _tokenSource;
    private static bool _isStartupLoaded;

#nullable restore

    /// <summary>
    /// If the Spindle launcher module has been loaded.
    /// </summary>
    public static bool IsActive { get; private set; }

    /// <summary>
    /// The core assembly for Spindle launcher.
    /// </summary>
    public static Assembly AssemblyCore { get; } = typeof(SpindleLauncher).Assembly;

    /// <summary>
    /// Contains all services for Spindle loader.
    /// </summary>
    public static IServiceProvider ServiceProvider => _serviceProvider;


#nullable disable

    /// <summary>
    /// Configuration stored in the 'Spindle Configuration' file.
    /// </summary>
    public static IConfiguration Configuration { get; private set; }

    /// <summary>
    /// Used to create loggers for different types.
    /// </summary>
    public static ILoggerFactory LoggerFactory { get; private set; }

    /// <summary>
    /// Handles converting values to display-friendly strings for translations and logging.
    /// </summary>
    public static IValueStringConvertService ValueStringConvertService { get; private set; }

    /// <summary>
    /// Handles loading plugins and core mods for Spindle launcher.
    /// </summary>
    public static SpindlePluginLoader PluginLoader { get; private set; }

    /// <summary>
    /// Manages storing <see cref="Language"/> objects.
    /// </summary>
    public static ILanguageService LanguageService { get; private set; }

    /// <summary>
    /// Manages all registered <see cref="TranslationCollection"/> types.
    /// </summary>
    public static ITranslationService TranslationService { get; private set; }
    
    /// <summary>
    /// Executes text commands for players and the console.
    /// </summary>
    public static ICommandDispatcher CommandDispatcher { get; private set; }

    /// <summary>
    /// File provider covering the <see cref="SpindlePaths.SpindleDirectory"/> and all sub-directories.
    /// </summary>
    public static PhysicalFileProvider SpindleDirectoryFileProvider { get; private set; }

#nullable restore

    private void BeginInitialization()
    {
        IsActive = true;

        try
        {
            GameThread.Setup();
            InitializeIntl();
        }
        catch (Exception ex)
        {
            CommandWindow.LogError(Properties.Resources.SpindleLaunchException);
            CommandWindow.LogError(ex);
        }
    }

    private void InitializeIntl()
    {
        _tokenSource?.Dispose();
        _tokenSource = new CancellationTokenSource();

        InitializeUniTask();

        InitializeConfiguration();

        SpindlePlayerComponent.Initialize();
        ThreadSafePlayerList.Initialize();

        _isStartupLoaded = false;

        // patch the startup asset loader to keep assets from loading until plugins have started
        MethodInfo? startupAssetsMethod = typeof(Assets).GetMethod("StartupAssetLoading", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        if (startupAssetsMethod != null)
        {
            Patcher.Patch(startupAssetsMethod, postfix: new HarmonyMethod(Accessor.GetMethod(PostfixStartupAssetLoading)));
        }
        else
        {
            CommandWindow.LogError($"Unable to find method {Accessor.Formatter.Format(new MethodDefinition("StartupAssetLoading").DeclaredIn<Assets>(isStatic: false).WithNoParameters().Returning<IEnumerator>())}.");
        }

        UniTask.Create(async () =>
        {
            try
            {
                await InitializeAsync(_tokenSource.Token);
            }
            catch (Exception ex)
            {
                CommandWindow.LogError(Properties.Resources.SpindleLaunchException);
                CommandWindow.LogError(ex);
            }
        });
    }

    private async UniTask InitializeAsync(CancellationToken token)
    {
        await UniTask.SwitchToMainThread(token);

        LoggerFactory = new SpindleLoggerFactory(new SpindleLoggerProvider(), _spindleConfiguration.GetSection("Logging"));
        _logger = LoggerFactory.CreateLogger<SpindleLauncher>();

        SpindleParentServiceProvider spindleParentProvider = new SpindleParentServiceProvider();
        ServiceContainer collection = new ServiceContainer(spindleParentProvider);
        _serviceProvider = collection;

        if (JsonLanguageService.IsAvailable(out _))
        {
            LanguageService = new JsonLanguageService(LoggerFactory.CreateLogger<JsonLanguageService>());
        }
        else
        {
            LanguageService = new YamlLanguageService(LoggerFactory.CreateLogger<YamlLanguageService>());
        }

        PluginLoader = new SpindlePluginLoader(LoggerFactory.CreateLogger<SpindlePluginLoader>());
        PluginLoader.DiscoverPluginsFromFiles();

        ValueStringConvertService = new ValueStringConvertService(LanguageService, LoggerFactory.CreateLogger<ValueStringConvertService>(), PluginLoader);

        await LanguageService.InitializeAsync();
        await UniTask.SwitchToMainThread();

        ConfigureServices(collection);

        PluginLoader.RunCoreMods(collection);
        
        // dispose default services if they were replaced by a core mod.

        LoggerFactory = CheckService(collection, LoggerFactory);
        if (collection.GetService(typeof(Microsoft.Extensions.Logging.ILoggerFactory)) is Microsoft.Extensions.Logging.ILoggerFactory loggerFactory && !ReferenceEquals(loggerFactory, LoggerFactory))
        {
            collection.AddService(typeof(Microsoft.Extensions.Logging.ILoggerFactory), LoggerFactory);
        }

        LanguageService = CheckService(collection, LanguageService);

        ValueStringConvertService = CheckService(collection, ValueStringConvertService);

        ITranslationDataStore translationDataStore
            = CheckService(collection, () => new PropertiesTranslationDataStore(PluginLoader, LanguageService, LoggerFactory.CreateLogger<PropertiesTranslationDataStore>()));
        
        TranslationService
            = CheckService(collection, () => new SpindleTranslationService(LanguageService, translationDataStore, ValueStringConvertService, LoggerFactory, PluginLoader));

        ICommandParser parser
            = CheckService(collection, () => new SpindleCommandParser());

        CommandDispatcher
            = CheckService(collection, () => new SpindleCommandDispatcher(parser, LoggerFactory.CreateLogger<SpindleCommandDispatcher>()));

        CheckUnchangeableService(collection, PluginLoader);
        CheckUnchangeableService(collection, SpindleDirectoryFileProvider);
        CheckUnchangeableService<IFileProvider>(collection, SpindleDirectoryFileProvider);
        
        await PluginLoader.RunPluginsAsync(ServiceProvider, token);
        await UniTask.SwitchToMainThread(token);

        _logger.LogInformation("Spindle started: v{0} for Unturned v{1}.", AssemblyCore.GetName().Version.ToString(3), Provider.APP_VERSION);
        _isStartupLoaded = true;
    }

    private static void CheckUnchangeableService<T>(ServiceContainer collection, T value)
    {
        if (collection.GetService(typeof(T)) is T registeredValue)
        {
            if (ReferenceEquals(registeredValue, value))
                return;

            Type type = registeredValue.GetType();
            _logger.LogWarning("Spindle service {0} is not able to be overridden but was overridden by {1} from {2}. The changes will not be applied.", typeof(T), type, type.Assembly.GetName().Name);
            if (registeredValue is IDisposable disp)
            {
                try
                {
                    disp.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing invalid registration for {0}: {1}.", typeof(T), type);
                }
            }
        }

        collection.AddService(typeof(T), value);
    }

    private static T CheckService<T>(ServiceContainer collection, T value)
    {
        if (collection.GetService(typeof(T)) is T registeredValue)
        {
            if (ReferenceEquals(registeredValue, value))
                return value;

            Type type = registeredValue.GetType();
            _logger.LogInformation("Spindle service {0} overridden by {1} from {2}.", typeof(T), type, type.Assembly.GetName().Name);
            if (value is IDisposable disp)
            {
                try
                {
                    disp.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing default registration for {0}: {1}.", typeof(T), value.GetType());
                }
            }

            return registeredValue;
        }

        collection.AddService(typeof(T), value);
        return value;
    }

    private static T CheckService<T>(ServiceContainer collection, Func<T> factory)
    {
        if (collection.GetService(typeof(T)) is T registeredValue)
        {
            Type type = registeredValue.GetType();
            _logger.LogInformation("Spindle service {0} overridden by {1} from {2}.", typeof(T), type, type.Assembly.GetName().Name);
            return registeredValue;
        }

        T value = factory();
        collection.AddService(typeof(T), value);
        return value;
    }

    private void ConfigureServices(IServiceContainer collection)
    {
        collection.AddService(typeof(SpindleLauncher), this);
        collection.AddService(typeof(IModuleNexus), this);

        collection.AddService(typeof(IFileProvider), SpindleDirectoryFileProvider);
        collection.AddService(typeof(PhysicalFileProvider), SpindleDirectoryFileProvider);

        collection.AddService(typeof(IConfiguration), Configuration);

        collection.AddService(typeof(SpindlePluginLoader), PluginLoader);

        collection.AddService(typeof(ILoggerFactory), LoggerFactory);
        collection.AddService(typeof(Microsoft.Extensions.Logging.ILoggerFactory), LoggerFactory);

        collection.AddService(typeof(ILogger), (_, _) => LoggerFactory.CreateLogger(string.Empty));
        collection.AddService(typeof(Microsoft.Extensions.Logging.ILogger), (_, _) => LoggerFactory.CreateLogger(string.Empty));
    }

    private static void PostfixStartupAssetLoading(ref IEnumerator __result)
    {
        __result = PatchedAssetCoroutine(__result);
    }

    private static IEnumerator PatchedAssetCoroutine(IEnumerator oldCoroutine)
    {
        while (!_isStartupLoaded)
            yield return null;

        yield return oldCoroutine;
    }

    private static void InitializeConfiguration()
    {
        Directory.CreateDirectory(SpindlePaths.SpindleDirectory);
        Directory.CreateDirectory(SpindlePaths.LocalizationDirectory);
        Directory.CreateDirectory(SpindlePaths.LibrariesDirectory);
        Directory.CreateDirectory(SpindlePaths.PluginsDirectory);

        SpindleDirectoryFileProvider = new PhysicalFileProvider(SpindlePaths.SpindleDirectory);

        ConfigurationBuilder bldr = new ConfigurationBuilder();

        string configFileYml = Path.Combine(SpindlePaths.SpindleDirectory, "Spindle Configuration.yml");
        string configFileJson = Path.Combine(SpindlePaths.SpindleDirectory, "Spindle Configuration.json");

        if (File.Exists(configFileJson))
        {
            if (File.Exists(configFileYml))
                CommandWindow.LogWarning("Two Spindle Configuration files exist, using JSON file.");

            bldr.AddJsonFile(SpindleDirectoryFileProvider, Path.GetRelativePath(SpindleDirectoryFileProvider.Root, configFileJson), optional: false, reloadOnChange: true);
        }
        else if (File.Exists(configFileYml))
        {
            bldr.AddYamlFile(SpindleDirectoryFileProvider, Path.GetRelativePath(SpindleDirectoryFileProvider.Root, configFileYml), optional: false, reloadOnChange: true);
        }
        else
        {
            using (Stream resxStream = GetDefaultConfigStream())
            {
                using (FileStream writeToYml = new FileStream(configFileYml, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    resxStream.CopyTo(writeToYml);
                    writeToYml.Flush();
                }
            }

            bldr.AddYamlFile(SpindleDirectoryFileProvider, Path.GetRelativePath(SpindleDirectoryFileProvider.Root, configFileYml), optional: false, reloadOnChange: true);
        }

        // add defaults that may have came from an update or something
        using (Stream resxStream = GetDefaultConfigStream())
        {
            bldr.AddYamlStream(resxStream);
        }

        _spindleConfiguration = bldr.Build();
        Configuration = _spindleConfiguration;

        return;

        static Stream GetDefaultConfigStream()
        {
            return AssemblyCore.GetManifestResourceStream("Spindle.Defaults.Spindle Configuration.yml")
                   ?? throw new InvalidProgramException("Missing Spindle Configuration.yml defaults.");
        }
    }

    /// <summary>
    /// Starts a graceful shutdown.
    /// </summary>
    /// <remarks>This task will never finish.</remarks>
    [ContractAnnotation("=> halt")]
    public async UniTask ShutdownAsync(string? explanation, CancellationToken token = default)
    {
        for (int i = Provider.clients.Count - 1; i >= 0; i--)
        {
            SteamPlayer pl = Provider.clients[i];
            Provider.kick(pl.playerID.steamID, string.IsNullOrWhiteSpace(explanation) ? "Server shutting down: " + explanation : "Server shutting down");
        }

        // todo lock players joining

        await PluginLoader.ShutdownAsync(token: token);

        SDG.Framework.Modules.Module? module = ModuleHook.modules.Find(x => x.status == EModuleStatus.Initialized && Array.IndexOf(x.assemblies, AssemblyCore) >= 0);
        if (module == null)
        {
            _logger.LogWarning("Spindle module not found.");
        }
        else
        {
            module.isEnabled = false;
        }

        Provider.shutdown(0, explanation ?? string.Empty);
        await UniTask.WaitWhile(() => true, cancellationToken: CancellationToken.None);
    }

    private void ShutdownForcefully()
    {
        try
        {
            PluginLoader.HandleNonGracefulShutdownRequest();

            _serviceProvider.Dispose();
            
            if (_spindleConfiguration is IDisposable disposableConfiguration)
                disposableConfiguration.Dispose();

            SpindleDirectoryFileProvider.Dispose();
        }
        catch (Exception ex)
        {
            CommandWindow.LogError(Properties.Resources.SpindleShutdownException);
            CommandWindow.LogError(ex);
        }

        if (_tokenSource != null)
        {
            _tokenSource.Dispose();
            _tokenSource = null;
        }

        IsActive = false;
    }

    private static void InitializeUniTask()
    {
        // initialize UniTask if OpenMod hasn't already
        if (PlayerLoopHelper.IsInjectedUniTaskPlayerLoop())
        {
            CommandWindow.Log(Properties.Resources.UniTaskInjectionHandledByOtherModule);
            return;
        }

        MethodInfo? initMethod = typeof(PlayerLoopHelper).GetMethod("Init",
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, CallingConventions.Any, Type.EmptyTypes, null
        );

        if (initMethod != null)
        {
            initMethod.Invoke(null, Array.Empty<object>());
            CommandWindow.Log(Properties.Resources.UniTaskInjectionSuccess);
        }
        else
            CommandWindow.LogError(Properties.Resources.UniTaskInjectionReflectionFailure);

        UniTaskScheduler.UnobservedTaskException += UnitaskExceptionUnobserved;
        UniTaskScheduler.DispatchUnityMainThread = false;
        UniTaskScheduler.PropagateOperationCanceledException = true;
    }

    private static void UnitaskExceptionUnobserved(Exception ex)
    {
        if (LoggerFactory == null)
        {
            if (GameThread.IsCurrent)
            {
                CommandWindow.LogError(Properties.Resources.UniTaskUnobservedException);
                CommandWindow.LogError(ex);
            }
            else
            {
                Exception ex2 = ex;
                UniTask.Create(async () =>
                {
                    await UniTask.SwitchToMainThread();
                    CommandWindow.LogError(Properties.Resources.UniTaskUnobservedException);
                    CommandWindow.LogError(ex2);
                });
            }
        }
        else
        {
            LoggerFactory.CreateLogger<UniTask>().LogError(ex, Properties.Resources.UniTaskUnobservedException);
        }
    }

    private class SpindleParentServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, Func<Type, Type, object?>> _services = new Dictionary<Type, Func<Type, Type, object?>>
        {
            { typeof(ILogger<>), (_, innerType) => LoggerFactory.CreateLogger(innerType) },
            { typeof(Microsoft.Extensions.Logging.ILogger<>), (_, innerType) => LoggerFactory.CreateLogger(innerType) }
        };

        public object? GetService(Type serviceType)
        {
            if (!serviceType.IsConstructedGenericType)
            {
                if (serviceType.IsSubclassOf(typeof(TranslationCollection)))
                {
                    return TranslationService.Get(serviceType);
                }

                return null;
            }

            Type def = serviceType.GetGenericTypeDefinition();
            return _services.TryGetValue(def, out Func<Type, Type, object?> action)
                ? action(def, def.GetGenericArguments()[0])
                : null;
        }
    }

    /// <summary>
    /// Actual Unturned injection point, stored as a separate class to make TypeLoadExceptions not get hidden by the game.
    /// </summary>
    [UsedImplicitly]
    private class Nexus : IModuleNexus
    {
        private object? _launcher;

        private static object Initialize()
        {
            SpindleLauncher nexus = new SpindleLauncher();
            nexus.BeginInitialization();
            return nexus;
        }

        private static void Shutdown(ref object? launcher)
        {
            if (launcher == null)
                return;

            SpindleLauncher nexus = (SpindleLauncher)launcher;
            nexus.ShutdownForcefully();
        }

        void IModuleNexus.initialize()
        {
            try
            {
                _launcher = Initialize();
            }
            catch (Exception ex)
            {
                CommandWindow.LogError(Properties.Resources.SpindleLaunchException);
                CommandWindow.LogError(ex);
                
                Assembly thisAsm = Assembly.GetExecutingAssembly();

                SDG.Framework.Modules.Module? module = ModuleHook.modules.Find(x => Array.IndexOf(x.assemblies, thisAsm) >= 0);
                if (module != null)
                {
                    module.isEnabled = false;
                }
            }
        }

        void IModuleNexus.shutdown()
        {
            try
            {
                Shutdown(ref _launcher);
            }
            catch (Exception ex)
            {
                CommandWindow.LogError(Properties.Resources.SpindleShutdownException);
                CommandWindow.LogError(ex);
            }
        }
    }
}