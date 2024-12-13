using DanielWillett.ReflectionTools.Emit;
using DanielWillett.ReflectionTools.Formatting;
using Spindle.Logging;
using Spindle.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Spindle.Unturned;

/// <summary>
/// Abstraction around the vanilla <see cref="Assets"/> class.
/// </summary>
public static class UnturnedAssets
{
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    public readonly struct AssetTypeInfoProxy
    {
        [FieldOffset(0)]
        internal readonly int Index;

        /// <summary>
        /// List of all assets in this category.
        /// </summary>
        /// <remarks>Assets of type <see cref="RedirectorAsset"/> are included in this list.</remarks>
        public IReadOnlyList<Asset> All
        {
            get
            {
                EnsureInitialized();
                return _legacyTypeListsReadOnly![Index] ??= new ReadOnlyCollection<Asset>(_legacyTypeLists![Index]);
            }
        }

        /// <summary>
        /// Dictionary of all assets in this category by their legacy ID.
        /// </summary>
        /// <remarks>Assets of type <see cref="RedirectorAsset"/> are included in this dictionary.</remarks>
        public IReadOnlyDictionary<ushort, Asset> ById
        {
            get
            {
                EnsureInitialized();
                return _legacyTypeDictionariesReadOnly![Index] ??= new ReadOnlyDictionary<ushort, Asset>(_legacyTypeDictionaries![Index]);
            }
        }

        /// <summary>
        /// Get an asset by it's category and legacy ID. If this asset is a <see cref="RedirectorAsset"/>, it will be resolved properly.
        /// </summary>
        [Pure]
        public Asset? Get(ushort id)
        {
            if (id == 0)
                return null;

            EnsureInitialized();
            _legacyTypeDictionaries![Index].TryGetValue(id, out Asset? asset);
            return ResolveRedirectAsset(asset);
        }

        /// <summary>
        /// Get an asset by it's category and GUID. If this asset is a <see cref="RedirectorAsset"/>, it will be resolved properly.
        /// </summary>
        /// <remarks>This is really no different than getting any asset by GUID. If the asset with this GUID is not in the current category, <see langword="null"/> will be returned instead.</remarks>
        [Pure]
        public Asset? Get(Guid guid)
        {
            Asset? asset = UnturnedAssets.Get(guid);
            if (asset != null && asset.assetCategory != _legacyTypesMin + Index)
            {
                return null;
            }

            return asset;
        }
    }

    private static volatile bool _hasInitializedAllAssets;
    private static volatile int _effectiveAssetVersion;

    private static Func<int>? _getAssetVersion;

    private static readonly object Sync = new object();

    private static Asset[]? _allAssets;
    private static Dictionary<Guid, Asset>? _allDictionary;
    private static Dictionary<ushort, Asset>[]? _legacyTypeDictionaries;
    private static Asset[][]? _legacyTypeLists;
    private static IReadOnlyList<Asset>? _allAssetsReadonly;
    private static IReadOnlyDictionary<Guid, Asset>? _allDictionaryReadOnly;
    private static IReadOnlyDictionary<ushort, Asset>?[]? _legacyTypeDictionariesReadOnly;
    private static IReadOnlyList<Asset>?[]? _legacyTypeListsReadOnly;
    private static EAssetType _legacyTypesMin;

    /// <summary>
    /// Dictionary of all loaded assets by their unique <see cref="Guid"/>.
    /// </summary>
    /// <remarks>Assets of type <see cref="RedirectorAsset"/> are included in this dictionary.</remarks>
    public static IReadOnlyDictionary<Guid, Asset> ByGuid
    {
        get
        {
            EnsureInitialized();
            return _allDictionaryReadOnly!;
        }
    }

    /// <summary>
    /// List of all assets currently loaded.
    /// </summary>
    public static IReadOnlyList<Asset> All
    {
        get
        {
            EnsureInitialized();
            return _allAssetsReadonly!;
        }
    }

    /// <summary>
    /// Get information about a specific category of assets.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Invalid asset type.</exception>
    [Pure]
    public static AssetTypeInfoProxy Category(EAssetType assetType)
    {
        EnsureInitialized();
        int index = GetAssetTypeIndex(assetType);
        return Unsafe.As<int, AssetTypeInfoProxy>(ref index);
    }

