using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Spindle.Interaction.Commands;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public class SpindleCommandDispatcher : ICommandDispatcher
{
    private readonly ICommandParser _commandParser;
    private readonly ILogger<SpindleCommandDispatcher> _logger;

    private List<ICommandRegistration> _parentCommands;

    public IReadOnlyList<ICommandRegistration> Commands { get; private set; }

    public SpindleCommandDispatcher(ICommandParser commandParser, ILogger<SpindleCommandDispatcher> logger)
    {
        _commandParser = commandParser;
        _logger = logger;

        Commands = Array.Empty<ICommandRegistration>();
        _parentCommands = new List<ICommandRegistration>();
    }

    public ICommandRegistration? FindParentCommand(string commandName)
    {
        for (int i = 0; i < _parentCommands.Count; ++i)
        {
            ICommandRegistration cmd = _parentCommands[i];

            if (commandName.Equals(cmd.Name, StringComparison.InvariantCultureIgnoreCase))
                return cmd;

            for (int j = 0; j < _parentCommands.Count; ++j)
            {
                
            }
        }
    }

    public UniTask<bool> TryExecuteCommandAsync(IInteractionUser user, string command, bool requirePrefix, [MaybeNullWhen(false)] out ICommandRegistration foundCommand, CancellationToken token = default)
    {
        ParsedCommandInfo info = _commandParser.ParseCommandInput(command, requirePrefix);
        return TryExecuteCommandAsync(user, info, out foundCommand, token);
    }

    public UniTask<bool> TryExecuteCommandAsync(IInteractionUser user, ParsedCommandInfo preParsedCommand, [MaybeNullWhen(false)] out ICommandRegistration foundCommand, CancellationToken token = default)
    {
        if (string.IsNullOrEmpty(preParsedCommand.CommandName))
        {
            foundCommand = null;
            return UniTask.FromResult(false);
        }

        foundCommand = FindCommand(preParsedCommand.CommandName);

        return TryExecuteCommandAsync()
    }

    private async UniTask<bool> TryExecuteCommandIntl(ICommandRegistration command, IInteractionUser user, ParsedCommandInfo info, string original)
    {

    }

    public UniTask InitializeAsync(CancellationToken token) => throw new NotImplementedException();

    public UniTask ShutdownAsync(CancellationToken token) => throw new NotImplementedException();
}
