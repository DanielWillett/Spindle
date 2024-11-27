using System;
using System.Collections.Generic;

namespace Spindle.Localization;

/// <summary>
/// Manages all registered <see cref="TranslationCollection"/> types.
/// </summary>
/// <remarks>Can be <see cref="IDisposable"/>.</remarks>
public interface ITranslationService
{
    /// <summary>
    /// Dictionary of all translation collections with their types as a key.
    /// </summary>
    IReadOnlyDictionary<Type, TranslationCollection> TranslationCollections { get; }

    // /// <summary>
    // /// Accessor for enumerating certain player groups based on their language settings.
    // /// </summary>
    // LanguageSets SetOf { get; }

    /// <summary>
    /// Get a translation collection from this provider.
    /// </summary>
    T Get<T>() where T : TranslationCollection, new();


    /// <summary>
    /// Get a translation collection from this provider.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid collection type.</exception>
    TranslationCollection Get(Type type);
}