    [Pure]
    private static int GetAssetTypeIndex(EAssetType assetType)
    {
        int typeIndex = assetType - _legacyTypesMin;
        if (typeIndex < 0 || typeIndex > _legacyTypeLists!.Length)
            throw new ArgumentOutOfRangeException(nameof(assetType));
        return typeIndex;
    }

    [Pure]
    private static Asset? ResolveRedirectAsset(Asset? asset)
    {
        Asset? a = asset;
        for (int ct = 32; ct >= 0 && a is RedirectorAsset redir; _allDictionary!.TryGetValue(redir.TargetGuid, out a))
        {
            --ct;
        }

        return a;
    }

    /// <summary>
    /// Find an asset by its legacy ID and category. Unless necessary, you should always find assets by <see cref="Guid"/> instead of legacy IDs.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Invalid asset type.</exception>
    /// <remarks>Resolves <see cref="RedirectorAsset"/> automatically.</remarks>
    [Pure]
    public static Asset? Get(EAssetType assetType, ushort id)
    {
        if (id == 0 || assetType == EAssetType.NONE)
            return null;

        EnsureInitialized();
        _legacyTypeDictionaries![GetAssetTypeIndex(assetType)].TryGetValue(id, out Asset? asset);
        return ResolveRedirectAsset(asset);
    }

    /// <summary>
    /// Find an asset by its legacy ID and category. Unless necessary, you should always find assets by <see cref="Guid"/> instead of legacy IDs.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Invalid asset type.</exception>
    /// <param name="resolveRedirect">Resolves <see cref="RedirectorAsset"/> automatically.</param>
    [Pure]
    public static Asset? Get(EAssetType assetType, ushort id, bool resolveRedirect)
    {
        if (id == 0 || assetType == EAssetType.NONE)
            return null;

        EnsureInitialized();
        _legacyTypeDictionaries![GetAssetTypeIndex(assetType)].TryGetValue(id, out Asset? asset);
        return resolveRedirect ? ResolveRedirectAsset(asset) : asset;
    }

    /// <summary>
    /// Find an asset by its legacy ID. Unless necessary, you should always find assets by <see cref="Guid"/> instead of legacy IDs.
    /// </summary>
    /// <remarks>Resolves <see cref="RedirectorAsset"/> automatically.</remarks>
    [Pure]
    public static TAsset? Get<TAsset>(ushort id) where TAsset : Asset
    {
        return Get(GetAssetCategory<TAsset>(), id) as TAsset;
    }

    /// <summary>
    /// Find an asset by its unique <see cref="Guid"/>.
    /// </summary>
    [Pure]
    public static Asset? Get(Guid guid)
    {
        if (guid == Guid.Empty)
            return null;

        _allDictionary!.TryGetValue(guid, out Asset? asset);
        return ResolveRedirectAsset(asset);
    }

    /// <summary>
    /// Find an asset by its unique <see cref="Guid"/>.
    /// </summary>
    /// <param name="resolveRedirect">Resolves <see cref="RedirectorAsset"/> automatically.</param>
    [Pure]
    public static Asset? Get(Guid guid, bool resolveRedirect)
    {
        if (guid == Guid.Empty)
            return null;

        _allDictionary!.TryGetValue(guid, out Asset? asset);
        return resolveRedirect ? ResolveRedirectAsset(asset) : asset;
    }

    /// <summary>
    /// Find an asset by either its legacy ID or unique <see cref="Guid"/> in a specific category, prioritizing <paramref name="guid"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Invalid asset type.</exception>
    /// <param name="resolveRedirect">Resolves <see cref="RedirectorAsset"/> automatically.</param>
    [Pure]
    public static Asset? Get(EAssetType assetType, Guid guid, ushort id, bool resolveRedirect)
    {
        Asset? guidFind = Get(guid, resolveRedirect);
        if (guidFind == null)
            return Get(assetType, id, resolveRedirect);

        // throw error if out of range
        if (assetType != EAssetType.NONE)
            _ = GetAssetTypeIndex(assetType);
        return guidFind.assetCategory == assetType ? guidFind : null;
    }

