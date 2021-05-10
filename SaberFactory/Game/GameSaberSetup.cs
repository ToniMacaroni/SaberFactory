using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SaberFactory.Configuration;
using SaberFactory.DataStore;
using SaberFactory.Models;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace SaberFactory.Game
{
    internal class GameSaberSetup : IInitializable, IDisposable
    {
        public Task SetupTask { get; private set; }

        private readonly PluginConfig _config;
        private readonly SaberSet _saberSet;
        private readonly MainAssetStore _mainAssetStore;
        private readonly BeatmapData _beatmapData;

        private readonly SaberModel _oldLeftSaberModel;
        private readonly SaberModel _oldRightSaberModel;

        private bool _gotCJDTypes;
        private MethodInfo _cjdAtMethod;
        private PropertyInfo _cjdLevelCustomDataType;

        private GameSaberSetup(PluginConfig config, SaberSet saberSet, MainAssetStore mainAssetStore, [Inject(Id = "beatmapdata")] BeatmapData beatmapData)
        {
            _config = config;
            _saberSet = saberSet;
            _mainAssetStore = mainAssetStore;
            _beatmapData = beatmapData;

            _oldLeftSaberModel = _saberSet.LeftSaber;
            _oldRightSaberModel = _saberSet.RightSaber;

            Setup();
        }

        public async void Setup()
        {
            SetupTask = SetupInternal();
            await SetupTask;
        }

        private async Task SetupInternal()
        {
            if (_config.RandomSaber)
            {
                await _saberSet.Randomize();
                return;
            }

            if (!_config.OverrideSongSaber)
            {
                await SetupSongSaber();
                return;
            }
        }

        private void GetTypes()
        {
            if (_gotCJDTypes) return;
            _gotCJDTypes = true;

            var customBeatmapDataType =
                Type.GetType(
                    "CustomJSONData.CustomBeatmap.CustomBeatmapData, CustomJSONData, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null");

            var treesType =
                Type.GetType(
                    "CustomJSONData.Trees, CustomJSONData, Version=1.1.4.0, Culture=neutral, PublicKeyToken=null");

            _cjdAtMethod = treesType?.GetMethod("at", BindingFlags.Static | BindingFlags.Public);
            _cjdLevelCustomDataType = customBeatmapDataType?.GetProperty("levelCustomData", BindingFlags.Public | BindingFlags.Instance);
        }

        private async Task SetupSongSaber()
        {
            try
            {
                GetTypes();
                if (_cjdAtMethod == null || _cjdLevelCustomDataType == null)
                {
                    return;
                }

                var songSaber = _cjdAtMethod?.Invoke(null, new[] { _cjdLevelCustomDataType?.GetValue(_beatmapData), "_customSaber"});
                if (songSaber == null)
                {
                    return;
                }

                var metaData = _mainAssetStore.GetAllMetaData(AssetTypeDefinition.CustomSaber);
                var saber = metaData.FirstOrDefault(x => x.ListName == songSaber.ToString());
                if (saber == null)
                {
                    return;
                }

                _saberSet.LeftSaber = new SaberModel(ESaberSlot.Left);
                _saberSet.RightSaber = new SaberModel(ESaberSlot.Right);
                await _saberSet.SetSaber(saber);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
            
        }

        public void Initialize()
        {

        }

        public void Dispose()
        {
            _saberSet.LeftSaber = _oldLeftSaberModel;
            _saberSet.RightSaber = _oldRightSaberModel;
        }
    }
}