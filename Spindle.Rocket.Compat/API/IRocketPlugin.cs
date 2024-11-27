using Rocket.API.Collections;

namespace Rocket.API;

[TypeForwardedFrom(RocketCompatIntl.RocketAPIAssembly)]
public interface IRocketPlugin
{
    string Name { get; }

    PluginState State { get; }

    TranslationList DefaultTranslations { get; }

    IAsset<TranslationList> Translations { get; }

    T TryAddComponent<T>() where T : Component;

    void TryRemoveComponent<T>() where T : Component;
}