    /// <summary>
    /// Find an asset by either its legacy ID or unique <see cref="Guid"/> in a specific category, prioritizing <paramref name="guid"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Invalid asset type.</exception>
    /// <remarks>Resolves <see cref="RedirectorAsset"/> automatically.</remarks>
    [Pure]
    public static Asset? Get(EAssetType assetType, Guid guid, ushort id)
    {
        Asset? guidFind = Get(guid);
        if (guidFind == null)
            return Get(assetType, id);

        // throw error if out of range
        if (assetType != EAssetType.NONE)
            _ = GetAssetTypeIndex(assetType);
        return guidFind.assetCategory == assetType ? guidFind : null;
    }

    /// <summary>
    /// Find an asset by either its legacy ID or unique <see cref="Guid"/> in a specific category, prioritizing <paramref name="guid"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Invalid asset type.</exception>
    /// <remarks>Resolves <see cref="RedirectorAsset"/> automatically.</remarks>
    [Pure]
    public static TAsset? Get<TAsset>(Guid guid, ushort id) where TAsset : Asset
    {
        EAssetType assetType = GetAssetCategory<TAsset>();

        Asset? guidFind = Get(guid);
        if (guidFind == null)
            return Get(assetType, id) as TAsset;

        // throw error if out of range
        if (assetType != EAssetType.NONE)
            _ = GetAssetTypeIndex(assetType);
        return guidFind.assetCategory == assetType ? guidFind as TAsset : null;
    }

    /// <summary>
    /// Find an asset by its unique <see cref="Guid"/>.
    /// </summary>
    /// <remarks>Resolves <see cref="RedirectorAsset"/> automatically.</remarks>
    [Pure]
    public static TAsset? Get<TAsset>(Guid guid) where TAsset : Asset
    {
        return Get(guid) as TAsset;
    }

    /// <summary>
    /// Find an asset by a reference to the asset.
    /// </summary>
    /// <remarks>Resolves <see cref="RedirectorAsset"/> automatically.</remarks>
    [Pure]
    public static TAsset? Get<TAsset>(AssetReference<TAsset> assetReference) where TAsset : Asset
    {
        return Get(assetReference.GUID) as TAsset;
    }

    /// <summary>
    /// Find an asset by a reference to the asset.
    /// </summary>
    /// <remarks>Resolves <see cref="RedirectorAsset"/> automatically.</remarks>
    [Pure]
    public static TAsset? Get<TAsset>(IAssetReference assetReference) where TAsset : Asset
    {
        return assetReference.isValid ? Get(assetReference.GUID) as TAsset : null;
    }

    /// <summary>
    /// Find an asset by a reference to the asset.
    /// </summary>
    /// <remarks>Resolves <see cref="RedirectorAsset"/> automatically.</remarks>
    [Pure]
    public static Asset? Get(IAssetReference assetReference)
    {
        return assetReference.isValid ? Get(assetReference.GUID) : null;
    }

    /// <summary>
    /// Find an asset by a reference to the asset.
    /// </summary>
    /// <param name="resolveRedirect">Resolves <see cref="RedirectorAsset"/> automatically.</param>
    [Pure]
    public static Asset? Get(IAssetReference assetReference, bool resolveRedirect)
    {
        return assetReference.isValid ? Get(assetReference.GUID, resolveRedirect) : null;
    }

    /// <summary>
    /// Find an asset by its legacy ID. Unless necessary, you should always find assets by <see cref="Guid"/> instead of legacy IDs.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Invalid asset type.</exception>
    /// <remarks>Resolves <see cref="RedirectorAsset"/> automatically.</remarks>
    public static bool TryGet(EAssetType assetType, ushort id, [MaybeNullWhen(false)] out Asset asset)
    {
        if (id == 0 || assetType == EAssetType.NONE)
        {
            asset = null;
            return false;
        }

        EnsureInitialized();
        if (_legacyTypeDictionaries![GetAssetTypeIndex(assetType)].TryGetValue(id, out Asset? assetNew))
        {
            asset = ResolveRedirectAsset(assetNew);
            return asset != null;
        }

        asset = null;
        return false;
    }

