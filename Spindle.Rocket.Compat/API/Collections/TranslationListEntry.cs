using System;
using System.Xml.Serialization;

namespace Rocket.API.Collections;

[XmlType(AnonymousType = false, IncludeInSchema = true, TypeName = "Translation")]
[Serializable]
[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public class TranslationListEntry
{
    [XmlAttribute]
    public string Id;

    [XmlAttribute]
    public string Value;

    public TranslationListEntry() { }

    public TranslationListEntry(string id, string value)
    {
        Id = id;
        Value = value;
    }
}