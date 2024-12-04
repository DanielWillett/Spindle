using Spindle.Players;
using System.Globalization;

namespace Spindle.Localization;
public sealed class LocalizationPlayerComponent : IPlayerComponent
{
    private readonly ILanguageService _languageService;

#nullable disable

    public SpindlePlayer Player { get; private set; }
    public Language Language { get; private set; }
    public CultureInfo Culture { get; private set; }

#nullable restore

    public LocalizationPlayerComponent(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    void IPlayerComponent.Initialize(SpindlePlayer player)
    {
        Player = player;

        Language = _languageService.GetDefaultLanguage();
        Culture = _languageService.GetDefaultCulture();
    }
}