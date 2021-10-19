using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SaberFactory.DataStore;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.Loaders
{
    internal class CustomSaberAssetLoader : AssetBundleLoader
    {
        public override string HandledExtension => ".saber";

        public override ISet<AssetMetaPath> CollectFiles(PluginDirectories dirs)
        {
            var paths = new HashSet<AssetMetaPath>();

            foreach (var path in dirs.CustomSaberDir.EnumerateFiles("*.saber", SearchOption.AllDirectories))
            {
                paths.Add(new AssetMetaPath(path));
            }

            return paths;
        }

        public override async Task<StoreAsset> LoadStoreAssetAsync(string relativePath)
        {
            var fullPath = PathTools.ToFullPath(relativePath);
            if (!File.Exists(fullPath))
            {
                return null;
            }

            var result = await Readers.LoadAssetFromAssetBundleAsync<GameObject>(fullPath, "_CustomSaber");
            if (result == null)
            {
                return null;
            }

            return new StoreAsset(relativePath, result.Item1, result.Item2);
        }
    }
}