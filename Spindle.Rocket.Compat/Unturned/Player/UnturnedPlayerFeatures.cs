using System;

namespace Rocket.Unturned.Player;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public sealed class UnturnedPlayerFeatures : UnturnedPlayerComponent
{
    private bool _vanishMode;
 
    public DateTime Joined = DateTime.Now;

    public bool GodMode { get; set; }
    public bool VanishMode
    {
        get => _vanishMode;
        set
        {
            Player.GetComponent<UnturnedPlayerMovement>().VanishMode = value;

            PlayerMovement movement = Player.Player.movement;
            movement.canAddSimulationResultsToUpdates = !value;
            
            if (_vanishMode && !value)
            {
                PlayerLook look = movement.player.look;
                movement.updates.Add(new PlayerStateUpdate(movement.transform.position, look.angle, look.rot));
            }

            _vanishMode = value;
        }
    }

    protected override void Load() { }
}