using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Rocket.API.Collections;

[XmlRoot("Translations")]
[XmlType(AnonymousType = false, IncludeInSchema = true, TypeName = "Translation")]
[Serializable]
[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public class TranslationList : IDefaultable, IEnumerable<TranslationListEntry>
{
    // ReSharper disable once InconsistentNaming
    protected List<TranslationListEntry> translations = new List<TranslationListEntry>();

    public int Count => translations.Count;

    public IEnumerator<TranslationListEntry> GetEnumerator()
    {
        return translations.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => translations.GetEnumerator();

    public void Add(object o) => translations.Add(o as TranslationListEntry ?? throw new ArgumentException("Expected TranslationListEntry."));

    public void Add(string key, string value)
    {
        translations.Add(new TranslationListEntry(key, value));
    }

    public void AddRange(IEnumerable<TranslationListEntry> collection)
    {
        translations.AddRange(collection);
    }

    public void AddRange(TranslationList collection)
    {
        translations.AddRange(collection.translations);
    }

    public string this[string key]
    {
        get
        {
            return translations.FirstOrDefault(k => k.Id == key)?.Value;
        }
        set
        {
            translations.ForEach(k =>
            {
                if (k.Id == key)
                    k.Value = value;
            });
        }
    }

    public string Translate(string translationKey, params object[] placeholder)
    {
        string format = this[translationKey];

        if (string.IsNullOrEmpty(format))
            return translationKey;

        if (!format.Contains("{") || !format.Contains("}") || placeholder == null || placeholder.Length == 0)
            return format;

        for (int index = 0; index < placeholder.Length; ++index)
        {
            placeholder[index] ??= "NULL";
        }

        return string.Format(format, placeholder);
    }

    public virtual void LoadDefaults() { }
}