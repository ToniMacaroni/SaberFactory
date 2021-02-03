using System.Threading.Tasks;
using SaberFactory.Configuration;
using SaberFactory.Models;
using Zenject;

namespace SaberFactory.Game
{
    internal class GameSaberSetup
    {
        public Task SetupTask { get; private set; }

        private readonly PluginConfig _config;
        private readonly SaberSet _saberSet;

        private GameSaberSetup(PluginConfig config, SaberSet saberSet)
        {
            _config = config;
            _saberSet = saberSet;

            Initialize();
        }

        public async void Initialize()
        {
            SetupTask = Setup();
            await SetupTask;
        }

        public async Task Setup()
        {
            if (_config.RandomSaber)
            {
                await _saberSet.Randomize();
            }
        }
    }
}