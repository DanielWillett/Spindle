using Microsoft.Extensions.Logging;
using Spindle.Logging;
using System;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace Spindle;

// this class is mostly copied and expanded from
// https://github.com/dotnet/extensions/blob/v3.1.0/src/Logging/Logging.Abstractions/src/LoggerExtensions.cs
//  - Microsoft.Extensions.Logging.Abstractions v3.1.0 source

/// <summary>
/// Used to log information to the console in Spindle launcher. Fully supports scopes (so long as the logger is only accessed from one thread at a time).
/// </summary>
/// <remarks>Implementation of <see cref="Microsoft.Extensions.Logging.ILogger"/> with the same name so people use this interface and avoid the extension methods in <see cref="LoggerExtensions"/>.</remarks>
public interface ILogger<out T> : Microsoft.Extensions.Logging.ILogger<T>, ILogger;

/// <summary>
/// Used to log information to the console in Spindle launcher. Fully supports scopes (so long as the logger is only accessed from one thread at a time).
/// </summary>
/// <remarks>Implementation of <see cref="Microsoft.Extensions.Logging.ILogger"/> with the same name so people use this interface and avoid the extension methods in <see cref="LoggerExtensions"/>.</remarks>
public interface ILogger : Microsoft.Extensions.Logging.ILogger
{
    //------------------------------------------DEBUG------------------------------------------//


    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogDebug(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(EventId eventId, Exception? exception, string message, params object?[]? args)
    {
        Log(LogLevel.Debug, eventId, exception, message, args);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogDebug(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(EventId eventId, string message, params object?[]? args)
    {
        Log(LogLevel.Debug, eventId, message, args);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogDebug(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(Exception? exception, string message, params object?[]? args)
    {
        Log(LogLevel.Debug, exception, message, args);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogDebug("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(string message, params object?[]? args)
    {
        Log(LogLevel.Debug, message, args);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(EventId eventId, Exception? exception, string message)
    {
        Log(LogLevel.Debug, eventId, exception, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(EventId eventId, string message)
    {
        Log(LogLevel.Debug, eventId, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(Exception? exception, string message)
    {
        Log(LogLevel.Debug, exception, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(string message)
    {
        Log(LogLevel.Debug, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(EventId eventId, Exception? exception, string message, object? arg1)
    {
        Log(LogLevel.Debug, eventId, exception, message, arg1);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(EventId eventId, string message, object? arg1)
    {
        Log(LogLevel.Debug, eventId, message, arg1);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(Exception? exception, string message, object? arg1)
    {
        Log(LogLevel.Debug, exception, message, arg1);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(string message, object? arg1)
    {
        Log(LogLevel.Debug, message, arg1);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(EventId eventId, Exception? exception, string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Debug, eventId, exception, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(EventId eventId, string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Debug, eventId, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(Exception? exception, string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Debug, exception, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Debug, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(EventId eventId, Exception? exception, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Debug, eventId, exception, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(EventId eventId, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Debug, eventId, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(Exception? exception, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Debug, exception, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Debug, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(EventId eventId, Exception? exception, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Debug, eventId, exception, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(EventId eventId, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Debug, eventId, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(Exception? exception, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Debug, exception, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogDebug("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogDebug(string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Debug, message, arg1, arg2, arg3, arg4);
    }

    //------------------------------------------TRACE------------------------------------------//

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogTrace(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(EventId eventId, Exception? exception, string message, params object?[]? args)
    {
        Log(LogLevel.Trace, eventId, exception, message, args);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogTrace(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(EventId eventId, string message, params object?[]? args)
    {
        Log(LogLevel.Trace, eventId, message, args);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogTrace(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(Exception? exception, string message, params object?[]? args)
    {
        Log(LogLevel.Trace, exception, message, args);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogTrace("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(string message, params object?[]? args)
    {
        Log(LogLevel.Trace, message, args);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(EventId eventId, Exception? exception, string message)
    {
        Log(LogLevel.Trace, eventId, exception, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(EventId eventId, string message)
    {
        Log(LogLevel.Trace, eventId, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(Exception? exception, string message)
    {
        Log(LogLevel.Trace, exception, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(string message)
    {
        Log(LogLevel.Trace, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(EventId eventId, Exception? exception, string message, object? arg1)
    {
        Log(LogLevel.Trace, eventId, exception, message, arg1);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(EventId eventId, string message, object? arg1)
    {
        Log(LogLevel.Trace, eventId, message, arg1);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(Exception? exception, string message, object? arg1)
    {
        Log(LogLevel.Trace, exception, message, arg1);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(string message, object? arg1)
    {
        Log(LogLevel.Trace, message, arg1);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(EventId eventId, Exception? exception, string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Trace, eventId, exception, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(EventId eventId, string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Trace, eventId, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(Exception? exception, string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Trace, exception, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Trace, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(EventId eventId, Exception? exception, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Trace, eventId, exception, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(EventId eventId, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Trace, eventId, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(Exception? exception, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Trace, exception, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Trace, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(EventId eventId, Exception? exception, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Trace, eventId, exception, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(EventId eventId, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Trace, eventId, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(Exception? exception, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Trace, exception, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a trace log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogTrace("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogTrace(string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Trace, message, arg1, arg2, arg3, arg4);
    }

    //------------------------------------------INFORMATION------------------------------------------//

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogInformation(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(EventId eventId, Exception? exception, string message, params object?[]? args)
    {
        Log(LogLevel.Information, eventId, exception, message, args);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogInformation(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(EventId eventId, string message, params object?[]? args)
    {
        Log(LogLevel.Information, eventId, message, args);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogInformation(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(Exception? exception, string message, params object?[]? args)
    {
        Log(LogLevel.Information, exception, message, args);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogInformation("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(string message, params object?[]? args)
    {
        Log(LogLevel.Information, message, args);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(EventId eventId, Exception? exception, string message)
    {
        Log(LogLevel.Information, eventId, exception, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(EventId eventId, string message)
    {
        Log(LogLevel.Information, eventId, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(Exception? exception, string message)
    {
        Log(LogLevel.Information, exception, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(string message)
    {
        Log(LogLevel.Information, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(EventId eventId, Exception? exception, string message, object? arg1)
    {
        Log(LogLevel.Information, eventId, exception, message, arg1);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(EventId eventId, string message, object? arg1)
    {
        Log(LogLevel.Information, eventId, message, arg1);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(Exception? exception, string message, object? arg1)
    {
        Log(LogLevel.Information, exception, message, arg1);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(string message, object? arg1)
    {
        Log(LogLevel.Information, message, arg1);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(EventId eventId, Exception? exception, string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Information, eventId, exception, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(EventId eventId, string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Information, eventId, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(Exception? exception, string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Information, exception, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Information, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(EventId eventId, Exception? exception, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Information, eventId, exception, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(EventId eventId, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Information, eventId, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(Exception? exception, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Information, exception, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Information, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(EventId eventId, Exception? exception, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Information, eventId, exception, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(EventId eventId, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Information, eventId, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(Exception? exception, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Information, exception, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes an informational log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogInformation("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogInformation(string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Information, message, arg1, arg2, arg3, arg4);
    }

    //------------------------------------------WARNING------------------------------------------//

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogWarning(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(EventId eventId, Exception? exception, string message, params object?[]? args)
    {
        Log(LogLevel.Warning, eventId, exception, message, args);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogWarning(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(EventId eventId, string message, params object?[]? args)
    {
        Log(LogLevel.Warning, eventId, message, args);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogWarning(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(Exception? exception, string message, params object?[]? args)
    {
        Log(LogLevel.Warning, exception, message, args);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogWarning("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(string message, params object?[]? args)
    {
        Log(LogLevel.Warning, message, args);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(EventId eventId, Exception? exception, string message)
    {
        Log(LogLevel.Warning, eventId, exception, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(EventId eventId, string message)
    {
        Log(LogLevel.Warning, eventId, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(Exception? exception, string message)
    {
        Log(LogLevel.Warning, exception, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(string message)
    {
        Log(LogLevel.Warning, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(EventId eventId, Exception? exception, string message, object? arg1)
    {
        Log(LogLevel.Warning, eventId, exception, message, arg1);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(EventId eventId, string message, object? arg1)
    {
        Log(LogLevel.Warning, eventId, message, arg1);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(Exception? exception, string message, object? arg1)
    {
        Log(LogLevel.Warning, exception, message, arg1);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(string message, object? arg1)
    {
        Log(LogLevel.Warning, message, arg1);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(EventId eventId, Exception? exception, string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Warning, eventId, exception, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(EventId eventId, string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Warning, eventId, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(Exception? exception, string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Warning, exception, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Warning, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(EventId eventId, Exception? exception, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Warning, eventId, exception, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(EventId eventId, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Warning, eventId, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(Exception? exception, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Warning, exception, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Warning, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(EventId eventId, Exception? exception, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Warning, eventId, exception, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(EventId eventId, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Warning, eventId, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(Exception? exception, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Warning, exception, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a warning log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogWarning("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogWarning(string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Warning, message, arg1, arg2, arg3, arg4);
    }

    //------------------------------------------ERROR------------------------------------------//

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogError(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(EventId eventId, Exception? exception, string message, params object?[]? args)
    {
        Log(LogLevel.Error, eventId, exception, message, args);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogError(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(EventId eventId, string message, params object?[]? args)
    {
        Log(LogLevel.Error, eventId, message, args);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogError(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(Exception? exception, string message, params object?[]? args)
    {
        Log(LogLevel.Error, exception, message, args);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogError("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(string message, params object?[]? args)
    {
        Log(LogLevel.Error, message, args);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(EventId eventId, Exception? exception, string message)
    {
        Log(LogLevel.Error, eventId, exception, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(EventId eventId, string message)
    {
        Log(LogLevel.Error, eventId, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(Exception? exception, string message)
    {
        Log(LogLevel.Error, exception, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(string message)
    {
        Log(LogLevel.Error, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(EventId eventId, Exception? exception, string message, object? arg1)
    {
        Log(LogLevel.Error, eventId, exception, message, arg1);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(EventId eventId, string message, object? arg1)
    {
        Log(LogLevel.Error, eventId, message, arg1);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(Exception? exception, string message, object? arg1)
    {
        Log(LogLevel.Error, exception, message, arg1);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(string message, object? arg1)
    {
        Log(LogLevel.Error, message, arg1);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(EventId eventId, Exception? exception, string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Error, eventId, exception, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(EventId eventId, string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Error, eventId, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(Exception? exception, string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Error, exception, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Error, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(EventId eventId, Exception? exception, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Error, eventId, exception, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(EventId eventId, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Error, eventId, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(Exception? exception, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Error, exception, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Error, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(EventId eventId, Exception? exception, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Error, eventId, exception, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(EventId eventId, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Error, eventId, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(Exception? exception, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Error, exception, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes an error log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogError("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogError(string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Error, message, arg1, arg2, arg3, arg4);
    }

    //------------------------------------------CRITICAL------------------------------------------//

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogCritical(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(EventId eventId, Exception? exception, string message, params object?[]? args)
    {
        Log(LogLevel.Critical, eventId, exception, message, args);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogCritical(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(EventId eventId, string message, params object?[]? args)
    {
        Log(LogLevel.Critical, eventId, message, args);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogCritical(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(Exception? exception, string message, params object?[]? args)
    {
        Log(LogLevel.Critical, exception, message, args);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogCritical("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(string message, params object?[]? args)
    {
        Log(LogLevel.Critical, message, args);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(EventId eventId, Exception? exception, string message)
    {
        Log(LogLevel.Critical, eventId, exception, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(EventId eventId, string message)
    {
        Log(LogLevel.Critical, eventId, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(Exception? exception, string message)
    {
        Log(LogLevel.Critical, exception, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(string message)
    {
        Log(LogLevel.Critical, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(EventId eventId, Exception? exception, string message, object? arg1)
    {
        Log(LogLevel.Critical, eventId, exception, message, arg1);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(EventId eventId, string message, object? arg1)
    {
        Log(LogLevel.Critical, eventId, message, arg1);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(Exception? exception, string message, object? arg1)
    {
        Log(LogLevel.Critical, exception, message, arg1);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(string message, object? arg1)
    {
        Log(LogLevel.Critical, message, arg1);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(EventId eventId, Exception? exception, string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Critical, eventId, exception, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(EventId eventId, string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Critical, eventId, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(Exception? exception, string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Critical, exception, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(string message, object? arg1, object? arg2)
    {
        Log(LogLevel.Critical, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(EventId eventId, Exception? exception, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Critical, eventId, exception, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(EventId eventId, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Critical, eventId, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(Exception? exception, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Critical, exception, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(string message, object? arg1, object? arg2, object? arg3)
    {
        Log(LogLevel.Critical, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(EventId eventId, Exception? exception, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Critical, eventId, exception, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(EventId eventId, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Critical, eventId, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(Exception? exception, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Critical, exception, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a critical log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogCritical("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    void LogCritical(string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(LogLevel.Critical, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, string message, params object?[]? args)
    {
        Log(logLevel, 0, null, message, args);
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, EventId eventId, string message, params object?[]? args)
    {
        Log(logLevel, eventId, null, message, args);
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, Exception? exception, string message, params object?[]? args)
    {
        Log(logLevel, 0, exception, message, args);
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, EventId eventId, Exception? exception, string message, params object?[]? args)
    {
        args ??= Array.Empty<object?>();
        if (SpindleLauncher.IsActive)
        {
            Log(logLevel, eventId, new SpindleFormattedLogValues(message, args), exception, MessageFormatter);
        }
        else
        {
            LoggerExtensions.Log(this, logLevel, eventId, exception, message, args);
        }
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, string message)
    {
        Log(logLevel, 0, null, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, EventId eventId, string message)
    {
        Log(logLevel, eventId, null, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, Exception? exception, string message)
    {
        Log(logLevel, 0, exception, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, EventId eventId, Exception? exception, string message)
    {
        if (SpindleLauncher.IsActive)
        {
            Log(logLevel, eventId, new SpindleFormattedLogValues(message, Array.Empty<object>()), exception, MessageFormatter);
        }
        else
        {
            LoggerExtensions.Log(this, logLevel, eventId, exception, message, Array.Empty<object>());
        }
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, string message, object? arg1)
    {
        Log(logLevel, 0, null, message, arg1);
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, EventId eventId, string message, object? arg1)
    {
        Log(logLevel, eventId, null, message, arg1);
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, Exception? exception, string message, object? arg1)
    {
        Log(logLevel, 0, exception, message, arg1);
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, EventId eventId, Exception? exception, string message, object? arg1)
    {
        if (SpindleLauncher.IsActive)
        {
            Log(logLevel, eventId, new SpindleFormattedLogValues(message, arg1), exception, MessageFormatter);
        }
        else
        {
            LoggerExtensions.Log(this, logLevel, eventId, exception, message, [arg1]);
        }
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, string message, object? arg1, object? arg2)
    {
        Log(logLevel, 0, null, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, EventId eventId, string message, object? arg1, object? arg2)
    {
        Log(logLevel, eventId, null, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, Exception? exception, string message, object? arg1, object? arg2)
    {
        Log(logLevel, 0, exception, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, EventId eventId, Exception? exception, string message, object? arg1, object? arg2)
    {
        if (SpindleLauncher.IsActive)
        {
            Log(logLevel, eventId, new SpindleFormattedLogValues(message, arg1, arg2), exception, MessageFormatter);
        }
        else
        {
            LoggerExtensions.Log(this, logLevel, eventId, exception, message, [arg1, arg2]);
        }
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(logLevel, 0, null, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, EventId eventId, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(logLevel, eventId, null, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, Exception? exception, string message, object? arg1, object? arg2, object? arg3)
    {
        Log(logLevel, 0, exception, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, EventId eventId, Exception? exception, string message, object? arg1, object? arg2, object? arg3)
    {
        if (SpindleLauncher.IsActive)
        {
            Log(logLevel, eventId, new SpindleFormattedLogValues(message, arg1, arg2, arg3), exception, MessageFormatter);
        }
        else
        {
            LoggerExtensions.Log(this, logLevel, eventId, exception, message, [ arg1, arg2, arg3 ]);
        }
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(logLevel, 0, null, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, EventId eventId, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(logLevel, eventId, null, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, Exception? exception, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        Log(logLevel, 0, exception, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a log message at the specified log level.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="logLevel">Entry will be written on this level.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message.</param>
    [StringFormatMethod(nameof(message))]
    void Log(LogLevel logLevel, EventId eventId, Exception? exception, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        if (SpindleLauncher.IsActive)
        {
            Log(logLevel, eventId, new SpindleFormattedLogValues(message, arg1, arg2, arg3, arg4), exception, MessageFormatter);
        }
        else
        {
            LoggerExtensions.Log(this, logLevel, eventId, exception, message, [arg1, arg2, arg3, arg4]);
        }
    }

    private static readonly Func<SpindleFormattedLogValues, Exception?, string> MessageFormatter = MessageFormatterMtd;
    private static string MessageFormatterMtd(SpindleFormattedLogValues state, Exception? error)
    {
        return state.ToString();
    }
}

public static class SpindleLoggerExtensions
{
    //---------------------------------------CONDITIONAL---------------------------------------//

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogDebug(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, EventId eventId, Exception? exception, string message, params object?[]? args)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, eventId, exception, message, args);
        else
            logger.Log(LogLevel.Debug, eventId, exception, message, args ?? Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogConditional(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, EventId eventId, string message, params object?[]? args)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, eventId, message, args);
        else
            logger.Log(LogLevel.Debug, eventId, message, args ?? Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogConditional(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, Exception? exception, string message, params object?[]? args)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, exception, message, args);
        else
            logger.Log(LogLevel.Debug, exception, message, args ?? Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <example>logger.LogConditional("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, string message, params object?[]? args)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, message, args);
        else
            logger.Log(LogLevel.Debug, message, args ?? Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, EventId eventId, Exception? exception, string message)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, eventId, exception, message, Array.Empty<object>());
        else
            logger.Log(LogLevel.Debug, eventId, exception, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, EventId eventId, string message)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, eventId, message, Array.Empty<object>());
        else
            logger.Log(LogLevel.Debug, eventId, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, Exception? exception, string message)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, exception, message, Array.Empty<object>());
        else
            logger.Log(LogLevel.Debug, exception, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, string message)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, message, Array.Empty<object>());
        else
            logger.Log(LogLevel.Debug, message, Array.Empty<object>());
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, EventId eventId, Exception? exception, string message, object? arg1)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, eventId, exception, message, arg1);
        else
            logger.Log(LogLevel.Debug, eventId, exception, message, arg1);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, EventId eventId, string message, object? arg1)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, eventId, message, arg1);
        else
            logger.Log(LogLevel.Debug, eventId, message, arg1);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, Exception? exception, string message, object? arg1)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, exception, message, arg1);
        else
            logger.Log(LogLevel.Debug, exception, message, arg1);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, string message, object? arg1)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, message, arg1);
        else
            logger.Log(LogLevel.Debug, message, arg1);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, EventId eventId, Exception? exception, string message, object? arg1, object? arg2)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, eventId, exception, message, arg1, arg2);
        else
            logger.Log(LogLevel.Debug, eventId, exception, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, EventId eventId, string message, object? arg1, object? arg2)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, eventId, message, arg1, arg2);
        else
            logger.Log(LogLevel.Debug, eventId, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, Exception? exception, string message, object? arg1, object? arg2)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, message, arg1, arg2);
        else
            logger.Log(LogLevel.Debug, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, string message, object? arg1, object? arg2)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, message, arg1, arg2);
        else
            logger.Log(LogLevel.Debug, message, arg1, arg2);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, EventId eventId, Exception? exception, string message, object? arg1, object? arg2, object? arg3)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, eventId, exception, message, arg1, arg2, arg3);
        else
            logger.Log(LogLevel.Debug, eventId, exception, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, EventId eventId, string message, object? arg1, object? arg2, object? arg3)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, eventId, message, arg1, arg2, arg3);
        else
            logger.Log(LogLevel.Debug, eventId, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, Exception? exception, string message, object? arg1, object? arg2, object? arg3)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, exception, message, arg1, arg2, arg3);
        else
            logger.Log(LogLevel.Debug, exception, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, string message, object? arg1, object? arg2, object? arg3)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, message, arg1, arg2, arg3);
        else
            logger.Log(LogLevel.Debug, message, arg1, arg2, arg3);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional(0, exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, EventId eventId, Exception? exception, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, eventId, exception, message, arg1, arg2, arg3, arg4);
        else
            logger.Log(LogLevel.Debug, eventId, exception, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="eventId">The event id associated with the log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional(0, "Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, EventId eventId, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, eventId, message, arg1, arg2, arg3, arg4);
        else
            logger.Log(LogLevel.Debug, eventId, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional(exception, "Error while processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, Exception? exception, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, exception, message, arg1, arg2, arg3, arg4);
        else
            logger.Log(LogLevel.Debug, exception, message, arg1, arg2, arg3, arg4);
    }

    /// <summary>
    /// Formats and writes a debug log message.
    /// </summary>
    /// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> to write to.</param>
    /// <param name="message">Format string of the log message in message template format. Example: <code>"User {0} logged in from {1}"</code></param>
    /// <example>logger.LogConditional("Processing request from {Address}", address)</example>
    [StringFormatMethod(nameof(message))]
    [Conditional("DEBUG")]
    public static void LogConditional(this Microsoft.Extensions.Logging.ILogger logger, string message, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        if (logger is ILogger s)
            s.Log(LogLevel.Debug, message, arg1, arg2, arg3, arg4);
        else
            logger.Log(LogLevel.Debug, message, arg1, arg2, arg3, arg4);
    }
}