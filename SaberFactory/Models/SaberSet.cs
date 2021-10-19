using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SaberFactory.Configuration;
using SaberFactory.DataStore;
using SaberFactory.Helpers;
using SaberFactory.Saving;
using Zenject;

namespace SaberFactory.Models
{
    /// <summary>
    ///     Stores currently used left and right saber model implementation
    /// </summary>
    internal class SaberSet : IFactorySerializable, ILoadingTask
    {
        private static readonly Random RNG = new Random();

        public SaberModel LeftSaber { get; set; }

        public SaberModel RightSaber { get; set; }

        public bool IsEmpty => LeftSaber.IsEmpty && RightSaber.IsEmpty;
        private readonly PluginConfig _config;

        private readonly List<int> _lastSelectedRandoms = new List<int> { 1 };
        private readonly MainAssetStore _mainAssetStore;

        private readonly PresetSaveManager _presetSaveManager;

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

        public async Task FromJson(JObject obj, Serializer serializer)
        {
            await LeftSaber.FromJson((JObject)obj[nameof(LeftSaber)], serializer);
            await RightSaber.FromJson((JObject)obj[nameof(RightSaber)], serializer);
        }

        public async Task<JToken> ToJson(Serializer serializer)
        {
            var obj = new JObject
            {
                { nameof(LeftSaber), await LeftSaber.ToJson(serializer) },
                { nameof(RightSaber), await RightSaber.ToJson(serializer) }
            };
            return obj;
        }

        public Task CurrentTask { get; private set; }

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
                await RandomizeFrom(_mainAssetStore.GetAllMetaData(AssetTypeDefinition.CustomSaber).ToList());
            }
        }

        public async Task RandomizeFrom(IList<PreloadMetaData> meta)
        {
            if (_lastSelectedRandoms.Count == meta.Count)
            {
                _lastSelectedRandoms.Clear();
            }

            int idx;
            do
            {
                idx = RandomNumber(meta.Count);
            } while (_lastSelectedRandoms.Contains(idx));

            _lastSelectedRandoms.Add(idx);

            var comp = meta[idx];
            SetModelComposition(await _mainAssetStore.GetCompositionByMeta(comp));
        }

        public async Task SetSaber(string saberName)
        {
            var metaData = _mainAssetStore.GetAllMetaData(AssetTypeDefinition.CustomSaber);
            var saber = metaData.FirstOrDefault(x => x.ListName == saberName);
            await SetSaber(saber);
        }

        public async Task SetSaber(PreloadMetaData preloadData)
        {
            if (preloadData == null)
            {
                return;
            }

            SetModelComposition(await _mainAssetStore.GetCompositionByMeta(preloadData));
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
            CurrentTask = _presetSaveManager.LoadSaber(this, fileName);
            await CurrentTask;
            CurrentTask = null;
        }

        public void Sync(SaberModel fromModel)
        {
            fromModel.Sync();
            var otherSaber = fromModel == LeftSaber ? RightSaber : LeftSaber;
            otherSaber.SaberWidth = fromModel.SaberWidth;
        }

        private int RandomNumber(int count)
        {
            lock (RNG)
            {
                return RNG.Next(count);
            }
        }
    }
}