using System.Collections.Generic;
using System.Threading.Tasks;
using SaberFactory.DataStore;
using SaberFactory.Helpers;

namespace SaberFactory.Loaders
{
    internal abstract class AssetBundleLoader
    {
        public abstract string HandledExtension { get; }

        public async Task<IEnumerable<StoreAsset>> LoadAllStoreAssetsAsync(IEnumerable<string> paths)
        {
            var assets = new List<StoreAsset>();
            foreach (var path in paths)
            {
                var relativePath = PathTools.ToRelativePath(path);
                assets.Add(await LoadStoreAssetAsync(relativePath));
            }

            return assets;
        }

        public async Task<IEnumerable<StoreAsset>> LoadAllStoreAssetsAsync()
        {
            var paths = CollectFiles();
            return await LoadAllStoreAssetsAsync(paths);
        }

        public abstract IEnumerable<string> CollectFiles();

        public abstract Task<StoreAsset> LoadStoreAssetAsync(string relativePath);
    }
}