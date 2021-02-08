using System.IO;
using SaberFactory.Helpers;

namespace SaberFactory.Loaders
{
    internal class AssetMetaPath
    {
        public string Path;
        public string MetaDataPath;

        public bool HasMetaData => !string.IsNullOrEmpty(MetaDataPath) && File.Exists(MetaDataPath);
        public string RelativePath => PathTools.ToRelativePath(Path);
        public string RelativeMetaDataPath => PathTools.ToRelativePath(MetaDataPath);

        public AssetMetaPath(string path, string metaDataPath = null)
        {
            Path = path;
            MetaDataPath = metaDataPath ?? path+".meta";
        }
    }
}