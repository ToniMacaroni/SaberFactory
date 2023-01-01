using System;
using System.Linq;
using System.Threading.Tasks;
using IPA.Utilities;
using Newtonsoft.Json.Linq;
using SaberFactory.DataStore;
using SaberFactory.Helpers;
using SaberFactory.Serialization;
using SaberFactory.UI.Flow;
using SiraUtil.Logging;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace SaberFactory.Models
{
    /// <summary>
    ///     Stores currently used left and right saber model implementation
    /// </summary>
    public class SaberSet : IFactorySerializable, ILoadingTask
    {
        [Inject] private readonly SiraLog _logger = null;

        public SaberModel LeftSaber { get; set; }

        public SaberModel RightSaber { get; set; }
        
        public PresetInfo LoadedPreset { get; set; }

        public bool IsEmpty => LeftSaber.IsEmpty && RightSaber.IsEmpty;

        private readonly MainAssetStore _mainAssetStore;
        private readonly PresetSaveManager _presetSaveManager;

        /// <summary>
        /// Called when new sabers are loaded into the set
        /// </summary>
        public event Action OnSaberSetChanged;
        
        private SaberSet(
            [Inject(Id = ESaberSlot.Left)] SaberModel leftSaber,
            [Inject(Id = ESaberSlot.Right)] SaberModel rightSaber,
            PresetSaveManager presetSaveManager,
            MainAssetStore mainAssetStore)
        {
            _presetSaveManager = presetSaveManager;
            _mainAssetStore = mainAssetStore;
            LeftSaber = leftSaber;
            RightSaber = rightSaber;
        }

        public async Task FromJson(JObject obj, Serializer serializer)
        {
            try
            {
                await LeftSaber.FromJson((JObject)obj[nameof(LeftSaber)], serializer);
                await RightSaber.FromJson((JObject)obj[nameof(RightSaber)], serializer);
            }
            catch (Exception e)
            {
                _logger.Error("Saber loading error:\n"+e);
                throw;
            }
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

        /// <summary>
        /// Set custom saber by name
        /// </summary>
        /// <param name="saberName"></param>
        public async Task SetSaber(string saberName)
        {
            var metaData = _mainAssetStore.GetAllMetaData(AssetTypeDefinition.CustomSaber);
            var saber = metaData.FirstOrDefault(x => x.Name == saberName);
            await SetSaber(saber);
        }

        /// <summary>
        /// Set model composition by <see cref="PreloadMetaData"/>
        /// </summary>
        /// <param name="preloadData"></param>
        public async Task SetSaber(PreloadMetaData preloadData)
        {
            if (preloadData == null)
            {
                return;
            }

            SetModelComposition(await _mainAssetStore.GetCompositionByMeta(preloadData));
        }

        public async Task Save(PresetInfo presetInfo = null)
        {
            var preset = presetInfo ?? LoadedPreset;
            
            if (preset == null)
            {
                return;
            }
            
            try
            {
                await _presetSaveManager.SaveSaber(this, preset.File.Name);
                if (presetInfo != null)
                {
                    LoadedPreset = presetInfo;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error saving saberset");
                Debug.LogError(e);
            }
            
        }

        public async Task Load(PresetInfo presetInfo)
        {
            try
            { 
                CurrentTask = _presetSaveManager.LoadSaber(this, presetInfo.File.Name);
                LoadedPreset = presetInfo;
                await CurrentTask;
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading saberset");
                Debug.LogError(e);
            }
            
            CurrentTask = null;
        }

        public async Task Reload()
        {
            await Load(LoadedPreset);
        }

        public void Sync(SaberModel fromModel)
        {
            fromModel.Sync();
            var otherSaber = fromModel == LeftSaber ? RightSaber : LeftSaber;
            otherSaber.SaberWidth = fromModel.SaberWidth;
            otherSaber.SaberLength = fromModel.SaberLength;
        }
    }
}