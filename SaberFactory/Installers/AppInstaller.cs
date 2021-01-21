using System.IO;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Logging;
using SaberFactory.Configuration;
using SaberFactory.DataStore;
using SaberFactory.Instances;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SaberFactory.Saving;
using SaberFactory.UI.Lib.BSML;
using SiraUtil;
using Zenject;

namespace SaberFactory.Installers
{
    internal class AppInstaller : Installer
    {
        private readonly Logger _logger;
        private readonly Config _conf;
        private readonly DirectoryInfo _saberFactoryDir;

        private AppInstaller(Logger logger, Config conf, DirectoryInfo saberFactoryDir)
        {
            _logger = logger;
            _conf = conf;
            _saberFactoryDir = saberFactoryDir;
        }

        public override void InstallBindings()
        {
            var pluginConfig = _conf.Generated<PluginConfig>();
            if (pluginConfig.FirstLaunch)
            {
                pluginConfig.FirstLaunch = false;
                pluginConfig.RuntimeFirstLaunch = true;
            }

            Container.BindLoggerAsSiraLogger(_logger);
            Container.BindInstance(pluginConfig).AsSingle();

            Container.Bind<SaveManager>().AsSingle().WithArguments(_saberFactoryDir.CreateSubdirectory("Presets"));
            Container.BindInterfacesAndSelfTo<CustomComponentHandler>().AsSingle();
            Container.Bind<CommonResources>().AsSingle();

            Container.Bind<EmbeddedAssetLoader>().AsSingle();

            Container.Bind<CustomSaberModelLoader>().AsSingle();

            if (pluginConfig.LoadOnStart)
            {
                Container.BindInterfacesAndSelfTo<MainAssetStore>().AsSingle();
            }
            else
            {
                Container.Bind<MainAssetStore>().AsSingle();
            }

            // Model stuff
            Container.Bind<SaberModel>().WithId("LeftSaberModel").AsCached().WithArguments(ESaberSlot.Left);
            Container.Bind<SaberModel>().WithId("RightSaberModel").AsCached().WithArguments(ESaberSlot.Right);

            Container.Bind<SaberSet>().AsSingle();

            InstallFactories();
        }

        private void InstallFactories()
        {
            Container.BindFactory<StoreAsset, CustomSaberModel, CustomSaberModel.Factory>();

            //Container.BindFactory<BasePieceModel, BasePieceInstance, BasePieceInstance.Factory>().FromFactory<InstanceFactory>();
            Container.BindFactory<BasePieceModel, BasePieceInstance, BasePieceInstance.Factory>()
                .FromFactory<InstanceFactory>();
            Container.BindFactory<SaberModel, SaberInstance, SaberInstance.Factory>();
        }
    }
}