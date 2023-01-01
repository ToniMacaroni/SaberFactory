using System.IO;
using SaberFactory.DataStore;
using SaberFactory.Helpers;

namespace SaberFactory.Loaders
{
    internal class AssetMetaPath
    {
        /// <summary>
        /// Full path to the asset file
        /// </summary>
        public string Path => _file.FullName;

        /// <summary>
        /// Checks if the metadata file exists
        /// </summary>
        public bool HasMetaData => !string.IsNullOrEmpty(_metaDataPath) && System.IO.File.Exists(_metaDataPath);
        
        /// <summary>
        /// Relative path to the asset file
        /// </summary>
        public RelativePath RelativePath { get; }

        /// <summary>
        /// Relative path to the metadata file
        /// </summary>
        public RelativePath RelativeMetaDataPath { get; }

        /// <summary>
        /// <see cref="FileInfo"/> of the asset file
        /// </summary>
        public FileInfo File => _file;
        
        /// <summary>
        /// Absolute path to the metadata file
        /// </summary>
        public string MetaDataPath => _metaDataPath;

        /// <summary>
        /// Direct parent directory name of the asset file
        /// </summary>
        public string SubDirName => _subDirName;

        private readonly FileInfo _file;
        private readonly string _metaDataPath;
        private readonly string _subDirName;

        public AssetMetaPath(FileInfo file, string metaDataPath = null)
        {
            _file = file;
            _metaDataPath = metaDataPath ?? Path + ".meta";
            RelativePath = RelativePath.FromAbsolutePath(Path);
            RelativeMetaDataPath = RelativePath.FromAbsolutePath(_metaDataPath);
            _subDirName = PathTools.GetSubDir(RelativePath);
        }
    }
}