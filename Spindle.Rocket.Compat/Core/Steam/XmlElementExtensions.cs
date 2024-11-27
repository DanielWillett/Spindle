using System;
using System.Globalization;
using System.Xml;

namespace Rocket.Core.Steam;

[TypeForwardedFrom(RocketCompatIntl.RocketCoreAssembly)]
public static class XmlElementExtensions
{
    public static string ParseString(this XmlElement element) => element.InnerText;

    public static DateTime? ParseDateTime(this XmlElement element, CultureInfo cultureInfo)
    {
        try
        {
            return element == null ? null : DateTime.Parse(element.InnerText.Replace("st", "").Replace("nd", "").Replace("rd", "").Replace("th", ""), cultureInfo);
        }
        catch
        {
            return null;
        }
    }

    public static double? ParseDouble(this XmlElement element, CultureInfo cultureInfo)
    {
        try
        {
            return element == null ? null : double.Parse(element.InnerText, cultureInfo);
        }
        catch
        {
            return null;
        }
    }

    public static ushort? ParseUInt16(this XmlElement element, CultureInfo cultureInfo)
    {
        try
        {
            return element == null ? null : ushort.Parse(element.InnerText, cultureInfo);
        }
        catch
        {
            return null;
        }
    }

    public static uint? ParseUInt32(this XmlElement element, CultureInfo cultureInfo)
    {
        try
        {
            return element == null ? null : uint.Parse(element.InnerText, cultureInfo);
        }
        catch
        {
            return null;
        }
    }

    public static ulong? ParseUInt64(this XmlElement element, CultureInfo cultureInfo)
    {
        try
        {
            return element == null ? null : ulong.Parse(element.InnerText, cultureInfo);
        }
        catch
        {
            return null;
        }
    }

    public static bool? ParseBool(this XmlElement element)
    {
        try
        {
            return element != null ? element.InnerText == "1" : null;
        }
        catch
        {
            return null;
        }
    }

    public static Uri ParseUri(this XmlElement element)
    {
        try
        {
            return element == null ? null : new Uri(element.InnerText);
        }
        catch
        {
            return null;
        }
    }
}