using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IPA.Utilities.Async;
using SaberFactory.Configuration;
using SaberFactory.Helpers;
using SaberFactory.Loaders;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SiraUtil.Tools;

namespace SaberFactory.DataStore
{
    /// <summary>
    /// Class for managing store assets ie. parts and custom sabers
    /// </summary>
    internal class MainAssetStore : IDisposable
    {
        private static readonly int MAX_LOADING_THREADS = 15;

        public bool IsLoading { get; private set; }
        public Task CurrentTask;

        private readonly CustomSaberAssetLoader _customSaberAssetLoader;
        private readonly CustomSaberModelLoader _customSaberModelLoader;

        private readonly PluginConfig _config;
        private readonly SiraLog _logger;

        private readonly Dictionary<string, ModelComposition> _modelCompositions;
        private readonly Dictionary<string, PreloadMetaData> _metaData;

        private MainAssetStore(
            PluginConfig config,
            SiraLog logger,
            CustomSaberModelLoader customSaberModelLoader)
        {
            _config = config;
            _logger = logger;

            _customSaberAssetLoader = new CustomSaberAssetLoader();
            _customSaberModelLoader = customSaberModelLoader;

            _modelCompositions = new Dictionary<string, ModelComposition>();
            _metaData = new Dictionary<string, PreloadMetaData>();
        }

        public Task<ModelComposition> this[string path] => GetCompositionByPath(path);

        public async Task<ModelComposition> GetCompositionByPath(string path)
        {
            if (_modelCompositions.TryGetValue(path, out var result)) return result;

            return await LoadComposition(path);
        }

        public void Dispose()
        {
            UnloadAll();
        }

        public async Task LoadAllAsync(EAssetTypeConfiguration assetType)
        {
            await LoadAllCustomSabersAsync();
            //if (assetType == EAssetType.SaberFactory)
            //{
            //    await LoadAllCustomSabersAsync();
            //}else if (assetType == EAssetType.CustomSaber)
            //{
            //    // Part Loading
            //}
        }

        public async Task LoadAllMetaAsync(EAssetTypeConfiguration assetType)
        {
            await LoadAllCustomSaberMetaDataAsync();
        }

        public async Task LoadAllCustomSabersAsync()
        {
            if (!IsLoading)
            {
                IsLoading = true;
                CurrentTask = LoadAllCustomSabersAsyncInternal(_config.LoadingThreads);
            }

            await CurrentTask;
            IsLoading = false;
        }

        public async Task LoadAllCustomSaberMetaDataAsync()
        {
            if (!IsLoading)
            {
                IsLoading = true;
                CurrentTask = LoadAllCustomSaberMetaDataAsyncInternal();
            }

            await CurrentTask;
            IsLoading = false;
        }

        public IEnumerable<ModelComposition> GetAllModelCompositions()
        {
            return _modelCompositions.Values;
        }

        public IEnumerable<ModelComposition> GetAllModelCompositions(AssetTypeDefinition assetType)
        {
            return _modelCompositions.Values.Where(x => x.AssetTypeDefinition.Equals(assetType));
        }

        public IEnumerable<PreloadMetaData> GetAllMetaData()
        {
            return _metaData.Values;
        }