    /// <summary>
    /// Find an asset by its legacy ID. Unless necessary, you should always find assets by <see cref="Guid"/> instead of legacy IDs.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Invalid asset type.</exception>
    /// <param name="resolveRedirect">Resolves <see cref="RedirectorAsset"/> automatically.</param>
    public static bool TryGet(EAssetType assetType, ushort id, [MaybeNullWhen(false)] out Asset asset, bool resolveRedirect)
    {
        if (id == 0 || assetType == EAssetType.NONE)
        {
            asset = null;
            return false;
        }

        EnsureInitialized();
        if (_legacyTypeDictionaries![GetAssetTypeIndex(assetType)].TryGetValue(id, out Asset? assetNew))
        {
            asset = resolveRedirect ? ResolveRedirectAsset(assetNew) : assetNew;
            return asset != null;
        }

        asset = null;
        return false;
    }

    /// <summary>
    /// Find an asset by its legacy ID. Unless necessary, you should always find assets by <see cref="Guid"/> instead of legacy IDs.
    /// </summary>
    /// <remarks>Resolves <see cref="RedirectorAsset"/> automatically.</remarks>
    public static bool TryGet<TAsset>(ushort id, [MaybeNullWhen(false)] out TAsset asset) where TAsset : Asset
    {
        if (TryGet(GetAssetCategory<TAsset>(), id, out Asset? assetNew) && assetNew is TAsset assetCasted)
        {
            asset = assetCasted;
            return true;
        }

        asset = null;
        return false;
    }

    /// <summary>
    /// Find an asset by its unique <see cref="Guid"/>.
    /// </summary>
    /// <remarks>Resolves <see cref="RedirectorAsset"/> automatically.</remarks>
    public static bool TryGet<TAsset>(Guid guid, [MaybeNullWhen(false)] out TAsset asset) where TAsset : Asset
    {
        if (guid == Guid.Empty)
        {
            asset = null;
            return false;
        }

        EnsureInitialized();
        if (_allDictionary!.TryGetValue(guid, out Asset? assetNew))
        {
            asset = ResolveRedirectAsset(assetNew) as TAsset;
            return asset != null;
        }

        asset = null;
        return false;
    }

    /// <summary>
    /// Find an asset by its unique <see cref="Guid"/>.
    /// </summary>
    /// <remarks>Resolves <see cref="RedirectorAsset"/> automatically.</remarks>
    public static bool TryGet(Guid guid, [MaybeNullWhen(false)] out Asset asset)
    {
        if (guid == Guid.Empty)
        {
            asset = null;
            return false;
        }

        EnsureInitialized();
        if (_allDictionary!.TryGetValue(guid, out Asset? assetNew))
        {
            asset = ResolveRedirectAsset(assetNew);
            return asset != null;
        }

        asset = null;
        return false;
    }

    /// <summary>
    /// Find an asset by its unique <see cref="Guid"/>.
    /// </summary>
    /// <param name="resolveRedirect">Resolves <see cref="RedirectorAsset"/> automatically.</param>
    public static bool TryGet(Guid guid, [MaybeNullWhen(false)] out Asset asset, bool resolveRedirect)
    {
        if (guid == Guid.Empty)
        {
            asset = null;
            return false;
        }

        EnsureInitialized();
        if (_allDictionary!.TryGetValue(guid, out Asset? assetNew))
        {
            asset = resolveRedirect ? ResolveRedirectAsset(assetNew) : assetNew;
            return asset != null;
        }

        asset = null;
        return false;
    }

    /// <summary>
    /// Find an asset by a reference to the asset.
    /// </summary>
    /// <remarks>Resolves <see cref="RedirectorAsset"/> automatically.</remarks>
    public static bool TryGet<TAsset>(AssetReference<TAsset> assetReference, [MaybeNullWhen(false)] out TAsset asset) where TAsset : Asset
    {
        Guid guid = assetReference.GUID;
        return TryGet(guid, out asset);
    }

