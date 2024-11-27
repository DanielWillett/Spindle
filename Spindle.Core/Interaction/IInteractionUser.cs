using System;
using System.Collections.Generic;
using System.Text;
using Spindle.Localization;

namespace Spindle.Interaction;

/// <summary>
/// Generic endpoint for a user of a command, UI interaction, receiver of a chat message, etc.
/// </summary>
public interface IInteractionUser
{
    /// <summary>
    /// The Steam64 of the player this object represents, or <see cref="CSteamID.Nil"/> for a non-player such as an RCON client or the console.
    /// </summary>
    CSteamID Steam64 { get; }

    
}