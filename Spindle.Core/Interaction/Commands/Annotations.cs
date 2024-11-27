using System;

namespace Spindle.Interaction.Commands;

/// <summary>
/// Metadata for a <see cref="ISpindleCommand"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[BaseTypeRequired(typeof(ISpindleCommand))]
public sealed class CommandAttribute : Attribute
{
    /// <summary>
    /// String used to invoke the command without the slash, for example <c>home</c> would be the value for a <c>/home</c> command.
    /// </summary>
    /// <remarks>This value is usually lowercase but isn't case-sensitive.</remarks>
    public string CommandName { get; }
    
    /// <summary>
    /// File path from which the command was compiled.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// List of alternatives for <see cref="CommandName"/> used to invoke the command without the slash, for example <c>tp</c> would be the value for a <c>/teleport</c> command.
    /// </summary>
    /// <remarks>These values are usually lowercase but aren't case-sensitive.</remarks>
    public string[]? Aliases { get; set; }

    /// <summary>
    /// Permission needed to execute the command. Defaults to <c>plugin::commands[.subcommands...].[name]</c>.
    /// </summary>
    public string? PermissionOverride { get; set; }

    public CommandAttribute(string commandName, [CallerFilePath] string filePath = "")
    {
        CommandName = commandName;
        FilePath = filePath;
    }
}