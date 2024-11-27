namespace Spindle.Interaction.Commands;

/// <summary>
/// A command that can be invoked from chat or console.
/// </summary>
/// <remarks>Commands with an action should use <see cref="IExecutableSpindleCommand"/>. Other classes just act as empty sub-commands that either redirect to a child or redirect to /help.</remarks>
public interface ISpindleCommand;

/// <summary>
/// Command that can be executed directly.
/// </summary>
public interface IExecutableSpindleCommand : ISpindleCommand
{
    /// <summary>
    /// The context used to execute the command.
    /// </summary>
    CommandContext Context { get; set; }

    /// <summary>
    /// Entrypoint for execution of the command.
    /// </summary>
    /// <exception cref="CommandContext"/>
    UniTask ExecuteAsync(CancellationToken token);
}