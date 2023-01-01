using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SaberFactory.Configuration;
using SaberFactory.Helpers;
using SaberFactory.Loaders;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SaberFactory.UI.Flow;
using SiraUtil.Logging;
using SiraUtil.Tools;
using UnityEngine;

namespace SaberFactory.DataStore
{
    /// <summary>
    ///     Class for managing store assets ie. parts and custom sabers
    /// </summary>
    public class MainAssetStore : IDisposable, ILoadingTask
    {
        public List<string> AdditionalCustomSaberFolders { get; } = new List<string>();

        public Task<ModelComposition> this[RelativePath path] => GetCompositionByPath(path);

        public Task<ModelComposition> this[PreloadMetaData metaData] => GetCompositionByPath(metaData.AssetMetaPath.RelativePath);

        private readonly PluginConfig _config;

        private readonly CustomSaberAssetLoader _customSaberAssetLoader;
        private readonly CustomSaberModelLoader _customSaberModelLoader;
        private readonly SiraLog _logger;
        private readonly Dictionary<string, PreloadMetaData> _metaData;

        private readonly Dictionary<string, ModelComposition> _modelCompositions;
        private readonly PluginDirectories _pluginDirs;

        private MainAssetStore(
            PluginConfig config,
            SiraLog logger,
            CustomSaberModelLoader customSaberModelLoader,
            PluginDirectories pluginDirs)
        {
            _config = config;
            _logger = logger;
            _pluginDirs = pluginDirs;

            _customSaberAssetLoader = new CustomSaberAssetLoader();
            _customSaberModelLoader = customSaberModelLoader;

            _modelCompositions = new Dictionary<string, ModelComposition>();
            _metaData = new Dictionary<string, PreloadMetaData>();

            foreach (var directory in pluginDirs.CustomSaberDir.GetDirectories("*", SearchOption.AllDirectories))
            {
                var relPath = PathTools.ToRelativePath(directory.FullName);
                relPath = PathTools.CorrectRelativePath(relPath);
                relPath = relPath.Substring(relPath.IndexOf('\\') + 1);
                AdditionalCustomSaberFolders.Add(relPath);
            }
        }

        public void Dispose()
        {
            UnloadAll();
        }

        public Task CurrentTask { get; private set; }

        public async Task<ModelComposition> GetCompositionByPath(RelativePath relativePath)
        {
            if (_modelCompositions.TryGetValue(relativePath.Path, out var result))
            {
                return result;
            }

            return await LoadComposition(relativePath);
        }

        public async Task<ModelComposition> GetCompositionByMeta(PreloadMetaData meta)
        {
            return await this[meta.AssetMetaPath.RelativePath];
        }

        internal async Task LoadAllMetaAsync(EAssetTypeConfiguration assetType)
        {
            await LoadAllCustomSaberMetaDataAsync();
        }

        public async Task LoadAllCustomSaberMetaDataAsync()
        {
            if (CurrentTask == null)
            {
                CurrentTask = LoadAllMetaDataForLoader(_customSaberAssetLoader, true);
            }

            await CurrentTask;
            CurrentTask = null;
        }

        public IEnumerable<PreloadMetaData> GetAllMetaData()
        {
            return _metaData.Values;
        }

        public PreloadMetaData FindByAssetInfo(IAssetInfo asset)
        {
            return _metaData.Values.FirstOrDefault(x => x.Name == asset.Name && x.Author == asset.Author);
        }

        public IEnumerable<ModelComposition> GetAllCompositions()
        {
            return _modelCompositions.Values;
        }

        public PreloadMetaData GetMetaDataForComposition(ModelComposition comp)
        {
            var path = comp.GetLeft().StoreAsset.RelativePath.Path + ".meta";
            if (_metaData.TryGetValue(path, out var preloadMetaData))
            {
                return preloadMetaData;
            }

            return null;
        }

        public IEnumerable<PreloadMetaData> GetAllMetaData(AssetTypeDefinition assetType)
        {
            return _metaData.Values.Where(x => x.AssetTypeDefinition.Equals(assetType));
        }

        public void UnloadAll()
        {
            foreach (var modelCompositions in _modelCompositions.Values)
            {
                modelCompositions.Dispose();
            }

            _modelCompositions.Clear();
            _metaData.Clear();
        }

        public void Unload(RelativePath path)
        {
            if (!_modelCompositions.TryGetValue(path.Path, out var comp))
            {
                return;
            }

            comp.Dispose();
            _modelCompositions.Remove(path.Path);
            _metaData.Remove(path.Path + ".meta");
        }

        public async Task Reload(RelativePath path)
        {
            Unload(path);
            LoadMetaData(path);
            await LoadComposition(path);
        }

