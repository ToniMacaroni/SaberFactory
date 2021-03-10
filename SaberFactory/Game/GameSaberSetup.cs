using System.Threading.Tasks;
using SaberFactory.Configuration;
using SaberFactory.Models;
using Zenject;

namespace SaberFactory.Game
{
    internal class GameSaberSetup : IInitializable
    {
        public Task SetupTask { get; private set; }

        private readonly PluginConfig _config;
        private readonly SaberSet _saberSet;

        private GameSaberSetup(PluginConfig config, SaberSet saberSet)
        {
            _config = config;
            _saberSet = saberSet;

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
            }
        }

        public void Initialize()
        {
        }
    }
}