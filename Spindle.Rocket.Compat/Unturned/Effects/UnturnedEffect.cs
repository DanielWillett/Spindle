using Rocket.Unturned.Player;

namespace Rocket.Unturned.Effects;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public class UnturnedEffect
{
    private EffectAsset _asset;

    public string Type;
    public ushort EffectID;
    public bool Global;

    public UnturnedEffect(string type, ushort effectID, bool global)
    {
        Type = type;
        EffectID = effectID;
        Global = global;
    }

    public void Trigger(UnturnedPlayer player)
    {
        _asset ??= Assets.find(EAssetType.EFFECT, EffectID) as EffectAsset;

        if (_asset == null)
            return;

        TriggerEffectParameters parameters = new TriggerEffectParameters(_asset)
        {
            position = player.Player.transform.position
        };

        if (!Global)
        {
            parameters.SetRelevantPlayer(player.Player.channel.owner);
        }
        else
        {
            parameters.relevantDistance = 1024;
        }

        EffectManager.triggerEffect(parameters);
    }

    public void Trigger(Vector3 position)
    {
        _asset ??= Assets.find(EAssetType.EFFECT, EffectID) as EffectAsset;

        if (_asset == null)
            return;

        TriggerEffectParameters parameters = new TriggerEffectParameters(_asset)
        {
            position = position,
            relevantDistance = 1024
        };

        EffectManager.triggerEffect(parameters);
    }
}