using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SaberFactory.Helpers;
using SaberFactory.Loaders;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SiraUtil.Tools;

namespace SaberFactory.DataStore
{
    internal class MainAssetStore : IDisposable
    {
        public event Action OnLoadingFinished;
        public bool IsLoading { get; private set; }
        public Task CurrentTask;

        private readonly CustomSaberAssetLoader _customSaberAssetLoader;
        private readonly CustomSaberModelLoader _customSaberModelLoader;

        private readonly SiraLog _logger;

        private readonly Dictionary<string, ModelComposition> _modelCompositions;

        private MainAssetStore(
            SiraLog logger,
            CustomSaberModelLoader customSaberModelLoader)
        {
            _logger = logger;

            _customSaberAssetLoader = new CustomSaberAssetLoader();
            _customSaberModelLoader = customSaberModelLoader;

            _modelCompositions = new Dictionary<string, ModelComposition>();
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

        public async Task LoadAllCustomSabersAsync(bool fireEvent)
        {
            var sw = Stopwatch.StartNew();
            foreach (var path in _customSaberAssetLoader.CollectFiles())
            {
                var relativePath = PathTools.ToRelativePath(path);
                if(_modelCompositions.ContainsKey(relativePath)) continue;

                await LoadComposition(relativePath);
            }
            sw.Stop();
            _logger.Info($"Loaded in {sw.Elapsed.Seconds} Seconds");

            if (fireEvent) NotifyLoadingFinished();
        }

        public async Task LoadAll()
        {
            if (!IsLoading)
            {
                IsLoading = true;
                CurrentTask = LoadAllCustomSabersAsync(false);
            }

            await CurrentTask;
            NotifyLoadingFinished();
        }

        public List<ModelComposition> GetAllModelCompositions()
        {
            return _modelCompositions.Values.ToList();
        }

        public void UnloadAll()
        {
            foreach (var modelCompositions in _modelCompositions.Values)
            {
                modelCompositions.Dispose();
            }
            _modelCompositions.Clear();
        }

        public void Unload(string path)
        {
            if (!_modelCompositions.TryGetValue(path, out var comp)) return;
            comp.Dispose();
            _modelCompositions.Remove(path);
        }

        public async Task Reload(string path)
        {
            Unload(path);
            await LoadComposition(path);
        }

        public async Task ReloadAll()
        {
            UnloadAll();
            await LoadAll();
        }

        public void Delete(string path)
        {
            Unload(path);
            var filePath = PathTools.ToFullPath(path);
            File.Delete(filePath);
        }

        private void NotifyLoadingFinished()
        {
            IsLoading = false;
            OnLoadingFinished?.Invoke();
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