        public PreloadMetaData GetMetaDataForComposition(ModelComposition comp)
        {
            var path = comp.GetLeft().StoreAsset.Path + ".meta";
            if (_metaData.TryGetValue(path, out var preloadMetaData)) return preloadMetaData;
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
            if (!_modelCompositions.TryGetValue(path, out var comp)) return;
            comp.Dispose();
            _modelCompositions.Remove(path);
            _metaData.Remove(path+".meta");
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

        private async Task LoadAllCustomSabersAsyncInternal(int threads)
        {
            threads = Math.Min(threads, MAX_LOADING_THREADS);

            var tasks = new List<Task>();
            var files = new ConcurrentQueue<string>();

            foreach (var assetMetaPath in await _customSaberAssetLoader.CollectFiles())
            {
                files.Enqueue(assetMetaPath.Path);
            }

            _logger.Info($"{files.Count} custom sabers found");
            if (files.Count == 0) return;

            var sw = Stopwatch.StartNew();

            async Task LoadingThread()
            {
                while (files.TryDequeue(out string file))
                {
                    var relativePath = PathTools.ToRelativePath(file);
                    if (_modelCompositions.ContainsKey(relativePath)) continue;

                    await LoadComposition(relativePath);
                }
            }

            for (int i = 0; i < threads; i++)
            {
                tasks.Add(LoadingThread());
            }

            await Task.WhenAll(tasks);

            sw.Stop();
            _logger.Info($"Loaded in {sw.Elapsed.Seconds}.{sw.Elapsed.Milliseconds}s");
        }

        private async Task LoadAllCustomSaberMetaDataAsyncInternal()
        {
            await LoadAllMetaDataForLoader(_customSaberAssetLoader, true);
        }

        private async Task LoadAllMetaDataForLoader(AssetBundleLoader loader, bool createIfNotExisting = false)
        {
            var sw = Stopwatch.StartNew();

            await Task.Run(async () =>
            {
                foreach (var assetMetaPath in await _customSaberAssetLoader.CollectFiles())
                {
                    var relativePath = PathTools.ToRelativePath(assetMetaPath.MetaDataPath);
                    if (_metaData.TryGetValue(relativePath, out _)) continue;

                    if (!assetMetaPath.HasMetaData)
                    {
                        if (createIfNotExisting)
                        {
                            var comp = await await UnityMainThreadTaskScheduler.Factory.StartNew(() => this[PathTools.ToRelativePath(assetMetaPath.Path)]);

                            if (comp == null) continue;

                            var metaData = new PreloadMetaData(assetMetaPath, comp, comp.AssetTypeDefinition);
                            await metaData.SaveToFile();
                            _metaData.Add(relativePath, metaData);
                        }
                    }
                    else
                    {
                        var metaData = new PreloadMetaData(assetMetaPath);
                        await metaData.LoadFromFile();
                        metaData.IsFavorite = _config.IsFavorite(PathTools.ToRelativePath(assetMetaPath.Path));
                        _metaData.Add(relativePath, metaData);
                    }
                }
            });

            sw.Stop();
            _logger.Info($"Loaded Metadata in {sw.Elapsed.Seconds}.{sw.Elapsed.Milliseconds}s");
        }

        private PreloadMetaData LoadMetaData(string pieceRelativePath)
        {
            var assetMetaPath = new AssetMetaPath(PathTools.ToFullPath(pieceRelativePath));
            if (_metaData.TryGetValue(assetMetaPath.RelativeMetaDataPath, out var preloadMetaData)) return preloadMetaData;
            if (!File.Exists(assetMetaPath.MetaDataPath)) return null;

            var metaData = new PreloadMetaData(assetMetaPath);
            metaData.IsFavorite = _config.IsFavorite(assetMetaPath.RelativePath);
            _metaData.Add(assetMetaPath.RelativeMetaDataPath, metaData);
            return metaData;
        }

        private void AddModelComposition(string key, ModelComposition modelComposition)
        {
            if(!_modelCompositions.ContainsKey(key)) _modelCompositions.Add(key, modelComposition);
        }

        private async Task<ModelComposition> LoadModelCompositionAsync(string bundlePath)
        {
            // TODO: Switch between customsaber and part implementation

            AssetBundleLoader loader = _customSaberAssetLoader;
            IStoreAssetParser modelCreator = _customSaberModelLoader;

            var storeAsset = await loader.LoadStoreAssetAsync(bundlePath);
            if (storeAsset == null) return null;
            var model = modelCreator.GetComposition(storeAsset);

            return model;
        }

        private async Task<ModelComposition> LoadComposition(string path)
        {
            var composition = await LoadModelCompositionAsync(path);
            if(composition!=null) _modelCompositions.Add(path, composition);
            return composition;
        }
    }
}