    /// <summary>
    /// Find an asset by a reference to the asset.
    /// </summary>
    /// <remarks>Resolves <see cref="RedirectorAsset"/> automatically.</remarks>
    public static bool TryGet<TAsset>(IAssetReference assetReference, [MaybeNullWhen(false)] out TAsset asset) where TAsset : Asset
    {
        if (assetReference.isValid)
            return TryGet(assetReference.GUID, out asset);

        asset = null;
        return false;
    }

    /// <summary>
    /// Find an asset by a reference to the asset.
    /// </summary>
    /// <param name="resolveRedirect">Resolves <see cref="RedirectorAsset"/> automatically.</param>
    public static bool TryGet(IAssetReference assetReference, [MaybeNullWhen(false)] out Asset asset)
    {
        if (assetReference.isValid)
            return TryGet(assetReference.GUID, out asset);

        asset = null;
        return false;
    }
    
    /// <summary>
    /// Find an asset by a reference to the asset.
    /// </summary>
    /// <param name="resolveRedirect">Resolves <see cref="RedirectorAsset"/> automatically.</param>
    public static bool TryGet(IAssetReference assetReference, [MaybeNullWhen(false)] out Asset asset, bool resolveRedirect)
    {
        if (assetReference.isValid)
            return TryGet(assetReference.GUID, out asset, resolveRedirect);

        asset = null;
        return false;
    }

    /// <summary>
    /// Find an asset by either its legacy ID or unique <see cref="Guid"/> in a specific category, prioritizing <paramref name="guid"/>.
    /// </summary>
    /// <param name="resolveRedirect">Resolves <see cref="RedirectorAsset"/> automatically.</param>
    public static bool TryGet<TAsset>(Guid guid, ushort id, [MaybeNullWhen(false)] out TAsset asset) where TAsset : Asset
    {
        EAssetType assetType = GetAssetCategory<TAsset>();

        Asset? guidFind = Get(guid);
        if (guidFind == null)
        {
            Asset? idFind = Get(assetType, id);
            if (idFind == null)
            {
                asset = null;
                return false;
            }

            asset = idFind as TAsset;
            return asset != null;
        }

        // throw error if out of range
        if (assetType != EAssetType.NONE)
            _ = GetAssetTypeIndex(assetType);

        if (guidFind.assetCategory != assetType)
        {
            asset = null;
            return false;
        }

        asset = guidFind as TAsset;
        return asset != null;
    }

    /// <summary>
    /// Find an asset by either its legacy ID or unique <see cref="Guid"/> in a specific category, prioritizing <paramref name="guid"/>.
    /// </summary>
    /// <param name="resolveRedirect">Resolves <see cref="RedirectorAsset"/> automatically.</param>
    public static bool TryGet(EAssetType assetType, Guid guid, ushort id, [MaybeNullWhen(false)] out Asset asset)
    {
        Asset? guidFind = Get(guid);
        if (guidFind == null)
        {
            Asset? idFind = Get(assetType, id);
            if (idFind == null)
            {
                asset = null;
                return false;
            }

            asset = idFind;
            return true;
        }

        // throw error if out of range
        if (assetType != EAssetType.NONE)
            _ = GetAssetTypeIndex(assetType);

        if (guidFind.assetCategory != assetType)
        {
            asset = null;
            return false;
        }

        asset = guidFind;
        return true;
    }

    /// <summary>
    /// Find an asset by either its legacy ID or unique <see cref="Guid"/> in a specific category, prioritizing <paramref name="guid"/>.
    /// </summary>
    /// <param name="resolveRedirect">Resolves <see cref="RedirectorAsset"/> automatically.</param>
    public static bool TryGet(EAssetType assetType, Guid guid, ushort id, [MaybeNullWhen(false)] out Asset asset, bool resolveRedirect)
    {
        Asset? guidFind = Get(guid, resolveRedirect);
        if (guidFind == null)
        {
            Asset? idFind = Get(assetType, id, resolveRedirect);
            if (idFind == null)
            {
                asset = null;
                return false;
            }

            asset = idFind;
            return true;
        }

        // throw error if out of range
        if (assetType != EAssetType.NONE)
            _ = GetAssetTypeIndex(assetType);

        if (guidFind.assetCategory != assetType)
        {
            asset = null;
            return false;
        }

        asset = guidFind;
        return true;
    }

