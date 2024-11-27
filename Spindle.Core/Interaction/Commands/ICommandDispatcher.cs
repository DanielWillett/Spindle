using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Spindle.Interaction.Commands;

public interface ICommandDispatcher
{
    /// <summary>
    /// List of all registered parent commands. Does not contain sub-commands.
    /// </summary>
    public IReadOnlyList<ICommandRegistration> Commands { get; }

    /// <summary>
    /// Find a parent command by name or alias.
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    /// <remarks>Does not look for sub-commands.</remarks>
    ICommandRegistration? FindParentCommand(string commandName);

    /// <summary>
    /// Execute a command as a <paramref name="user"/> from a string of commands and arguments.
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    /// <param name="requirePrefix">If a prefix is required to recognize the string as a command. This is usually <see langword="true"/> for players and <see langword="false"/> for the console.</param>
    UniTask<bool> TryExecuteCommandAsync(IInteractionUser user, string command, bool requirePrefix, [MaybeNullWhen(false)] out ICommandRegistration foundCommand, CancellationToken token = default);

    /// <summary>
    /// Execute a command as a <paramref name="user"/> from an already parsed set of a command name, arguments, and flags.
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    UniTask<bool> TryExecuteCommandAsync(IInteractionUser user, ParsedCommandInfo preParsedCommand, [MaybeNullWhen(false)] out ICommandRegistration foundCommand, CancellationToken token = default);
    
    UniTask InitializeAsync(CancellationToken token);

    UniTask ShutdownAsync(CancellationToken token);
}