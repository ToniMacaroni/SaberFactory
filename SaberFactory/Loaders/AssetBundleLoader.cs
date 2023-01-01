using System.Collections.Generic;
using System.Threading.Tasks;
using SaberFactory.DataStore;
using UnityEngine;

namespace SaberFactory.Loaders;

/// <summary>
///     Base class for loading store assets
/// </summary>
internal abstract class AssetBundleLoader
{
    /// <summary>
    ///     File extension to look for when loading
    /// </summary>
    public abstract string HandledExtension { get; }

    /// <summary>
    ///     Get all the loadable file paths
    /// </summary>
    /// <returns></returns>
    public abstract ISet<AssetMetaPath> CollectFiles(PluginDirectories dirs);

    /// <summary>
    /// Loads the implemented asset by the <see cref="RelativePath"/>
    /// </summary>
    /// <param name="relativePath">the <see cref="RelativePath"/> to the asset</param>
    /// <returns><see cref="StoreAsset"/> with the implemented asset</returns>
    public abstract Task<StoreAsset> LoadStoreAssetAsync(RelativePath relativePath);

    /// <summary>
    /// Load the implemented asset by <see cref="AssetBundle"/>
    /// </summary>
    /// <param name="bundle">The <see cref="AssetBundle"/> to load the asset from</param>
    /// <param name="assetName">The new asset name for the asset</param>
    /// <returns><see cref="StoreAsset"/> with the implemented asset</returns>
    public abstract Task<StoreAsset> LoadStoreAssetFromBundleAsync(AssetBundle bundle, string assetName);
}