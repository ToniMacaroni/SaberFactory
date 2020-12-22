using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IPA.Utilities;
using SaberFactory.DataStore;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.Loaders
{
    internal class CustomSaberAssetLoader : AssetBundleLoader
    {
        public override string HandledExtension => ".saber";

        public override IEnumerable<string> CollectFiles()
        {
            var dir1 = new DirectoryInfo(Path.Combine(UnityGame.InstallPath, "CustomSabers"));
            var dir2 = new DirectoryInfo(Path.Combine(PathTools.SaberFactoryUserPath, "CustomSabers"));

            var paths = new HashSet<string>();

            if (dir1.Exists)
            {
                foreach (var path in dir1.EnumerateFiles("*.saber", SearchOption.AllDirectories))
                {
                    paths.Add(path.FullName);
                }
            }

            if (dir2.Exists)
            {
                foreach (var path in dir2.EnumerateFiles("*.saber", SearchOption.AllDirectories))
                {
                    paths.Add(path.FullName);
                }
            }

            return paths;
        }

        public override async Task<StoreAsset> LoadStoreAssetAsync(string relativePath)
        {
            var fullPath = PathTools.ToFullPath(relativePath);
            var result = await AsyncReaders.LoadAssetFromAssetBundleAsync<GameObject>(fullPath, "_CustomSaber");
            if (result == null) return null;
            return new StoreAsset(relativePath, result.Item1, result.Item2);
        }
    }
}