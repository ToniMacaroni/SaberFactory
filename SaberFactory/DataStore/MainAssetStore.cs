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
using SiraUtil.Tools;

namespace SaberFactory.DataStore
{
    /// <summary>
    ///     Class for managing store assets ie. parts and custom sabers
    /// </summary>
    internal class MainAssetStore : IDisposable, ILoadingTask
    {
        public List<string> AdditionalCustomSaberFolders { get; } = new List<string>();

        public Task<ModelComposition> this[string path] => GetCompositionByPath(path);

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

        public async Task<ModelComposition> GetCompositionByPath(string relativePath)
        {
            if (_modelCompositions.TryGetValue(relativePath, out var result))
            {
                return result;
            }

            return await LoadComposition(relativePath);
        }

        public async Task<ModelComposition> GetCompositionByMeta(PreloadMetaData meta)
        {
            return await this[PathTools.ToRelativePath(meta.AssetMetaPath.Path)];
        }

        public async Task LoadAllMetaAsync(EAssetTypeConfiguration assetType)
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

        public IEnumerable<ModelComposition> GetAllCompositions()
        {
            return _modelCompositions.Values;
        }

        public PreloadMetaData GetMetaDataForComposition(ModelComposition comp)
        {
            var path = comp.GetLeft().StoreAsset.RelativePath + ".meta";
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

        public void Unload(string path)
        {
            if (!_modelCompositions.TryGetValue(path, out var comp))
            {
                return;
            }

            comp.Dispose();
            _modelCompositions.Remove(path);
            _metaData.Remove(path + ".meta");
        }

        public async Task Reload(string path)
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

        public void Delete(string path)
        {
            if (_metaData.TryGetValue(path + ".meta", out var meta) && meta.AssetMetaPath.HasMetaData)
            {
                File.Delete(meta.AssetMetaPath.MetaDataPath);
            }

            Unload(path);
            var filePath = PathTools.ToFullPath(path);
            File.Delete(filePath);
        }

        private async Task LoadAllMetaDataForLoader(AssetBundleLoader loader, bool createIfNotExisting = false)
        {
            var sw = DebugTimer.StartNew("Loading Metadata");

            foreach (var assetMetaPath in loader.CollectFiles(_pluginDirs))
            {
                if (_metaData.TryGetValue(assetMetaPath.RelativePath+".meta", out _))
                {
                    continue;
                }

                if (!assetMetaPath.HasMetaData)
                {
                    if (createIfNotExisting)
                    {
                        var comp = await this[PathTools.ToRelativePath(assetMetaPath.Path)];

                        if (comp == null)
                        {
                            continue;
                        }

                        var metaData = new PreloadMetaData(assetMetaPath, comp, comp.AssetTypeDefinition);
                        metaData.SaveToFile();
                        _metaData.Add(assetMetaPath.RelativePath+".meta", metaData);
                    }
                }
                else
                {
                    var metaData = new PreloadMetaData(assetMetaPath);
                    metaData.LoadFromFile();
                    metaData.IsFavorite = _config.IsFavorite(PathTools.ToRelativePath(assetMetaPath.Path));
                    _metaData.Add(assetMetaPath.RelativePath+".meta", metaData);
                }
            }

            sw.Print(_logger);
        }

        public async Task<ModelComposition> CreateMetaData(AssetMetaPath assetMetaPath)
        {
            var relativePath = assetMetaPath.RelativePath+".meta";
            if (_metaData.TryGetValue(relativePath, out _))
            {
                return null;
            }

            var comp = await this[PathTools.ToRelativePath(assetMetaPath.Path)];

            if (comp == null)
            {
                return null;
            }

            var metaData = new PreloadMetaData(assetMetaPath, comp, comp.AssetTypeDefinition);
            metaData.SaveToFile();
            _metaData.Add(relativePath, metaData);

            return comp;
        }

        private void LoadMetaData(string pieceRelativePath)
        {
            var assetMetaPath = new AssetMetaPath(new FileInfo(PathTools.ToFullPath(pieceRelativePath)), _pluginDirs.Cache.GetFile(Path.GetFileName(pieceRelativePath)+".meta").FullName);
            if (_metaData.TryGetValue(assetMetaPath.RelativePath+".meta", out _))
            {
                return;
            }

            if (!File.Exists(assetMetaPath.MetaDataPath))
            {
                return;
            }

            var metaData = new PreloadMetaData(assetMetaPath);
            metaData.IsFavorite = _config.IsFavorite(assetMetaPath.RelativePath);
            metaData.LoadFromFile();
            _metaData.Add(assetMetaPath.RelativePath+".meta", metaData);
        }

        private void AddModelComposition(string key, ModelComposition modelComposition)
        {
            if (!_modelCompositions.ContainsKey(key))
            {
                _modelCompositions.Add(key, modelComposition);
            }
        }

        private async Task<ModelComposition> LoadModelCompositionAsync(string relativeBundlePath)
        {
            // TODO: Switch between customsaber and part implementation

            AssetBundleLoader loader = _customSaberAssetLoader;
            IStoreAssetParser modelCreator = _customSaberModelLoader;

            var storeAsset = await loader.LoadStoreAssetAsync(relativeBundlePath);
            if (storeAsset == null)
            {
                return null;
            }

            var model = modelCreator.GetComposition(storeAsset);

            return model;
        }

        private async Task<ModelComposition> LoadComposition(string relativePath)
        {
            var composition = await LoadModelCompositionAsync(relativePath);
            if (composition != null)
            {
                _modelCompositions.Add(relativePath, composition);
            }

            return composition;
        }
    }
}