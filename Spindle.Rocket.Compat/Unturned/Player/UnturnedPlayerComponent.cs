namespace Rocket.Unturned.Player;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class UnturnedPlayerComponent : MonoBehaviour
{
    public UnturnedPlayer Player { get; private set; }

#pragma warning disable IDE0051
    // ReSharper disable UnusedMember.Local

    private void Awake()
    {
        Player = UnturnedPlayer.FromPlayer(gameObject.GetComponent<SDG.Unturned.Player>());
    }

    private void OnEnable() => Load();

    private void OnDisable() => Unload();

    // ReSharper restore UnusedMember.Local
#pragma warning restore IDE0051

    protected virtual void Load() { }
    protected virtual void Unload() { }
}