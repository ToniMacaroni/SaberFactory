using System.Collections.Generic;
using System.Linq;
using SaberFactory.Configuration;
using SaberFactory.DataStore;
using SaberFactory.Models;
using SaberFactory.UI.Flow;
using UnityEngine;

namespace SaberFactory.UI.Lib;

public class SaberListController
{
    private readonly MainAssetStore _mainAssetStore;
    private readonly PluginConfig _pluginConfig;
    private readonly List<RemoteLocationPart> _remoteParts;
    private readonly ListAssetDirectoryManager _dirManager;

    public enum ESortMode
    {
        Name,
        Date,
        Size,
        Author
    }

    public ESortMode SortMode { get; set; } = ESortMode.Name;

    internal SaberListController(
        MainAssetStore mainAssetStore,
        PluginConfig pluginConfig,
        List<RemoteLocationPart> remoteParts)
    {
        _mainAssetStore = mainAssetStore;
        _pluginConfig = pluginConfig;
        _remoteParts = remoteParts;
        _dirManager = new ListAssetDirectoryManager(_mainAssetStore.AdditionalCustomSaberFolders);
    }

    public SaberCollectionResult GetSabers()
    {
        var result = new SaberCollectionResult();
        
        // Get all metadata and sort by favorite
        var metaEnumerable = from meta in _mainAssetStore.GetAllMetaData()
            orderby meta.IsFavorite descending
            select meta;

        // Sort everything else by the selected sort mode
        switch (SortMode)
        {
            case ESortMode.Name:
                metaEnumerable = metaEnumerable.ThenBy(x => x.Name);
                break;
            case ESortMode.Date:
                metaEnumerable = metaEnumerable.ThenByDescending(x => x.AssetMetaPath.File.LastWriteTime);
                break;
            case ESortMode.Size:
                metaEnumerable = metaEnumerable.ThenByDescending(x => x.AssetMetaPath.File.Length);
                break;
            case ESortMode.Author:
                metaEnumerable = metaEnumerable.ThenBy(x => x.Author);
                break;
        }

        var items = new List<IAssetInfo>(metaEnumerable);
        var loadedNames = items.Select(x => x.Name).ToList();

        var addedDownloadables = 0;

        // Show downloadable sabers
        if (_pluginConfig.ShowDownloadableSabers)
        {
            var idx = items.Count(x => x.IsFavorite);

            // if the saber isn't aleady present
            // add the downloadable option
            foreach (var remotePart in _remoteParts)
            {
                if (!loadedNames.Contains(remotePart.Name))
                {
                    items.Insert(idx, remotePart);
                    addedDownloadables++;
                }
            }
        }

        result.NoUserSabers = items.Count <= addedDownloadables;

        // Fill the saber list with the currently selected directory
        var (folders, sabers) = _dirManager.Process(items);

        result.Sabers = sabers;
        result.Folders = folders;

        return result;
    }

    public void ChangeDirectory(string dirName)
    {
        _dirManager.Navigate(dirName);
    }

    public struct SaberCollectionResult
    {
        public List<IAssetInfo> Sabers;
        public HashSet<IFolderInfo> Folders;
        public bool NoUserSabers;
    }
}