using System.Collections.Generic;
using System.Linq;
using SaberFactory.UI.Flow;

namespace SaberFactory.UI
{
    internal class ListAssetDirectoryManager
    {
        private const string UpDirIndicator = "<";
        public string DirectoryString { get; private set; } = "";

        public bool IsInRoot => string.IsNullOrEmpty(_currentDirectory);
        private readonly List<string> _additionalFolderPool;

        private string _currentDirectory = "";

        public ListAssetDirectoryManager(List<string> additionalFolderPool)
        {
            _additionalFolderPool = additionalFolderPool;
        }

        public void GoBack()
        {
            if (!_currentDirectory.Contains("\\"))
            {
                _currentDirectory = "";
            }
            else
            {
                _currentDirectory = _currentDirectory.Substring(0, _currentDirectory.LastIndexOf('\\'));
            }
        }

        public void Navigate(string path)
        {
            if (path == UpDirIndicator)
            {
                GoBack();
                return;
            }

            _currentDirectory += (IsInRoot ? "" : "\\") + path;

            RefreshDirectoryString();
        }

        public (HashSet<IFolderInfo> folders, List<IAssetInfo> assets) Process(IEnumerable<IAssetInfo> items)
        {
            var itemsList = FilterForDir(items, _currentDirectory).ToList();

            var addedFolders = new HashSet<IFolderInfo>();

            if (!IsInRoot)
            {
                addedFolders.Add(new GenericFolder(UpDirIndicator));
            }

            foreach (var folder in _additionalFolderPool)
            {
                if (!folder.StartsWith(_currentDirectory))
                {
                    continue;
                }

                var d = _currentDirectory == string.Empty ? folder : folder.Replace(_currentDirectory, "");
                if (d.Length > 0 && d[0] == '\\')
                {
                    d = d.Substring(1);
                }

                d = d.Contains('\\') ? d.Substring(0, d.IndexOf('\\')) : d;
                if (d != string.Empty)
                {
                    addedFolders.Add(new GenericFolder(d));
                }
            }

            return (addedFolders, itemsList);
        }

        public IEnumerable<IAssetInfo> FilterForDir(IEnumerable<IAssetInfo> items, string dir)
        {
            foreach (var item in items)
            {
                if (item.SubDir == dir)
                {
                    yield return item;
                }
            }
        }

        private void RefreshDirectoryString()
        {
            DirectoryString = _currentDirectory;
        }
        
        public class GenericFolder : IFolderInfo
        {
            public GenericFolder(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }
    }
}