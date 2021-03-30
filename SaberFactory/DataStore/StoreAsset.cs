using System.IO;
using UnityEngine;

namespace SaberFactory.DataStore
{
    /// <summary>
    /// Keeps information about the origin of the asset
    /// </summary>
    internal class StoreAsset
    {
        public readonly string RelativePath;
        public readonly string Name;
        public readonly string NameWithoutExtension;
        public readonly string Extension;

        public GameObject Prefab;

        public readonly AssetBundle AssetBundle;

        public StoreAsset(string relativePath, GameObject prefab, AssetBundle assetBundle)
        {
            RelativePath = relativePath;
            Name = System.IO.Path.GetFileName(RelativePath);
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