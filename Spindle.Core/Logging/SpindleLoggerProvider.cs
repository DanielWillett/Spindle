using DanielWillett.ReflectionTools;
using Microsoft.Extensions.Logging;
using SDG.Framework.Utilities;
using Spindle.Threading;
using Spindle.Util;
using StackCleaner;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Spindle.Logging;

/// <summary>
/// Creates <see cref="SpindleLogger"/> instances for a <see cref="ILoggerFactory"/>. Recommended to use this through <see cref="SpindleLoggerFactory"/> instead of directly.
/// </summary>
public class SpindleLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentQueue<LogMessage> _messages;

    internal StackTraceCleaner StackCleaner;

    private static readonly StaticGetter<LogFile> GetDebugLog = Accessor.GenerateStaticGetter<Logs, LogFile>("debugLog", throwOnError: true)!;

    private static readonly Action<string> CallLogInfoIntl =
        (Action<string>)typeof(CommandWindow)
            .GetMethod("internalLogInformation", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
            .CreateDelegate(typeof(Action<string>), Dedicator.commandWindow);

    private static readonly Action<string> CallLogWarningIntl =
        (Action<string>)typeof(CommandWindow)
            .GetMethod("internalLogWarning", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
            .CreateDelegate(typeof(Action<string>), Dedicator.commandWindow);

    private static readonly Action<string> CallLogErrorIntl =
        (Action<string>)typeof(CommandWindow)
            .GetMethod("internalLogError", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
            .CreateDelegate(typeof(Action<string>), Dedicator.commandWindow);

    public SpindleLoggerProvider()
    {
        _messages = new ConcurrentQueue<LogMessage>();

        StackCleanerConfiguration config = new StackCleanerConfiguration
        {
            ColorFormatting = StackColorFormatType.ExtendedANSIColor,
            Colors = UnityColor32Config.Default,
            IncludeNamespaces = false,
            IncludeILOffset = true,
            IncludeLineData = true,
            IncludeFileData = true,
            IncludeAssemblyData = false,
            IncludeSourceData = true,
            Locale = CultureInfo.InvariantCulture,
            PutSourceDataOnNewLine = true,
        };

        // add UniTask types to hidden types
        List<Type> hiddenTypes = [ ..config.GetHiddenTypes(), typeof(UniTask) ];

        Assembly uniTaskAsm = typeof(UniTask).Assembly;
        Type? type = uniTaskAsm.GetType("Cysharp.Threading.Tasks.EnumeratorAsyncExtensions+EnumeratorPromise", false, false);
        if (type != null)
            hiddenTypes.Add(type);

        type = uniTaskAsm.GetType("Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask`1", false, false);
        if (type != null)
            hiddenTypes.Add(type);

        type = uniTaskAsm.GetType("Cysharp.Threading.Tasks.CompilerServices.AsyncUniTask`2", false, false);
        if (type != null)
            hiddenTypes.Add(type);

        foreach (Type baseType in typeof(UniTask)
                     .GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                     .Where(x => x.Name.IndexOf("promise", StringComparison.OrdinalIgnoreCase) != -1))
        {
            hiddenTypes.Add(baseType);
        }

        config.HiddenTypes = hiddenTypes;

        StackCleaner = new StackTraceCleaner(config);
    }

    /// <summary>
    /// Log something to the console and file without any extra formatting.
    /// </summary>
    /// <remarks>Use <see cref="TerminalColorHelper.ColorSetting"/> to ensure any color formatting uses the correct format type.</remarks>
    /// <param name="text">Text printed to console.</param>
    /// <param name="unformattedLog">Text written to the server log file. If this is <see langword="null"/> than nothing will be written to the log file.</param>
    /// <exception cref="GameThreadException"/>
    public static void WriteToLogRaw(LogLevel logLevel, string text, string? unformattedLog)
    {
        GameThread.AssertCurrent();

        LogIntl(logLevel, text, unformattedLog);
    }

    private static void LogIntl(LogLevel logLevel, string text, string? unformattedLog)
    {
        if (!SpindleLauncher.IsActive)
        {
            Console.WriteLine(text);
            System.Diagnostics.Debug.WriteLine(text);
            return;
        }

        switch (logLevel)
        {
            default:
                CallLogInfoIntl.Invoke(text);
                break;

            case LogLevel.Warning:
                CallLogWarningIntl.Invoke(text);
                break;

            case LogLevel.Critical:
            case LogLevel.Error:
                CallLogErrorIntl.Invoke(text);
                break;
        }

        if (unformattedLog == null)
            return;

        try
        {
            GetDebugLog().writeLine(unformattedLog);
        }
        catch (Exception ex)
        {
            CallLogErrorIntl("Failed to write error: " + ex);
        }
    }

    internal void QueueOutput(LogLevel logLevel, string text, string? unformattedLog)
    {
        if (GameThread.IsCurrent)
        {
            Update();
            LogIntl(logLevel, text, unformattedLog);
            return;
        }

        _messages.Enqueue(new LogMessage
        {
            Level = logLevel,
            Text = text,
            Unformatted = unformattedLog
        });
    }

    private void Update()
    {
        while (_messages.TryDequeue(out LogMessage message))
        {
            LogIntl(message.Level, message.Text, message.Unformatted);
        }
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new SpindleLogger(categoryName, this, SpindleLauncher.ValueStringConvertService);
    }

    Microsoft.Extensions.Logging.ILogger Microsoft.Extensions.Logging.ILoggerProvider.CreateLogger(string categoryName)
    {
        return CreateLogger(categoryName);
    }

    void IDisposable.Dispose()
    {
        TimeUtility.updated -= Update;
        if (GameThread.IsCurrent)
            Update();
        else
        {
            UniTask.Create(async () =>
            {
                await UniTask.SwitchToMainThread();
                Update();
            });
        }
    }

    private struct LogMessage
    {
        public string Text;
        public LogLevel Level;
        public string? Unformatted;
    }
}