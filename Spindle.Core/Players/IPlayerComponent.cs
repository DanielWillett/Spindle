namespace Spindle.Players;

/// <summary>
/// Object that can optionally be a Unity component that is added to players on joined and remove on disconnect.
/// </summary>
/// <remarks>Use <see cref="SpindlePlayer"/> to access components.</remarks>
public interface IPlayerComponent
{
    SpindlePlayer Player { get; }
    void Initialize(SpindlePlayer player);
}
