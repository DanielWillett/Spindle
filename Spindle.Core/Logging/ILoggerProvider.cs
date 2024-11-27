namespace Spindle.Logging;

/// <summary>
/// Logger provider for Spindle launcher.
/// </summary>
public interface ILoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider
{
    /// <summary>
    /// Creates a new <see cref="ILogger" /> instance.
    /// </summary>
    /// <param name="categoryName">The category name for messages produced by the logger.</param>
    /// <returns>The instance of <see cref="ILogger" /> that was created.</returns>
    new ILogger CreateLogger(string categoryName);
}