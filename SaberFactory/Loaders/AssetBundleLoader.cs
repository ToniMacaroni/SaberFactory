using System.Collections.Generic;
using System.Threading.Tasks;
using SaberFactory.DataStore;

namespace SaberFactory.Loaders
{
    /// <summary>
    ///     Base class for loading store assets
    /// </summary>
    internal abstract class AssetBundleLoader
    {
        /// <summary>
        ///     File extension to look for while loading
        /// </summary>
        public abstract string HandledExtension { get; }

        /// <summary>
        ///     Get all the loadable file paths
        /// </summary>
        /// <returns></returns>
        public abstract ISet<AssetMetaPath> CollectFiles(PluginDirectories dirs);

        public abstract Task<StoreAsset> LoadStoreAssetAsync(string relativePath);
    }
}