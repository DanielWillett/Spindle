using System;

namespace Spindle.Threading;

/// <summary>
/// Utility for quickly checking if the current thread is the game thread.
/// </summary>
public static class GameThread
{
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
}

/// <summary>
/// Thrown when something can't be ran off the main thread.
/// </summary>
public class GameThreadException : NotSupportedException
{
    public GameThreadException() : base(Properties.Resources.ExceptionGameThread) { }
    public GameThreadException(string feature) : base(string.Format(Properties.Resources.ExceptionGameThreadContext, feature)) { }
}