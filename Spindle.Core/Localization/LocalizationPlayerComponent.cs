using Microsoft.Extensions.DependencyInjection;
using Spindle.Players;
using System;
using System.Globalization;

namespace Spindle.Localization;
public sealed class LocalizationPlayerComponent : IPlayerComponent
{
    public SpindlePlayer Player { get; private set; }

    public Language Language { get; private set; }
    public CultureInfo Culture { get; private set; }

    public void Initialize(SpindlePlayer player, IServiceProvider serviceProvider)
    {
        Player = player;

        ILanguageService langService = serviceProvider.GetRequiredService<ILanguageService>();

        Language = langService.GetDefaultLanguage();
        Culture = langService.GetDefaultCulture();
    }
}