    /// <summary>
    /// Returns the asset category (<see cref="EAssetType"/>) of <typeparamref name="TAsset"/>. Efficiently cached.
    /// </summary>
    [Pure]
    public static EAssetType GetAssetCategory<TAsset>() where TAsset : Asset => GetAssetCategoryCache<TAsset>.Category;

    /// <summary>
    /// Returns the asset category (<see cref="EAssetType"/>) of <paramref name="assetType"/>.
    /// </summary>
    [Pure]
    public static EAssetType GetAssetCategory(Type assetType)
    {
        if (typeof(ItemAsset).IsAssignableFrom(assetType))
        {
            return EAssetType.ITEM;
        }
        if (typeof(VehicleAsset).IsAssignableFrom(assetType))
        {
            return EAssetType.VEHICLE;
        }
        if (typeof(ObjectAsset).IsAssignableFrom(assetType))
        {
            return EAssetType.OBJECT;
        }
        if (typeof(EffectAsset).IsAssignableFrom(assetType))
        {
            return EAssetType.EFFECT;
        }
        if (typeof(AnimalAsset).IsAssignableFrom(assetType))
        {
            return EAssetType.ANIMAL;
        }
        if (typeof(SpawnAsset).IsAssignableFrom(assetType))
        {
            return EAssetType.SPAWN;
        }
        if (typeof(SkinAsset).IsAssignableFrom(assetType))
        {
            return EAssetType.SKIN;
        }
        if (typeof(MythicAsset).IsAssignableFrom(assetType))
        {
            return EAssetType.MYTHIC;
        }
        if (typeof(ResourceAsset).IsAssignableFrom(assetType))
        {
            return EAssetType.RESOURCE;
        }
        if (typeof(DialogueAsset).IsAssignableFrom(assetType) || typeof(QuestAsset).IsAssignableFrom(assetType) || typeof(VendorAsset).IsAssignableFrom(assetType))
        {
            return EAssetType.NPC;
        }

        return EAssetType.NONE;
    }

