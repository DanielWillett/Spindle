using Spindle.Logging;
using Spindle.Plugins;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;

namespace Spindle.Localization;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public class SpindleTranslationService : ITranslationService
{
    private readonly ConcurrentDictionary<Type, TranslationCollection> _collections;

    private readonly ILanguageService _languageService;
    private readonly ITranslationDataStore _translationStorage;
    private readonly IValueStringConvertService _converter;
    private readonly ILoggerFactory _loggerFactory;
    private readonly SpindlePluginLoader _pluginLoader;

    public IReadOnlyDictionary<Type, TranslationCollection> TranslationCollections { get; }

    public SpindleTranslationService(ILanguageService languageService, ITranslationDataStore translationStorage, IValueStringConvertService converter, ILoggerFactory loggerFactory, SpindlePluginLoader pluginLoader)
    {
        _languageService = languageService;
        _translationStorage = translationStorage;
        _converter = converter;
        _loggerFactory = loggerFactory;
        _pluginLoader = pluginLoader;

        _collections = new ConcurrentDictionary<Type, TranslationCollection>();
        TranslationCollections = new ReadOnlyDictionary<Type, TranslationCollection>(_collections);
    }

    internal void InitializeViaReflection()
    {
        // ensures all files have been created at startup instead of as they're used.
        foreach (Type type in _pluginLoader.GetTypesOfBaseType<TranslationCollection>(true, allowValueTypes: false))
        {
            _ = Get(type);
        }
    }

    public T Get<T>() where T : TranslationCollection, new()
    {
        TranslationCollection c = _collections.GetOrAdd(typeof(T), _ => new T());

        c.TryInitialize(this, _languageService, _translationStorage, _converter, _loggerFactory);
        return (T)c;
    }

    public TranslationCollection Get(Type type)
    {
        TranslationCollection c = _collections.GetOrAdd(type, _ =>
        {
            if (!type.IsClass || type.IsAbstract || !type.IsSubclassOf(typeof(TranslationCollection)))
                throw new ArgumentException(Properties.Resources.ExceptionNotValidTranslationCollectionType);

            ConstructorInfo? ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
                throw new ArgumentException(Properties.Resources.ExceptionNotValidTranslationCollectionType);

            return (TranslationCollection)ctor.Invoke(Array.Empty<object>());
        });

        c.TryInitialize(this, _languageService, _translationStorage, _converter, _loggerFactory);
        return c;
    }
}
