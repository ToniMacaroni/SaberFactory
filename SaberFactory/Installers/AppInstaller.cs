using IPA.Logging;
using SaberFactory.Configuration;
using SaberFactory.DataStore;
using SaberFactory.Instances;
using SaberFactory.Loaders;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SiraUtil;
using Zenject;

namespace SaberFactory.Installers
{
    internal class AppInstaller : Installer
    {
        private readonly Logger _logger;
        private readonly PluginConfig _config;

        private AppInstaller(Logger logger, PluginConfig config)
        {
            _logger = logger;
            _config = config;
        }

        public override void InstallBindings()
        {
            Container.BindLoggerAsSiraLogger(_logger);
            Container.BindInstance(_config).AsSingle();

            Container.Bind<MainAssetStore>().AsSingle();

            // Model stuff
            Container.Bind<SaberModel>().WithId("LeftSaberModel").AsSingle();
            Container.Bind<SaberModel>().WithId("RightSaberModel").AsSingle();

            Container.Bind<SaberSet>().AsSingle();

            InstallFactories();
        }

        private void InstallFactories()
        {
            Container.BindFactory<StoreAsset, CustomSaberModel, CustomSaberModel.Factory>();
            Container.BindFactory<BasePieceModel, BasePieceInstance, BasePieceInstance.Factory>();
            Container.BindFactory<SaberModel, SaberInstance, SaberInstance.Factory>();
        }
    }
}