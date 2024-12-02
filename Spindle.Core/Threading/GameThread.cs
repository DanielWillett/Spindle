using System;
using System.Diagnostics;

namespace Spindle.Threading;

/// <summary>
/// Utility for quickly checking if the current thread is the game thread.
/// </summary>
public static class GameThread
{
    private static ILogger? _logger;


    [ThreadStatic]
    // ReSharper disable once FieldCanBeMadeReadOnly.Local (this is broken in mono for some reason)
    private static bool _isCurrent = true;

    /// <summary>
    /// <see langword="true"/> when fetched on the main thread, otherwise <see langword="false"/>.
    /// </summary>
    /// <remarks>Much more effecient than <see cref="ThreadUtil.IsGameThread"/>.</remarks>
    public static bool IsCurrent
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isCurrent;
    }


    /// <summary>
    /// Throw an error if this function isn't ran on the main thread.
    /// </summary>
    /// <exception cref="GameThreadException">Not on main thread.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AssertCurrent()
    {
        if (_isCurrent)
            return;

        throw new GameThreadException();
    }

    /// <summary>
    /// Throw an error if this function isn't ran on the main thread.
    /// </summary>
    /// <exception cref="GameThreadException">Not on main thread.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AssertCurrent(string feature)
    {
        if (_isCurrent)
            return;

        throw new GameThreadException(feature);
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    internal static void Setup()
    {
        ThreadUtil.assertIsGameThread();
    }

    /// <summary>
    /// Queue an action to run on the game thread and exit immediately.
    /// </summary>
    /// <remarks>If this method is called on the main thread, it will just be invoked before exiting. All exceptions will be caught and logged to console.</remarks>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Dispatch(Action action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        if (_isCurrent)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _logger ??= SpindleLauncher.LoggerFactory.CreateLogger(typeof(GameThread));

                _logger.LogError(ex, Properties.Resources.GameThreadDispatchExceptionCurrent);
                _logger.LogError("{0}", new StackTrace(1, true));
            }
        }
        else
        {
            Action a2 = action;
            LazyStackTrace stackTrace = new LazyStackTrace(1, true);
            PlayerLoopHelper.AddContinuation(PlayerLoopTiming.Update, () =>
            {
                try
                {
                    a2();
                }
                catch (Exception ex)
                {
                    StackTrace st = stackTrace.ToStackTrace();
                    _logger ??= SpindleLauncher.LoggerFactory.CreateLogger(typeof(GameThread));

                    _logger.LogError(ex, Properties.Resources.GameThreadDispatchExceptionNotCurrent);
                    _logger.LogError("{0}", st);
                }
            });
        }
    }
}

/// <summary>
/// Thrown when something can't be ran off the main thread.
/// </summary>
public class GameThreadException : NotSupportedException
{
    public GameThreadException() : base(Properties.Resources.ExceptionGameThread) { }
    public GameThreadException(string feature) : base(string.Format(Properties.Resources.ExceptionGameThreadContext, feature)) { }
}