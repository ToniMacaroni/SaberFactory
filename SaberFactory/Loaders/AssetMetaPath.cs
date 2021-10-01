using System.IO;
using SaberFactory.Helpers;

namespace SaberFactory.Loaders
{
    internal class AssetMetaPath
    {
        public string Path => File.FullName;

        public bool HasMetaData => !string.IsNullOrEmpty(MetaDataPath) && System.IO.File.Exists(MetaDataPath);
        public string RelativePath => PathTools.ToRelativePath(Path);
        public string RelativeMetaDataPath => PathTools.ToRelativePath(MetaDataPath);
        public readonly FileInfo File;
        public readonly string MetaDataPath;

        public string SubDirName;

        public AssetMetaPath(FileInfo file, string metaDataPath = null)
        {
            File = file;
            MetaDataPath = metaDataPath ?? Path + ".meta";

            SubDirName = PathTools.GetSubDir(RelativePath);
        }
    }
}