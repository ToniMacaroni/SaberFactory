using System.IO;
using System.Linq;
using SaberFactory.Helpers;
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
        public readonly string SubDirName;

        public GameObject Prefab;

        public readonly AssetBundle AssetBundle;

        public StoreAsset(string relativePath, GameObject prefab, AssetBundle assetBundle)
        {
            RelativePath = relativePath;
            Name = Path.GetFileName(RelativePath);
            NameWithoutExtension = Path.GetFileNameWithoutExtension(Name);
            Extension = Path.GetExtension(Name);
            SubDirName = PathTools.GetSubDir(relativePath);

            Prefab = prefab;
            AssetBundle = assetBundle;
        }

        public void Unload()
        {
            AssetBundle.Unload(true);
        }
    }
}