    private static void EnsureInitialized()
    {
        // ReSharper disable once InconsistentlySynchronizedField
        if (!_hasInitializedAllAssets)
        {
            InitializeAllAssets();
        }
        else if (_getAssetVersion != null && _effectiveAssetVersion != _getAssetVersion())
        {
            _hasInitializedAllAssets = false;
            InitializeAllAssets();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void InitializeAllAssets()
    {
        lock (Sync)
        {
            if (_hasInitializedAllAssets)
                return;

            SpindleLauncher.LoggerFactory.CreateLogger(typeof(UnturnedAssets)).LogConditional("Re-caching assets...");

            try
            {
                // pull assets from current mapping
                Type? assetMappingType = typeof(Assets).Assembly.GetType("SDG.Unturned.Assets+AssetMapping, Assembly-CSharp", false, false);
                if (assetMappingType == null)
                {
                    FallbackInitialze();
                    SpindleLauncher.LoggerFactory.CreateLogger(typeof(UnturnedAssets)).LogWarning(Properties.Resources.LogUnturnedAssetsTypeNotFound,
                        new TypeDefinition("AssetMapping").NestedIn<Assets>());
                    return;
                }

                FieldInfo? currentAssetMapping = typeof(Assets).GetField("currentAssetMapping", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (currentAssetMapping == null)
                {
                    FallbackInitialze();
                    SpindleLauncher.LoggerFactory.CreateLogger(typeof(UnturnedAssets)).LogWarning(Properties.Resources.LogUnturnedAssetsFieldNotFound,
                        new FieldDefinition("currentAssetMapping").DeclaredIn<Assets>(isStatic: true).WithFieldType(assetMappingType));
                    return;
                }

                FieldInfo? legacyAssetsTable = typeof(Assets).GetField("legacyAssetsTable", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (legacyAssetsTable == null)
                {
                    FallbackInitialze();
                    SpindleLauncher.LoggerFactory.CreateLogger(typeof(UnturnedAssets)).LogWarning(Properties.Resources.LogUnturnedAssetsFieldNotFound,
                        new FieldDefinition("legacyAssetsTable").DeclaredIn(assetMappingType, isStatic: false).WithFieldType<Dictionary<EAssetType, Dictionary<ushort, Asset>>>());
                    return;
                }

                FieldInfo? assetDictionary = typeof(Assets).GetField("assetDictionary", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (assetDictionary == null)
                {
                    FallbackInitialze();
                    SpindleLauncher.LoggerFactory.CreateLogger(typeof(UnturnedAssets)).LogWarning(Properties.Resources.LogUnturnedAssetsFieldNotFound,
                        new FieldDefinition("assetDictionary").DeclaredIn(assetMappingType, isStatic: false).WithFieldType<Dictionary<Guid, Asset>>());
                    return;
                }

                FieldInfo? assetList = typeof(Assets).GetField("assetList", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                if (assetList == null)
                {
                    FallbackInitialze();
                    SpindleLauncher.LoggerFactory.CreateLogger(typeof(UnturnedAssets)).LogWarning(Properties.Resources.LogUnturnedAssetsFieldNotFound,
                        new FieldDefinition("assetList").DeclaredIn(assetMappingType, isStatic: false).WithFieldType<List<Asset>>());
                    return;
                }

                if (_getAssetVersion == null)
                {
                    try
                    {
                        FieldInfo? modificationCounter = typeof(Assets).GetField("modificationCounter", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                        if (modificationCounter == null)
                        {
                            SpindleLauncher.LoggerFactory.CreateLogger(typeof(UnturnedAssets)).LogWarning(Properties.Resources.LogUnturnedAssetsVersionFieldNotFound,
                                new FieldDefinition("modificationCounter").DeclaredIn(assetMappingType, isStatic: false).WithFieldType<int>());
                        }
                        else
                        {
                            DynamicMethodInfo<Func<int>> versionGetter = DynamicMethodHelper.Create<Func<int>>("GetAssetVersion", typeof(UnturnedAssets), initLocals: false);
                            IOpCodeEmitter emit = versionGetter.GetEmitter();

                            emit.LoadStaticFieldValue(currentAssetMapping)
                                .LoadInstanceFieldValue(modificationCounter)
                                .Return();

                            _getAssetVersion = versionGetter.CreateDelegate(null);
                        }
                    }
                    catch (Exception ex)
                    {
                        SpindleLauncher.LoggerFactory.CreateLogger(typeof(UnturnedAssets)).LogWarning(ex, Properties.Resources.LogUnturnedAssetsVersionFieldFailed);
                    }
                }

                object assetMapping = currentAssetMapping.GetValue(null);
                Dictionary<EAssetType, Dictionary<ushort, Asset>> idTable = (Dictionary<EAssetType, Dictionary<ushort, Asset>>)legacyAssetsTable.GetValue(assetMapping);
                Dictionary<Guid, Asset> guidTable = (Dictionary<Guid, Asset>)assetDictionary.GetValue(assetMapping);
                List<Asset> list = (List<Asset>)assetList.GetValue(assetMapping);

                InitializeLegacyArrays(out EAssetType min, out EAssetType max);

                _allAssets = list.ToArray();
                _allAssetsReadonly = new ReadOnlyCollection<Asset>(list);

                _allDictionary = new Dictionary<Guid, Asset>(guidTable);
                _allDictionaryReadOnly = new ReadOnlyDictionary<Guid, Asset>(guidTable);

                foreach (KeyValuePair<EAssetType, Dictionary<ushort, Asset>> legacyAssetPair in idTable)
                {
                    EAssetType assetType = legacyAssetPair.Key;

                    if (assetType < min || assetType > max)
                        continue;

                    int index = assetType - min;
                    Dictionary<ushort, Asset> table = new Dictionary<ushort, Asset>(legacyAssetPair.Value);

                    _legacyTypeDictionaries![index] = table;
                    if (table.Count == 0)
                        continue;

                    Asset[] lists = new Asset[table.Count];
                    table.Values.CopyTo(lists, 0);
                    _legacyTypeLists![index] = lists;
                }

                for (int i = 0; i < _legacyTypeLists!.Length; ++i)
                {
                    _legacyTypeDictionaries![i] ??= new Dictionary<ushort, Asset>(0);
                    _legacyTypeLists[i] ??= Array.Empty<Asset>();
                }
            }
            catch (Exception ex)
            {
                FallbackInitialze();
                SpindleLauncher.LoggerFactory.CreateLogger(typeof(UnturnedAssets)).LogWarning(ex, Properties.Resources.LogUnturnedAssetsGenericError);
            }
            finally
            {
                Assets.onAssetsRefreshed += OnAssetsRefreshed;

                if (_getAssetVersion != null)
                {
                    _effectiveAssetVersion = _getAssetVersion();
                }

                _hasInitializedAllAssets = true;
            }
        }
    }

    private static void OnAssetsRefreshed()
    {
        InitializeAllAssets();
    }

    private static void InitializeLegacyArrays(out EAssetType min, out EAssetType max)
    {
        EAssetType maxVal = EnumValidationUtility.GetMaximumValue<EAssetType>();
        EAssetType minVal = EnumValidationUtility.GetMinimumValue<EAssetType>();

        if (minVal == EAssetType.NONE)
            ++minVal;

        int ct = maxVal - minVal + 1;

        if (_legacyTypeDictionaries == null)
            _legacyTypeDictionaries = new Dictionary<ushort, Asset>[ct];
        else
            Array.Clear(_legacyTypeDictionaries, 0, _legacyTypeDictionaries.Length);

        if (_legacyTypeDictionariesReadOnly == null)
            _legacyTypeDictionariesReadOnly = new IReadOnlyDictionary<ushort, Asset>[ct];
        else
            Array.Clear(_legacyTypeDictionariesReadOnly, 0, _legacyTypeDictionariesReadOnly.Length);

        if (_legacyTypeLists == null)
            _legacyTypeLists = new Asset[ct][];
        else
            Array.Clear(_legacyTypeLists, 0, _legacyTypeLists.Length);

        if (_legacyTypeListsReadOnly == null)
            _legacyTypeListsReadOnly = new IReadOnlyList<Asset>[ct];
        else
            Array.Clear(_legacyTypeListsReadOnly, 0, _legacyTypeListsReadOnly.Length);

        _legacyTypesMin = minVal;

        min = minVal;
        max = maxVal;
    }

    private static void FallbackInitialze()
    {
        List<Asset> allAssets = new List<Asset>(4096);
        Assets.find(allAssets);

        InitializeLegacyArrays(out EAssetType min, out EAssetType max);

        _allAssets = allAssets.ToArray();
        _allAssetsReadonly = new ReadOnlyCollection<Asset>(_allAssets);

        Dictionary<Guid, Asset> guidTable = new Dictionary<Guid, Asset>(allAssets.Count);

        foreach (Asset asset in allAssets)
        {
            guidTable[asset.GUID] = asset;

            EAssetType category = asset.assetCategory;
            if (category < min || category > max)
                continue;

            if (asset.id != 0)
            {
                (_legacyTypeDictionaries![category - min] ??= new Dictionary<ushort, Asset>(256))[asset.id] = asset;
            }
        }

        for (int i = 0; i < _legacyTypeLists!.Length; ++i)
        {
            _legacyTypeDictionaries![i] ??= new Dictionary<ushort, Asset>(0);

            Dictionary<ushort, Asset> dict = _legacyTypeDictionaries[i];
            if (dict.Count == 0)
            {
                _legacyTypeLists[i] = Array.Empty<Asset>();
                continue;
            }

            Asset[] list = new Asset[dict.Count];
            dict.Values.CopyTo(list, 0);
            _legacyTypeLists[i] = list;
        }

        _allDictionary = guidTable;
        _allDictionaryReadOnly = new ReadOnlyDictionary<Guid, Asset>(guidTable);
    }

    private static class GetAssetCategoryCache<TAsset> where TAsset : Asset
    {
        // ReSharper disable once MemberHidesStaticFromOuterClass
        public static readonly EAssetType Category = GetAssetCategory(typeof(TAsset));
    }
}