using SaberFactory.DataStore;
using SaberFactory.Models;
using Zenject;

namespace SaberFactory.Tests
{
    internal class LoadingTester : IInitializable
    {
        [Inject] private readonly MainAssetStore _mainAssetStore = null;
        [Inject] private readonly SaberSet _saberSet = null;

        public async void Initialize()
        {
            var composition = await _mainAssetStore["CustomSabers\\zIndustrial.saber"];
        }
    }
}