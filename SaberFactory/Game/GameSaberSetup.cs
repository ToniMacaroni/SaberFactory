using System;
using System.Threading.Tasks;
using CustomJSONData.CustomBeatmap;
using SaberFactory.Configuration;
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
        private readonly BeatmapData _beatmapData;

        private readonly SaberModel _oldLeftSaberModel;
        private readonly SaberModel _oldRightSaberModel;

        private GameSaberSetup(PluginConfig config, SaberSet saberSet, [Inject(Id = "beatmapdata")] BeatmapData beatmapData)
        {
            _config = config;
            _saberSet = saberSet;
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
            var songData = (CustomBeatmapData) _beatmapData;
            var songSaber = CustomJSONData.Trees.at(songData.levelCustomData, "_customSaber");
            if (songSaber == null)
            {
                return;
            }
            
            _saberSet.LeftSaber = new SaberModel(ESaberSlot.Left);
            _saberSet.RightSaber = new SaberModel(ESaberSlot.Right);
            await _saberSet.SetSaber(songSaber.ToString());
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