using System.Collections.Generic;
using System.Linq;
using SaberFactory.Models;
using SaberFactory.UI.CustomSaber.CustomComponents;
using SaberFactory.UI.Lib;

namespace SaberFactory.UI
{
    internal class ListItemDirectoryManager
    {
        private const string UpDirIndicator = "<";
        public string DirectoryString { get; private set; } = "";

        public bool IsInRoot => string.IsNullOrEmpty(_currentDirectory);
        private readonly List<string> _additionalFolderPool;

        private string _currentDirectory = "";

        public ListItemDirectoryManager(List<string> additionalFolderPool)
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

        public List<ICustomListItem> Process(IEnumerable<ICustomListItem> items)
        {
            var itemsList = FilterForDir(items, _currentDirectory).ToList();

            var addedFolders = new HashSet<string>();

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
                    addedFolders.Add(d);
                }
            }

            itemsList.InsertRange(0, addedFolders.Select(x => new CustomList.ListDirectory(x)));

            if (!IsInRoot)
            {
                itemsList.Insert(0, new CustomList.ListDirectory(UpDirIndicator));
            }

            return itemsList;
        }

        public IEnumerable<ICustomListItem> FilterForDir(IEnumerable<ICustomListItem> items, string dir)
        {
            foreach (var item in items)
            {
                if (item is PreloadMetaData preloadMetaData)
                {
                    if (preloadMetaData.AssetMetaPath.SubDirName == dir)
                    {
                        yield return item;
                    }
                }
                else if (item is ModelComposition comp)
                {
                    if (comp.GetLeft().StoreAsset.SubDirName == dir)
                    {
                        yield return item;
                    }
                }
                else
                {
                    yield return item;
                }
            }
        }

        private void RefreshDirectoryString()
        {
            DirectoryString = _currentDirectory;
        }
    }
}