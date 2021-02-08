using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaberFactory.Configuration;
using SaberFactory.DataStore;
using SaberFactory.Helpers;
using SaberFactory.Saving;
using Zenject;
using Random = UnityEngine.Random;

namespace SaberFactory.Models
{
    /// <summary>
    /// Stores currently used left and right saber model implementation
    /// </summary>
    internal class SaberSet
    {
        public SaberModel LeftSaber { get; set; }
        public SaberModel RightSaber { get; set; }
        public Task CurrentLoadingTask { get; private set; }

        private readonly PresetSaveManager _presetSaveManager;
        private readonly PluginConfig _config;
        private readonly MainAssetStore _mainAssetStore;

        private List<int> _lastSelectedRandoms = new List<int>{1};

        private SaberSet(
            [Inject(Id = ESaberSlot.Left)] SaberModel leftSaber,
            [Inject(Id = ESaberSlot.Right)] SaberModel rightSaber,
            PresetSaveManager presetSaveManager,
            PluginConfig config,
            MainAssetStore mainAssetStore)
        {
            _presetSaveManager = presetSaveManager;
            _config = config;
            _mainAssetStore = mainAssetStore;
            LeftSaber = leftSaber;
            RightSaber = rightSaber;

            Load();
        }

        public void SetModelComposition(ModelComposition modelComposition)
        {
            LeftSaber.SetModelComposition(modelComposition);
            RightSaber.SetModelComposition(modelComposition);
        }

        public async Task Randomize()
        {
            if (
                _config.AssetType == EAssetTypeConfiguration.CustomSaber ||
                _config.AssetType == EAssetTypeConfiguration.None)
            {
                await RandomizeFrom(_mainAssetStore.GetAllMetaData(AssetTypeDefinition.CustomSaber));
            }
        }

        public async Task RandomizeFrom(IEnumerable<PreloadMetaData> meta)
        {
            var list = meta.ToList();
            Console.WriteLine(list.Count);

            if(_lastSelectedRandoms.Count == list.Count) _lastSelectedRandoms.Clear();

            var idx = 1;
            while (_lastSelectedRandoms.Contains(idx))
            {
                idx = (int)(Random.Range(0f, 1f) * list.Count);
            }

            _lastSelectedRandoms.Add(idx);

            var comp = list[idx];
            SetModelComposition(await _mainAssetStore[PathTools.ToRelativePath(comp.AssetMetaPath.Path)]);
        }

        public async void Load()
        {
            await Load("default");
        }

        public void Save()
        {
            Save("default");
        }

        public void Save(string fileName)
        {
            _presetSaveManager.SaveSaber(this, fileName);
        }

        public async Task Load(string fileName)
        {
            CurrentLoadingTask = _presetSaveManager.LoadSaber(this, fileName);
            await CurrentLoadingTask;
        }

        public void Sync(SaberModel fromModel)
        {
            fromModel.Sync();
            var otherSaber = fromModel == LeftSaber ? RightSaber : LeftSaber;
            otherSaber.SaberWidth = fromModel.SaberWidth;
        }

        public bool IsEmpty => LeftSaber.IsEmpty && RightSaber.IsEmpty;
    }
}