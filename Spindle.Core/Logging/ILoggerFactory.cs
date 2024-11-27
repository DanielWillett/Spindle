using DanielWillett.ReflectionTools;
using System;

namespace Spindle.Logging;

/// <summary>
/// Implementation of the Microsoft Extension logger factory made for Spindle logging.
/// </summary>
public interface ILoggerFactory : Microsoft.Extensions.Logging.ILoggerFactory
{
    /// <summary>
    /// Creates a new <see cref="ILogger" /> instance.
    /// </summary>
    /// <param name="categoryName">The category name for messages produced by the logger.</param>
    /// <returns>The <see cref="ILogger" />.</returns>
    new ILogger CreateLogger(string categoryName);

    /// <summary>
    /// Creates a new <see cref="ILogger" /> instance for a given type.
    /// </summary>
    /// <typeparam name="T">The category name for messages produced by the logger.</typeparam>
    /// <returns>The <see cref="ILogger" />.</returns>
    ILogger<T> CreateLogger<T>();

    /// <summary>
    /// Re-creates all loggers.
    /// </summary>
    void Reset();

    /// <summary>
    /// Sets the <see cref="ILoggerProvider" /> for the logging system.
    /// </summary>
    /// <param name="provider">The <see cref="ILoggerProvider" />.</param>
    void SetProvider(ILoggerProvider provider);

    /// <summary>
    /// Creates a new <see cref="ILogger" /> instance for a given type.
    /// </summary>
    /// <param name="category">The category name for messages produced by the logger.</param>
    /// <returns>The <see cref="ILogger" />.</returns>
    ILogger CreateLogger(Type category)
    {
        return CreateLogger(Accessor.ExceptionFormatter.Format(category));
    }
}