namespace Rocket.API.Collections;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public static class TranslationListExtension
{
    public static void AddUnknownEntries(this TranslationList defaultTranslations, IAsset<TranslationList> translations)
    {
        bool needsSave = false;
        foreach (TranslationListEntry entry in defaultTranslations)
        {
            if (translations.Instance[entry.Id] != null)
                continue;

            translations.Instance.Add(entry);
            needsSave = true;
        }

        if (needsSave)
            translations.Save();
    }
}