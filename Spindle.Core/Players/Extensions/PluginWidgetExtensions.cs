using System.Runtime.InteropServices;

namespace Spindle.Players;

public partial struct SpindlePlayer
{
    /// <summary>
    /// Provides access to the plugin widgets for this player.
    /// </summary>
    public readonly ref readonly PlayerWidgets Widgets
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            unsafe
            {
#pragma warning disable CS9084 // Struct member returns 'this' or other instance members by reference
                return ref Unsafe.As<SpindlePlayer, PlayerWidgets>(ref Unsafe.AsRef(in this));
#pragma warning restore CS9084
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public readonly struct PlayerWidgets
    {
        [FieldOffset(0)]
        internal readonly SteamPlayer Player;

        public EPluginWidgetFlags Active => Player.player.pluginWidgetFlags;

        /// <summary>
        /// Enable or disable a widget flag.
        /// </summary>
        public bool this[EPluginWidgetFlags flag]
        {
            get => (Player.player.pluginWidgetFlags & flag) != 0;
            set => Player.player.setPluginWidgetFlag(flag, value);
        }

        /// <inheritdoc cref="EPluginWidgetFlags.Modal"/>
        public bool Modal
        {
            get => (Player.player.pluginWidgetFlags & EPluginWidgetFlags.Modal) != 0;
            set => Player.player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, value);
        }

        /// <inheritdoc cref="EPluginWidgetFlags.NoBlur"/>
        public bool NoBlur
        {
            get => (Player.player.pluginWidgetFlags & EPluginWidgetFlags.NoBlur) != 0;
            set => Player.player.setPluginWidgetFlag(EPluginWidgetFlags.NoBlur, value);
        }

        /// <inheritdoc cref="EPluginWidgetFlags.ForceBlur"/>
        public bool ForceBlur
        {
            get => (Player.player.pluginWidgetFlags & EPluginWidgetFlags.ForceBlur) != 0;
            set => Player.player.setPluginWidgetFlag(EPluginWidgetFlags.ForceBlur, value);
        }

        /// <inheritdoc cref="EPluginWidgetFlags.ShowInteractWithEnemy"/>
        public bool ShowInteractWithEnemy
        {
            get => (Player.player.pluginWidgetFlags & EPluginWidgetFlags.ShowInteractWithEnemy) != 0;
            set => Player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowInteractWithEnemy, value);
        }

        /// <inheritdoc cref="EPluginWidgetFlags.ShowDeathMenu"/>
        public bool ShowDeathMenu
        {
            get => (Player.player.pluginWidgetFlags & EPluginWidgetFlags.ShowDeathMenu) != 0;
            set => Player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowDeathMenu, value);
        }

        /// <inheritdoc cref="EPluginWidgetFlags.ShowHealth"/>
        public bool ShowHealth
        {
            get => (Player.player.pluginWidgetFlags & EPluginWidgetFlags.ShowHealth) != 0;
            set => Player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowHealth, value);
        }

        /// <inheritdoc cref="EPluginWidgetFlags.ShowFood"/>
        public bool ShowFood
        {
            get => (Player.player.pluginWidgetFlags & EPluginWidgetFlags.ShowFood) != 0;
            set => Player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowFood, value);
        }

        /// <inheritdoc cref="EPluginWidgetFlags.ShowWater"/>
        public bool ShowWater
        {
            get => (Player.player.pluginWidgetFlags & EPluginWidgetFlags.ShowWater) != 0;
            set => Player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowWater, value);
        }

        /// <inheritdoc cref="EPluginWidgetFlags.ShowVirus"/>
        public bool ShowVirus
        {
            get => (Player.player.pluginWidgetFlags & EPluginWidgetFlags.ShowVirus) != 0;
            set => Player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVirus, value);
        }

        /// <inheritdoc cref="EPluginWidgetFlags.ShowStamina"/>
        public bool ShowStamina
        {
            get => (Player.player.pluginWidgetFlags & EPluginWidgetFlags.ShowStamina) != 0;
            set => Player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStamina, value);
        }

        /// <inheritdoc cref="EPluginWidgetFlags.ShowOxygen"/>
        public bool ShowOxygen
        {
            get => (Player.player.pluginWidgetFlags & EPluginWidgetFlags.ShowOxygen) != 0;
            set => Player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowOxygen, value);
        }

        /// <inheritdoc cref="EPluginWidgetFlags.ShowStatusIcons"/>
        public bool ShowStatusIcons
        {
            get => (Player.player.pluginWidgetFlags & EPluginWidgetFlags.ShowStatusIcons) != 0;
            set => Player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, value);
        }

        /// <inheritdoc cref="EPluginWidgetFlags.ShowUseableGunStatus"/>
        public bool ShowUseableGunStatus
        {
            get => (Player.player.pluginWidgetFlags & EPluginWidgetFlags.ShowUseableGunStatus) != 0;
            set => Player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowUseableGunStatus, value);
        }

        /// <inheritdoc cref="EPluginWidgetFlags.ShowVehicleStatus"/>
        public bool ShowVehicleStatus
        {
            get => (Player.player.pluginWidgetFlags & EPluginWidgetFlags.ShowVehicleStatus) != 0;
            set => Player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVehicleStatus, value);
        }

        /// <inheritdoc cref="EPluginWidgetFlags.ShowCenterDot"/>
        public bool ShowCenterDot
        {
            get => (Player.player.pluginWidgetFlags & EPluginWidgetFlags.ShowCenterDot) != 0;
            set => Player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowCenterDot, value);
        }

        /// <inheritdoc cref="EPluginWidgetFlags.ShowReputationChangeNotification"/>
        public bool ShowReputationChangeNotification
        {
            get => (Player.player.pluginWidgetFlags & EPluginWidgetFlags.ShowReputationChangeNotification) != 0;
            set => Player.player.setPluginWidgetFlag(EPluginWidgetFlags.ShowReputationChangeNotification, value);
        }
    }
}