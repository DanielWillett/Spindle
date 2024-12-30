using System;
using System.ComponentModel;

namespace Spindle.Interaction.Commands;

/// <summary>
/// Default light-weight implementation of <see cref="ICommandParser"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public class SpindleCommandParser : ICommandParser
{
    private static readonly ParsedCommandInfo Failure = new ParsedCommandInfo(null, Array.Empty<string>(), Array.Empty<CommandFlagInfo>());

    public ParsedCommandInfo ParseCommandInput(ReadOnlySpan<char> originalMessage, bool requirePrefix)
    {
        // remove slash that gets put at the end a lot since its right next to enter.
        originalMessage = originalMessage.TrimEnd('\\').TrimStart();

        if (originalMessage.Length < (requirePrefix ? 1 : 0) + 1)
            return Failure;

        ReadOnlySpan<char> startArgChars = [ '"', '\'', '`', '´', '“', '‘' ];
        ReadOnlySpan<char> endArgChars = [ '"', '\'', '`', '´', '”', '’' ];

        // yes these are all different characters
        ReadOnlySpan<char> flagPrefixes = [ '-', '–', '—', '−' ];

        if (requirePrefix)
        {
            ReadOnlySpan<char> prefixes = [ '/', '@', '\\' ];
            char prefix = originalMessage[0];
            if (prefixes.IndexOf(prefix) < 0)
                return Failure;

            originalMessage = originalMessage[1..].TrimStart();
        }

        if (originalMessage.Length < 1)
            return Failure;

        ReadOnlySpan<char> command = null;

        // rare but supported case of something like: '/"command name" [args..]'
        int startQuote = startArgChars.IndexOf(originalMessage[0]);
        if (startQuote >= 0 && originalMessage.Length > 1)
        {
            int endQuote = originalMessage[1..].IndexOf(endArgChars[startQuote]);

            if (endQuote == 0)
            {
                originalMessage = originalMessage[2..].TrimStart();
            }
            else
            {
                if (endQuote < 0)
                    endQuote = originalMessage.Length - 1;

                command = originalMessage[1..(endQuote + 1)];
                originalMessage = endQuote == originalMessage.Length - 1 ? ReadOnlySpan<char>.Empty : originalMessage[(endQuote + 2)..].TrimStart();
            }
        }

        if (command.IsEmpty)
        {
            int firstWhiteSpace = 0;
            while (firstWhiteSpace < originalMessage.Length && !char.IsWhiteSpace(originalMessage[firstWhiteSpace]))
                ++firstWhiteSpace;

            command = originalMessage[..firstWhiteSpace];
            originalMessage = originalMessage[firstWhiteSpace..].TrimStart();
        }

        if (originalMessage.Length == 0)
            return new ParsedCommandInfo(command.IsEmpty ? null : new string(command), Array.Empty<string>(), Array.Empty<CommandFlagInfo>());

        // count args
        int argCt = 0, flagCt = 0;

        ReadOnlySpan<char> args = originalMessage;
        while (!args.IsEmpty)
        {
            ReadOnlySpan<char> next = GetNextArg(ref args, startArgChars, endArgChars, flagPrefixes, out bool isEmpty, out int flagDashCt);
            if (!isEmpty && next.IsEmpty)
                break;

            if (flagDashCt > 0)
                ++flagCt;
            else
                ++argCt;
        }

        string[] argOutput = argCt == 0 ? Array.Empty<string>() : new string[argCt];
        CommandFlagInfo[] flagOutput = flagCt == 0 ? Array.Empty<CommandFlagInfo>() : new CommandFlagInfo[flagCt];
        argCt = -1;
        flagCt = -1;
        while (!originalMessage.IsEmpty)
        {
            ReadOnlySpan<char> next = GetNextArg(ref originalMessage, startArgChars, endArgChars, flagPrefixes, out bool isEmpty, out int flagDashCt);
            if (!isEmpty && next.IsEmpty)
                break;

            string str = new string(next);
            if (flagDashCt > 0)
                flagOutput[++flagCt] = new CommandFlagInfo(str, flagDashCt, argCt);
            else
                argOutput[++argCt] = str;
        }

        return new ParsedCommandInfo(new string(command), argOutput, flagOutput);
    }

    private static ReadOnlySpan<char> GetNextArg(ref ReadOnlySpan<char> args, ReadOnlySpan<char> startArgChars, ReadOnlySpan<char> endArgChars, ReadOnlySpan<char> flagPrefixes, out bool isEmpty, out int flagDashCt)
    {
        while (!args.IsEmpty)
        {
            ReadOnlySpan<char> arg;
            char c = args[0];
            int startQuote = startArgChars.IndexOf(c);
            if (startQuote >= 0)
            {
                int endQuote = args[1..].IndexOf(endArgChars[startQuote]);

                isEmpty = endQuote == 0;

                if (endQuote < 0)
                    endQuote = args.Length - 1;

                arg = args[1..(endQuote + 1)];
                flagDashCt = 0;
                args = endQuote == args.Length - 1 ? ReadOnlySpan<char>.Empty : args[(endQuote + 2)..].TrimStart();

                while (!arg.IsEmpty && endArgChars.IndexOf(arg[^1]) >= 0)
                    arg = arg[..^1];

                return arg;
            }

            isEmpty = false;
            int flagPrefix = flagPrefixes.IndexOf(c);
            if (flagPrefix >= 0)
            {
                int firstNonFlag = 1;
                while (firstNonFlag < args.Length && flagPrefixes[flagPrefix] == args[firstNonFlag])
                    ++firstNonFlag;

                if (firstNonFlag is 1 or 2 && firstNonFlag < args.Length && !char.IsDigit(args[firstNonFlag]) && args[firstNonFlag] != '.' && args[firstNonFlag] != ',')
                {
                    args = args[firstNonFlag..];
                    ReadOnlySpan<char> span = GetNextArg(ref args, startArgChars, endArgChars, flagPrefixes, out isEmpty, out flagDashCt);
                    flagDashCt = firstNonFlag;
                    return span;
                }
            }

            int firstWhiteSpace = 0;
            while (firstWhiteSpace < args.Length && !(char.IsWhiteSpace(args[firstWhiteSpace]) || startArgChars.IndexOf(args[firstWhiteSpace]) >= 0))
                ++firstWhiteSpace;

            arg = args[..firstWhiteSpace];
            args = args[firstWhiteSpace..].TrimStart();

            flagDashCt = 0;
            while (!arg.IsEmpty && endArgChars.IndexOf(arg[^1]) >= 0)
                arg = arg[..^1];

            return arg;
        }

        flagDashCt = 0;
        isEmpty = false;
        return ReadOnlySpan<char>.Empty;
    }
}