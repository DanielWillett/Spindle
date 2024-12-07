using Spindle.Threading;
using Spindle.Unturned;
using System;

namespace Spindle.Players;

public partial struct SpindlePlayer
{
    /// <summary>
    /// If this player has been placed in handcuffs. They can be released using <see cref="EndArrest"/>.
    /// </summary>
    public readonly bool IsPlayerArrested => SteamPlayer.player.animator.captorStrength > 0;

    /// <summary>
    /// The Steam64 ID of the player that originally arrested this player, or <see cref="CSteamID.Nil"/> if they're not arrested.
    /// </summary>
    public readonly CSteamID ArrestingPlayerId => SteamPlayer.player.animator.captorID;

    /// <summary>
    /// The item currently being used to arrest the player, or <see langword="null"/> if they're not arrested.
    /// </summary>
    public readonly ItemArrestStartAsset? ArrestingItem => UnturnedAssets.Get<ItemArrestStartAsset>(SteamPlayer.player.animator.captorItem);

    /// <summary>
    /// Place this player under arrest using the given item. If the player is currently arrested they will be released first.
    /// </summary>
    /// <param name="instigator">The Steam64 ID of the player that should be credited for arresting this player. Can be <see cref="CSteamID.Nil"/> if no player should be credited. Only <paramref name="instigator"/> can release the player.</param>
    /// <param name="item">The item to use as handcuffs. This affects which items can be used to release the player.</param>
    /// <param name="strength">The number of times the player has to lean to break free. If this is less than zero, the <paramref name="item"/>'s default strength will be used. This value is clamped to the range of <see cref="ushort"/> if it's too large.</param>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    /// <exception cref="ArgumentException">Invalid Steam64 ID.</exception>
    public readonly void StartArrest(CSteamID instigator, ItemArrestStartAsset item, int strength = -1)
    {
        GameThread.AssertCurrent();

        AssertOnline();

        if (instigator.GetEAccountType() != EAccountType.k_EAccountTypeIndividual && instigator.m_SteamID != 0)
            throw new ArgumentException(Properties.Resources.ExceptionSteamIdNotIndividual, nameof(instigator));

        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (strength > ushort.MaxValue)
            strength = ushort.MaxValue;
        else if (strength < 0)
            strength = item.strength;

        PlayerAnimator animator = SteamPlayer.player.animator;

        if (animator.captorStrength > 0)
        {
            EndArrestIntl(animator);
        }

        animator.captorID = instigator;
        animator.captorItem = item.id;
        animator.captorStrength = (ushort)strength;
        animator.sendGesture(EPlayerGesture.ARREST_START, true);
    }

    /// <summary>
    /// Free this player from their arrest.
    /// </summary>
    /// <remarks>An exception will be thrown if the player is not arrested, so check <see cref="IsPlayerArrested"/> first.</remarks>
    /// <exception cref="GameThreadException"/>
    /// <exception cref="PlayerOfflineException"/>
    /// <exception cref="InvalidOperationException">Player is not arrested.</exception>
    public readonly void EndArrest()
    {
        GameThread.AssertCurrent();

        AssertOnline();

        PlayerAnimator animator = SteamPlayer.player.animator;
        if (animator.captorStrength == 0)
        {
            throw new InvalidOperationException(Properties.Resources.EndArrestPlayerNotArrested);
        }

        EndArrestIntl(animator);
        animator.sendGesture(EPlayerGesture.ARREST_STOP, true);
    }

    private static void EndArrestIntl(PlayerAnimator animator)
    {
        animator.captorStrength = 0;
        animator.captorID = CSteamID.Nil;
        animator.captorItem = 0;

        EffectAsset? asset = VanillaAssetConstants.Metal_1_Ref.Find();
        if (asset != null)
        {
            EffectManager.triggerEffect(new TriggerEffectParameters(asset)
            {
                relevantDistance = EffectManager.MEDIUM,
                position = animator.transform.position
            });
        }
    }
}