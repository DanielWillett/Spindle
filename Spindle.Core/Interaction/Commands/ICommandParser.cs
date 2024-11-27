using System;

namespace Spindle.Interaction.Commands;

/// <summary>
/// Handles converting a string to command arguments.
/// </summary>
public interface ICommandParser
{
    /// <summary>
    /// Parse a string of text into arguments. Quotation marks should keep arguments separated by spaces together and arguments starting in '-' or '--' should become flags.
    /// </summary>
    /// <param name="requirePrefix">If a prefix is required to recognize the string as a command. This is usually <see langword="true"/> for players and <see langword="false"/> for the console.</param>
    ParsedCommandInfo ParseCommandInput(ReadOnlySpan<char> originalMessage, bool requirePrefix);
}

/// <summary>
/// Output data for <see cref="ICommandParser"/> implementations.
/// </summary>
public readonly struct ParsedCommandInfo(string? commandName, string[] arguments, CommandFlagInfo[] flags)
{
    /// <summary>
    /// The first 'word' in the command string.
    /// </summary>
    public readonly string? CommandName = commandName;

    /// <summary>
    /// List of all arguments entered by the user.
    /// </summary>
    public readonly string[] Arguments = arguments;

    /// <summary>
    /// List of all flags entered by the user.
    /// </summary>
    public readonly CommandFlagInfo[] Flags = flags;

    /// <summary>
    /// Format a round-trip string that could be used later to accurately re-parse the command.
    /// </summary>
    public override string ToString()
    {
        if (string.IsNullOrEmpty(CommandName))
            return "{Command not found}";

        int argCt = Arguments.Length;
        int ct = argCt + Flags.Length;
        int len = 1 + CommandName.Length + ct;

        if (ContainsWhiteSpaceOrQuotes(CommandName))
            len += 2;

        for (int i = 0; i < ct; ++i)
        {
            string str;
            if (i >= argCt)
            {
                ref CommandFlagInfo info = ref Flags[i - argCt];
                str = info.FlagName;
                len += info.DashCount;
            }
            else
            {
                str = Arguments[i];
            }

            len += str.Length;

            // quotes
            if (ContainsWhiteSpaceOrQuotes(str) || IsFlag(str))
                len += 2;
        }

        return string.Create(len, this, (span, state) =>
        {
            char quote1, quote2 = '\0';
            int writeIndex = 1;
            span[0] = '/';
            bool ws = ContainsWhiteSpaceOrQuotes(state.CommandName!);
            if (ws)
            {
                ChooseQuotes(state.CommandName!, out quote1, out quote2);
                span[writeIndex] = quote1;
                ++writeIndex;
            }
            state.CommandName.AsSpan().CopyTo(span[writeIndex..]);
            writeIndex += state.CommandName!.Length;
            if (ws)
            {
                span[writeIndex] = quote2;
                ++writeIndex;
            }

            int argIndex = 0;
            int flagIndex = 0;
            while (true)
            {
                for (; flagIndex < state.Flags.Length; ++flagIndex)
                {
                    ref CommandFlagInfo info = ref state.Flags[flagIndex];
                    if (info.ArgumentPosition >= argIndex)
                    {
                        break;
                    }

                    ++flagIndex;
                    span[writeIndex] = ' ';
                    ++writeIndex;
                    ws = ContainsWhiteSpaceOrQuotes(info.FlagName) || IsFlag(info.FlagName);
                    span.Slice(writeIndex, info.DashCount).Fill('-');
                    writeIndex += info.DashCount;
                    if (ws)
                    {
                        ChooseQuotes(info.FlagName, out quote1, out quote2);
                        span[writeIndex] = quote1;
                        ++writeIndex;
                    }
                    info.FlagName.AsSpan().CopyTo(span[writeIndex..]);
                    writeIndex += info.FlagName.Length;
                    if (ws)
                    {
                        span[writeIndex] = quote2;
                        ++writeIndex;
                    }
                }

                if (argIndex >= state.Arguments.Length)
                {
                    break;
                }

                string arg = state.Arguments[argIndex];
                span[writeIndex] = ' ';
                ++writeIndex;
                ws = ContainsWhiteSpaceOrQuotes(arg) || IsFlag(arg);
                if (ws)
                {
                    ChooseQuotes(arg, out quote1, out quote2);
                    span[writeIndex] = quote1;
                    ++writeIndex;
                }
                arg.AsSpan().CopyTo(span[writeIndex..]);
                writeIndex += arg.Length;
                if (ws)
                {
                    span[writeIndex] = quote2;
                    ++writeIndex;
                }
                ++argIndex;
            }
        });
    }

    private static void ChooseQuotes(string str, out char start, out char end)
    {
        ReadOnlySpan<char> startArgChars = [ '"', '\'', '`', '´', '“', '‘' ];
        ReadOnlySpan<char> endArgChars = [ '"', '\'', '`', '´', '”', '’' ];

        ReadOnlySpan<char> span = str.AsSpan();

        int candidate = 0;
        for (int i = candidate; i < startArgChars.Length; ++i)
        {
            char c1 = startArgChars[i];
            char c2 = endArgChars[i];
            if (span.IndexOf(c1) < 0 && (c2 == c1 || span.IndexOf(c2) < 0))
            {
                start = c1;
                end = c2;
                return;
            }
        }

        start = '"';
        end = '"';
    }

    private static bool IsFlag(string str)
    {
        if (str.Length < 2)
            return false;

        ReadOnlySpan<char> flagPrefixes = [ '-', '–', '—', '−' ];
        if (flagPrefixes.IndexOf(str[0]) < 0)
            return false;

        return str.Length < 2 && flagPrefixes.IndexOf(str[1]) >= 0 || flagPrefixes.IndexOf(str[1]) < 0 || flagPrefixes.IndexOf(str[2]) < 0;
    }

    private static bool ContainsWhiteSpaceOrQuotes(string str)
    {
        if (str.Length == 0)
            return true;
        
        ReadOnlySpan<char> allQuoteChars = [ '"', '\'', '`', '´', '“', '”', '‘', '’'];
        for (int j = 0; j < str.Length; ++j)
        {
            char c = str[j];
            if (char.IsWhiteSpace(c) || allQuoteChars.IndexOf(c) > 0)
                return true;
        }

        return false;
    }
}

/// <summary>
/// Provides basic information about command flags.
/// </summary>
/// <param name="dashCount">Number of dashes, either 1 or 2.</param>
/// <param name="argumentPosition">The argument before the flag, where 1 is the first argument. A value of zero indicates this flag was before the first argument.</param>
public readonly struct CommandFlagInfo(string name, int dashCount, int argumentPosition)
{
    /// <summary>
    /// Name of the flag not including the dashes.
    /// </summary>
    public readonly string FlagName = name;

    /// <summary>
    /// Number of dashes, either 1 or 2.
    /// </summary>
    public readonly int DashCount = dashCount;

    /// <summary>
    /// The argument before the flag, where 1 is the first argument. A value of zero indicates this flag was before the first argument.
    /// </summary>
    /// <remarks>Note that there can be more than one flag with the same argument position.</remarks>
    public readonly int ArgumentPosition = argumentPosition;

    /// <summary>
    /// Returns the flag as it was entered.
    /// </summary>
    public override string ToString()
    {
        return string.Create(DashCount + FlagName.Length, this, (span, state) =>
        {
            span[..state.DashCount].Fill('-');
            state.FlagName.AsSpan().CopyTo(span[state.DashCount..]);
        });
    }
}