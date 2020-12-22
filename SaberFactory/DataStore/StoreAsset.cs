using UnityEngine;

namespace SaberFactory.DataStore
{
    /// <summary>
    /// Keeps information about the origin of the asset
    /// </summary>
    internal class StoreAsset
    {
        public readonly string Path;
        public readonly string Name;
        public readonly string NameWithoutExtension;
        public readonly string Extension;

        public readonly GameObject Prefab;

        public readonly AssetBundle AssetBundle;

        public StoreAsset(string path, GameObject prefab, AssetBundle assetBundle)
        {
            Path = path;
            Name = System.IO.Path.GetFileName(Path);
            NameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(Name);
            Extension = System.IO.Path.GetExtension(Name);

            Prefab = prefab;
            AssetBundle = assetBundle;
        }

        public void Unload()
        {
            AssetBundle.Unload(true);
        }
    }
}