        public async Task ReloadAll()
        {
            UnloadAll();
            await LoadAllCustomSaberMetaDataAsync();
        }

        public void Delete(RelativePath path)
        {
            if (_metaData.TryGetValue(path.Path + ".meta", out var meta) && meta.AssetMetaPath.HasMetaData)
            {
                File.Delete(meta.AssetMetaPath.MetaDataPath);
            }

            Unload(path);
            var filePath = path.ToAbsolutePath();
            File.Delete(filePath);
        }

        private async Task LoadAllMetaDataForLoader(AssetBundleLoader loader, bool createIfNotExisting = false)
        {
            var sw = DebugTimer.StartNew("Loading Metadata");

            foreach (var assetMetaPath in loader.CollectFiles(_pluginDirs))
            {
                if (_metaData.TryGetValue(assetMetaPath.RelativePath.Path+".meta", out _))
                {
                    continue;
                }

                if (!assetMetaPath.HasMetaData)
                {
                    if (createIfNotExisting)
                    {
                        var comp = await this[assetMetaPath.RelativePath];

                        if (comp == null)
                        {
                            continue;
                        }

                        var metaData = new PreloadMetaData(assetMetaPath, comp, comp.AssetTypeDefinition);
                        metaData.SaveToFile();
                        _metaData.Add(assetMetaPath.RelativePath.Path+".meta", metaData);
                    }
                }
                else
                {
                    var metaData = new PreloadMetaData(assetMetaPath);
                    metaData.LoadFromFile();
                    metaData.IsFavorite = _config.IsFavorite(PathTools.ToRelativePath(assetMetaPath.Path));
                    _metaData.Add(assetMetaPath.RelativePath.Path+".meta", metaData);
                }
            }

            sw.Print(_logger);
        }

        internal async Task<ModelComposition> CreateMetaData(AssetMetaPath assetMetaPath)
        {
            var relativePath = assetMetaPath.RelativeMetaDataPath.Path;
            if (_metaData.TryGetValue(relativePath, out _))
            {
                return null;
            }

            var comp = await this[assetMetaPath.RelativePath];

            if (comp == null)
            {
                return null;
            }

            var metaData = new PreloadMetaData(assetMetaPath, comp, comp.AssetTypeDefinition);
            metaData.SaveToFile();
            _metaData.Add(relativePath, metaData);

            return comp;
        }

        private void LoadMetaData(RelativePath pieceRelativePath)
        {
            var assetMetaPath = new AssetMetaPath(new FileInfo(pieceRelativePath.ToAbsolutePath()), _pluginDirs.Cache.GetFile(Path.GetFileName(pieceRelativePath.Path)+".meta").FullName);
            if (_metaData.TryGetValue(assetMetaPath.RelativePath.Path+".meta", out _))
            {
                return;
            }

            if (!File.Exists(assetMetaPath.MetaDataPath))
            {
                return;
            }

            var metaData = new PreloadMetaData(assetMetaPath);
            metaData.IsFavorite = _config.IsFavorite(assetMetaPath.RelativePath.Path);
            metaData.LoadFromFile();
            _metaData.Add(assetMetaPath.RelativePath.Path+".meta", metaData);
        }

        private (AssetBundleLoader loader, IStoreAssetParser creator) GetLoaderAndCreatorForCurrentSystem()
        {
            //TODO: Switch for implementation
            return (_customSaberAssetLoader, _customSaberModelLoader);
        }

        private async Task<ModelComposition> LoadModelCompositionFromFileAsync(RelativePath relativeBundlePath)
        {
            var (loader, modelCreator) = GetLoaderAndCreatorForCurrentSystem();

            var storeAsset = await loader.LoadStoreAssetAsync(relativeBundlePath);
            if (storeAsset == null)
            {
                return null;
            }

            var model = modelCreator.GetComposition(storeAsset);

            return model;
        }

        private async Task<ModelComposition> LoadModelCompositionFromBundleAsync(AssetBundle bundle, string saberName)
        {
            if (string.IsNullOrWhiteSpace(saberName))
            {
                _logger.Warn("SaberName needs to be unique and non-empty");
                return null;
            }

            var (loader, modelCreator) = GetLoaderAndCreatorForCurrentSystem();

            var storeAsset = await loader.LoadStoreAssetFromBundleAsync(bundle, saberName);
            if (storeAsset == null)
            {
                return null;
            }

            var model = modelCreator.GetComposition(storeAsset);

            return model;
        }

        private async Task<ModelComposition> LoadComposition(RelativePath relativePath)
        {
            var composition = await LoadModelCompositionFromFileAsync(relativePath);
            if (composition != null)
            {
                _modelCompositions.Add(relativePath.Path, composition);
            }

            return composition;
        }
    }
}