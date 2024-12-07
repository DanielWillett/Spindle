namespace Spindle.Players;

public partial struct SpindlePlayer
{
    /// <summary>
    /// The player's current model gesture.
    /// </summary>
    public readonly EPlayerGesture Gesture => SteamPlayer.player.animator.gesture;


}