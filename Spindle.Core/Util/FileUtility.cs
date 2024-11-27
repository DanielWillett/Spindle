using System;
using System.Collections.Generic;
using System.IO;

namespace Spindle.Util;

internal static class FileUtility
{
    private static readonly char[] InvalidCharacters = Path.GetInvalidFileNameChars();

    private static readonly HashSet<string> InvalidFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "CON", "PRN", "AUX", "NUL",                                                 // 1, 2, 3 superscripts
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM\u00B9", "COM\u00B2", "COM\u00B3",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9", "LPT\u00B9", "LPT\u00B2", "LPT\u00B3",
        ".", ".."
    };

    public static bool IsInvalidFileName(string name)
    {
        if (InvalidFileNames.Contains(name) || name.EndsWith(' ') || name.EndsWith('.'))
        {
            return false;
        }

        ReadOnlySpan<char> nameSpan = name;
        for (int i = 0; i < InvalidCharacters.Length; ++i)
        {
            if (nameSpan.IndexOf(InvalidCharacters[i]) >= 0)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Makes sure a <paramref name="name"/> is safe to be used as a file name. This can not be a whole path, just a name.
    /// </summary>
    public static string SanitizeFileName(string name)
    {
        ReadOnlySpan<char> trimTable = [ '.', ' ' ];

        ReadOnlySpan<char> nameSpan = name.AsSpan().TrimEnd(trimTable);

        bool isCopied = false;
        scoped Span<char> newSpan = default;
        for (int i = 0; i < InvalidCharacters.Length; ++i)
        {
            char c = InvalidCharacters[i];
            int index = -1;
            while (true)
            {
                index = nameSpan.IndexOf(c, index + 1);
                if (index < 0)
                    break;

                if (!isCopied)
                {
                    // ReSharper disable once StackAllocInsideLoop
                    newSpan = stackalloc char[nameSpan.Length];
                    nameSpan.CopyTo(newSpan);
                    isCopied = true;
                }

                newSpan[index] = '_';
            }
        }

        if (isCopied)
            return new string(newSpan);

        string newName = nameSpan.Length == name.Length ? name : new string(nameSpan);
        if (InvalidFileNames.Contains(newName))
        {
            newName = "_" + newName;
        }

        return newName;
    }

    public static string GetRelativeToSpindle(string languageFileName)
    {
        return Path.GetRelativePath(SpindlePaths.SpindleDirectory, languageFileName);
    }
}
