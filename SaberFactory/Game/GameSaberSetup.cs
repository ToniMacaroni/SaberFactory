using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SaberFactory.Configuration;
using SaberFactory.DataStore;
using SaberFactory.Models;
using UnityEngine;
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

        private async Task SetupSongSaber()
        {
            try
            {
                if (!_beatmapData.GetField("_customSaber", out var songSaber)) return;

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