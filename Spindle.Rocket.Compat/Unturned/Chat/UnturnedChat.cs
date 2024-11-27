using Rocket.API;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rocket.Unturned.Chat;

[TypeForwardedFrom(RocketCompatIntl.RocketUnturnedAssembly)]
public sealed class UnturnedChat : MonoBehaviour
{
    public static Color GetColorFromName(string colorName, Color fallback)
    {
        string lower = colorName.Trim().ToLower();
        switch (lower.Length)
        {
            case 3:
                if (lower == "red")
                    return Color.red;
                break;

            case 4:
                switch (lower[0])
                {
                    case 'b':
                        if (lower == "blue")
                            return Color.blue;
                        break;
                    case 'c':
                        if (lower == "cyan")
                            return Color.cyan;
                        break;
                    case 'g':
                        switch (lower)
                        {
                            case "gray":
                                return Color.gray;
                            case "grey":
                                return Color.grey;
                        }
                        break;
                }
                break;

            case 5:
                switch (lower[0])
                {
                    case 'b':
                        if (lower == "black")
                            return Color.black;
                        break;
                    case 'c':
                        if (lower == "clear")
                            return Color.clear;
                        break;
                    case 'g':
                        if (lower == "green")
                            return Color.green;
                        break;
                    case 'w':
                        if (lower == "white")
                            return Color.white;
                        break;
                }
                break;

            case 6:
                switch (lower[0])
                {
                    case 'r':
                        if (lower == "rocket")
                            return GetColorFromRGB(90, 206, 205);
                        break;
                    case 'y':
                        if (lower == "yellow")
                            return Color.yellow;
                        break;
                }
                break;

            case 7:
                if (lower == "magenta")
                    return Color.magenta;
                break;
        }
        Color? colorFromHex = GetColorFromHex(colorName);
        return colorFromHex ?? fallback;
    }

    public static Color? GetColorFromHex(string hexString)
    {
        hexString = hexString.Replace("#", "");
        if (hexString.Length == 3)
        {
            hexString = hexString.Insert(1, Convert.ToString(hexString[0]));
            hexString = hexString.Insert(3, Convert.ToString(hexString[2]));
            hexString = hexString.Insert(5, Convert.ToString(hexString[4]));
        }

        if (hexString.Length != 6 || !int.TryParse(hexString, NumberStyles.HexNumber, null, out int result))
            return null;

        byte r = (byte)(result >> 16 & byte.MaxValue);
        byte g = (byte)(result >> 8 & byte.MaxValue);
        byte b = (byte)(result & byte.MaxValue);
        return GetColorFromRGB(r, g, b);
    }

    public static Color GetColorFromRGB(byte R, byte G, byte B)
    {
        return GetColorFromRGB(R, G, B, 100);
    }

    public static Color GetColorFromRGB(byte R, byte G, byte B, short A)
    {
        return new Color(0.003921569f * R, 0.003921569f * G, 0.003921569f * B, 0.01f * A);
    }
    public static void Say(string message, bool rich)
    {
        Say(message, Palette.SERVER, rich);
    }

    public static void Say(string message)
    {
        Say(message, false);
    }

    public static void Say(IRocketPlayer player, string message)
    {
        Say(player, message, false);
    }

    public static void Say(IRocketPlayer player, string message, Color color, bool rich)
    {
        if (player is ConsolePlayer)
            Logger.Log(message, ConsoleColor.Gray);
        else
            Say(new CSteamID(ulong.Parse(player.Id)), message, color, rich);
    }

    public static void Say(IRocketPlayer player, string message, Color color)
    {
        Say(player, message, color, false);
    }

    public static void Say(string message, Color color, bool rich)
    {
        Logger.Log("Broadcast: " + message, ConsoleColor.Gray);
        ChatManager.serverSendMessage(message, color, null, null, EChatMode.SAY, null, rich);
    }

    public static void Say(string message, Color color)
    {
        Say(message, color, false);
    }

    public static void Say(IRocketPlayer player, string message, bool rich)
    {
        Say(player, message, Palette.SERVER, rich);
    }

    public static void Say(CSteamID CSteamID, string message, bool rich)
    {
        Say(CSteamID, message, Palette.SERVER, rich);
    }

    public static void Say(CSteamID CSteamID, string message)
    {
        Say(CSteamID, message, false);
    }

    public static void Say(CSteamID CSteamID, string message, Color color, bool rich)
    {
        if (CSteamID.ToString() == "0")
        {
            Logger.Log(message, ConsoleColor.Gray);
        }
        else
        {
            SteamPlayer steamPlayer = PlayerTool.getSteamPlayer(CSteamID);
            ChatManager.serverSendMessage(message, color, null, steamPlayer, EChatMode.SAY, null, rich);
        }
    }

    public static void Say(CSteamID CSteamID, string message, Color color)
    {
        Say(CSteamID, message, color, false);
    }

    [Obsolete("No longer wraps messages, just returns a list (ct=1) with 'text' in it.")]
    public static List<string> wrapMessage(string text)
    {
        return new List<string>(1) { text };
    }
}
