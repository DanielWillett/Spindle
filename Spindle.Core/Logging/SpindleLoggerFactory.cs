using DanielWillett.ReflectionTools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Spindle.Util;
using System;
using System.Collections.Generic;

namespace Spindle.Logging;

using ILoggerFactoryMs = Microsoft.Extensions.Logging.ILoggerFactory;
using ILoggerMs = Microsoft.Extensions.Logging.ILogger;
using ILoggerProviderMs = Microsoft.Extensions.Logging.ILoggerProvider;

/// <summary>
/// Overrides the default logger factory but only supports one logger provider for simplicity.
/// </summary>
public class SpindleLoggerFactory : ILoggerFactory
{
    private readonly Dictionary<string, LoggerWrapper> _loggers = new Dictionary<string, LoggerWrapper>(StringComparer.Ordinal);
    
    private ILoggerProviderMs _loggerProvider;
    private LogLevel _minimumLevel;

    private List<LogFilter> _loggingRules = null!; 

    public SpindleLoggerFactory(ILoggerProviderMs loggerProvider, IConfiguration loggerSettings)
    {
        _loggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));

        ReadLoggingSettings(loggerSettings);

        IChangeToken changeToken = loggerSettings.GetReloadToken();
        changeToken.RegisterChangeCallback(ReadLoggingSettings, loggerSettings);
    }

    public void Reset()
    {
        lock (_loggers)
        {
            foreach (KeyValuePair<string, LoggerWrapper> logger in _loggers)
            {
                ILogger oldLogger = Interlocked.Exchange(ref logger.Value.ActualLogger, CreateLoggerIntl(logger.Key));
                if (oldLogger is IDisposable loggerDisp)
                    loggerDisp.Dispose();
            }
        }
    }

    /// <summary>
    /// Add a category filter at run-time.
    /// </summary>
    public void AddLogFilter(string categoryName, LogLevel minimumLevel)
    {
        lock (_loggers)
        {
            _loggingRules = [.. _loggingRules, new LogFilter { Category = categoryName, MinimumLevel = minimumLevel }];
            foreach (KeyValuePair<string, LoggerWrapper> logger in _loggers)
            {
                UpdateLoggerMinimumLevel(logger.Key, logger.Value);
            }
        }
    }

    public void SetProvider(ILoggerProvider provider)
    {
        ((ILoggerFactoryMs)this).AddProvider(provider);
    }

    private ILogger CreateLoggerIntl(string categoryName)
    {
        ILoggerMs logger = _loggerProvider.CreateLogger(categoryName);
        if (logger is not ILogger spindleLogger)
        {
            if (logger is IDisposable disp)
                disp.Dispose();

            throw new InvalidOperationException("Logger providers must return loggers that implement Spindle.Logging.ILogger.");
        }

        return spindleLogger;
    }

    public ILogger CreateLogger(string categoryName)
    {
        LoggerWrapper logger;
        lock (_loggers)
        {
            if (!_loggers.TryGetValue(categoryName, out logger))
            {
                _loggers.Add(categoryName, logger = new LoggerWrapper { ActualLogger = CreateLoggerIntl(categoryName) });
                UpdateLoggerMinimumLevel(categoryName, logger);
            }
        }

        return logger;
    }
    
    public ILogger<T> CreateLogger<T>()
    {
        string categoryName = Accessor.Formatter.Format<T>();
        LoggerWrapper<T> typedWrapper;
        lock (_loggers)
        {
            if (!_loggers.TryGetValue(categoryName, out LoggerWrapper logger))
            {
                _loggers.Add(categoryName, typedWrapper = new LoggerWrapper<T> { ActualLogger = CreateLoggerIntl(categoryName) });
                UpdateLoggerMinimumLevel(categoryName, typedWrapper);
            }
            else if (logger is not LoggerWrapper<T> typed)
            {
                _loggers[categoryName] = typedWrapper = new LoggerWrapper<T>
                {
                    ActualLogger = logger.ActualLogger,
                    MinimumLevel =  logger.MinimumLevel
                };

                logger.ActualLogger = typedWrapper;
                logger.MinimumLevel = 0;
            }
            else
            {
                return typed;
            }
        }

        return typedWrapper;
    }

    void ILoggerFactoryMs.AddProvider(ILoggerProviderMs loggerProvider)
    {
        if (loggerProvider == null)
            throw new ArgumentNullException(nameof(loggerProvider));

        ILoggerProviderMs oldLoggerProvider;
        lock (_loggers)
        {
            oldLoggerProvider = _loggerProvider;
            _loggerProvider = loggerProvider;

            foreach (KeyValuePair<string, LoggerWrapper> logger in _loggers)
            {
                ILogger oldLogger = Interlocked.Exchange(ref logger.Value.ActualLogger, CreateLoggerIntl(logger.Key));
                if (oldLogger is IDisposable loggerDisp)
                    loggerDisp.Dispose();
            }
        }

        if (oldLoggerProvider is IDisposable disp)
        {
            disp.Dispose();
        }
    }

    private void ReadLoggingSettings(object? obj)
    {
        IConfiguration loggingSection = (IConfiguration?)obj!;

        string? logLvlStr = loggingSection["MinimumLevel"];

        LogLevel minimumLevel = Enum.TryParse(logLvlStr, ignoreCase: true, out LogLevel logLevel)
            ? logLevel
            : LogLevel.Trace;

        List<LogFilter> filters = new List<LogFilter>(4);

        foreach (IConfigurationSection value in loggingSection.GetSection("Rules").GetChildren())
        {
            string category = value.Key;
            logLvlStr = value.Value;
            if (string.IsNullOrEmpty(category) || !Enum.TryParse(logLvlStr, ignoreCase: true, out logLevel))
                continue;

            filters.Add(new LogFilter
            {
                Category = category,
                MinimumLevel = logLevel
            });
        }

        lock (_loggers)
        {
            _minimumLevel = minimumLevel;
            _loggingRules = filters;
            foreach (KeyValuePair<string, LoggerWrapper> logger in _loggers)
            {
                UpdateLoggerMinimumLevel(logger.Key, logger.Value);
            }
        }
    }

    private void UpdateLoggerMinimumLevel(string categoryName, LoggerWrapper logger)
    {
        if (_loggingRules.Count == 0)
        {
            logger.MinimumLevel = _minimumLevel;
            return;
        }

        ReadOnlySpan<char> categorySpan = categoryName;

        LogFilter best = default;
        bool found = false;
        int i = 0;
        for (; i < _loggingRules.Count; ++i)
        {
            LogFilter filter = _loggingRules[i];
            if (!FilterApplies(filter, categoryName))
                continue;

            best = filter;
            ++i;
            found = true;
            break;
        }

        if (!found)
        {
            logger.MinimumLevel = _minimumLevel;
            return;
        }

        for (; i < _loggingRules.Count; ++i)
        {
            LogFilter filter = _loggingRules[i];
            if (AggregateFilter(filter, best, categorySpan))
                best = filter;
        }

        logger.MinimumLevel = best.MinimumLevel;
    }

    private static bool FilterApplies(LogFilter next, ReadOnlySpan<char> category)
    {
        // implementation from https://github.com/dotnet/extensions/blob/v3.1.0/src/Logging/Logging/src/LoggerRuleSelector.cs#L44

        const char wildcard = '*';

        if (category.Count(wildcard) != 1)
        {
            return category.Equals(next.Category, StringComparison.OrdinalIgnoreCase);
        }

        ReadOnlySpan<char> toAnalyze = next.Category;

        int index = toAnalyze.IndexOf(wildcard);
        ReadOnlySpan<char> prefix = index == 0 ? ReadOnlySpan<char>.Empty : toAnalyze[..index];
        ReadOnlySpan<char> suffix = index == toAnalyze.Length - 1 ? ReadOnlySpan<char>.Empty : toAnalyze[(index + 1)..];

        return (prefix.Length == 0 || category.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
               && (suffix.Length == 0 || category.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
    }

    private static bool AggregateFilter(LogFilter next, LogFilter best, ReadOnlySpan<char> category)
    {
        return FilterApplies(next, category) && best.Category.Length <= next.Category.Length;
    }

    public void Dispose()
    {
        ILoggerProviderMs oldLoggerProvider;
        lock (_loggers)
        {
            oldLoggerProvider = _loggerProvider;
            _loggerProvider = NullLoggerProvider.Instance;

            foreach (LoggerWrapper logger in _loggers.Values)
            {
                ILogger oldLogger = Interlocked.Exchange(ref logger.ActualLogger, NullLogger.Instance);
                if (oldLogger is IDisposable loggerDisp)
                    loggerDisp.Dispose();
            }
        }

        if (oldLoggerProvider is IDisposable disp)
            disp.Dispose();
    }

    ILoggerMs ILoggerFactoryMs.CreateLogger(string categoryName) => CreateLogger(categoryName);

    private sealed class NullLogger : ILogger
    {
        public static readonly NullLogger Instance = new NullLogger();
        private NullLogger() { }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
        public bool IsEnabled(LogLevel logLevel) => false;
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;
    }

    private sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();
        private NullScope() { }
        public void Dispose() { }
    }

    private sealed class LoggerWrapper<T> : LoggerWrapper, ILogger<T>;
    private class LoggerWrapper : ILogger
    {
#nullable disable
        public ILogger ActualLogger;
        public LogLevel MinimumLevel;
#nullable restore

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            ActualLogger.Log(logLevel, eventId, state, exception, formatter);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= MinimumLevel && ActualLogger.IsEnabled(logLevel);
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return ActualLogger.BeginScope(state);
        }
    }

    private struct LogFilter
    {
        public string Category;
        public LogLevel MinimumLevel;
    }
}
