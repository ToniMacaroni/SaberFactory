using System;
using System.Linq;
using System.Threading.Tasks;
using SaberFactory.Configuration;
using SaberFactory.DataStore;
using SaberFactory.Helpers;
using SaberFactory.Models;
using SiraUtil.Logging;
using Zenject;

namespace SaberFactory.Game
{
    internal class GameSaberSetup : IInitializable, IDisposable
    {
        public Task SetupTask { get; private set; }
        private readonly BeatmapData _beatmapData;

        private readonly PluginConfig _config;
        private readonly MainAssetStore _mainAssetStore;

        private readonly SaberModel _oldLeftSaberModel;
        private readonly SaberModel _oldRightSaberModel;
        private readonly RandomUtil _randomUtil;
        private readonly SaberSet _saberSet;
        private readonly SiraLog _logger;

        private GameSaberSetup(PluginConfig config, SaberSet saberSet, MainAssetStore mainAssetStore,
            IReadonlyBeatmapData beatmap, RandomUtil randomUtil, SiraLog logger)
        {
            _config = config;
            _saberSet = saberSet;
            _mainAssetStore = mainAssetStore;
            _beatmapData = beatmap.CastChecked<BeatmapData>();
            _randomUtil = randomUtil;
            _logger = logger;

            _oldLeftSaberModel = _saberSet.LeftSaber;
            _oldRightSaberModel = _saberSet.RightSaber;

            Setup();
        }

        public void Dispose()
        {
            _saberSet.LeftSaber = _oldLeftSaberModel;
            _saberSet.RightSaber = _oldRightSaberModel;
        }

        public void Initialize()
        { }

        public async void Setup()
        {
            SetupTask = SetupInternal();
            await SetupTask;
        }

        private async Task SetupInternal()
        {
            if (_config.RandomSaber)
            {
                await RandomSaber();
                return;
            }

            if (!_config.OverrideSongSaber)
            {
                await SetupSongSaber();
            }
        }

        private async Task RandomSaber()
        {
            if (
                _config.AssetType == EAssetTypeConfiguration.CustomSaber ||
                _config.AssetType == EAssetTypeConfiguration.None)
            {
                var randomComp = _randomUtil.RandomizeFrom(_mainAssetStore.GetAllMetaData(AssetTypeDefinition.CustomSaber).ToList());
                _saberSet.SetModelComposition(await _mainAssetStore.GetCompositionByMeta(randomComp));
            }
        }

        private async Task SetupSongSaber()
        {
            try
            {
                if (_beatmapData == null)
                {
                    return;
                }
                
                if (!_beatmapData.GetField("_customSaber", out var songSaber))
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
                _logger.Error(e.ToString());
            }
        }
    }
}