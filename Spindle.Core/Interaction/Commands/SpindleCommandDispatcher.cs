using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Spindle.Interaction.Commands;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public class SpindleCommandDispatcher : ICommandDispatcher//, ISubCommandContainer
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

    public ICommandRegistration? FindParentCommand(string commandName, bool caseSensitive = true)
    {
        StringComparison comparison = caseSensitive
            ? StringComparison.InvariantCultureIgnoreCase
            : StringComparison.InvariantCulture;

        ICommandRegistration? highestPriorityAliasMatch = null;
        int highestPriorityAliasMatchPriority = 0;
        foreach (ICommandRegistration cmd in _parentCommands)
        {
            if (commandName.Equals(cmd.Name, comparison))
            {
                if (highestPriorityAliasMatchPriority > cmd.Priority && highestPriorityAliasMatch != null)
                    return highestPriorityAliasMatch;
                return cmd;
            }

            if (highestPriorityAliasMatch != null && highestPriorityAliasMatchPriority > cmd.Priority)
                continue;

            bool found = false;
            if (cmd.Aliases is string[] arr)
            {
                foreach (string str in arr)
                {
                    if (!commandName.Equals(str, comparison))
                        continue;

                    found = true;
                    break;
                }
            }
            else
            {
                foreach (string str in cmd.Aliases)
                {
                    if (!commandName.Equals(str, comparison))
                        continue;

                    found = true;
                    break;
                }
            }

            if (!found)
                continue;

            highestPriorityAliasMatchPriority = cmd.Priority;
            highestPriorityAliasMatch = cmd;
        }

        return highestPriorityAliasMatch;
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

        foundCommand = null;//FindCommand(preParsedCommand.CommandName);

        return UniTask.FromResult(false);// TryExecuteCommandAsync()
    }

    private async UniTask<bool> TryExecuteCommandIntl(ICommandRegistration command, IInteractionUser user, ParsedCommandInfo info, string original)
    {
        return false;
    }

    public UniTask InitializeAsync(CancellationToken token) => throw new NotImplementedException();

    public UniTask ShutdownAsync(CancellationToken token) => throw new NotImplementedException();
}
