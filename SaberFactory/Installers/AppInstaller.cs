using System;
using System.IO;
using System.Reflection;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Logging;
using SaberFactory.Configuration;
using SaberFactory.DataStore;
using SaberFactory.Helpers;
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
        private readonly PluginConfig _config;
        private readonly DirectoryInfo _saberFactoryDir;

        private AppInstaller(Logger logger, PluginConfig config, DirectoryInfo saberFactoryDir)
        {
            _logger = logger;
            _config = config;
            _saberFactoryDir = saberFactoryDir;
        }

        public override void InstallBindings()
        {
            if (_config.FirstLaunch)
            {
                _config.FirstLaunch = false;
                _config.RuntimeFirstLaunch = true;
            }

            Container.BindLoggerAsSiraLogger(_logger);
            Container.BindInstance(_config).AsSingle();

            Container.Bind<SaveManager>().AsSingle().WithArguments(_saberFactoryDir.CreateSubdirectory("Presets"));
            Container.BindInterfacesAndSelfTo<CustomComponentHandler>().AsSingle();
            Container.Bind<CommonResources>().AsSingle();

            Container.BindInterfacesAndSelfTo<EmbeddedAssetLoader>().AsSingle();

            Container.Bind<CustomSaberModelLoader>().AsSingle();

            Container.Bind<TextureStore>().AsSingle();

            Container.BindInterfacesAndSelfTo<MainAssetStore>().AsSingle()
                .OnInstantiated<MainAssetStore>(OnMainAssetStoreInstansiated);

            // Model stuff
            Container.Bind<SaberModel>().WithId("LeftSaberModel").AsCached().WithArguments(ESaberSlot.Left);
            Container.Bind<SaberModel>().WithId("RightSaberModel").AsCached().WithArguments(ESaberSlot.Right);

            Container.Bind<SaberSet>().AsSingle();

            InstallFactories();
        }

        private async void OnMainAssetStoreInstansiated(InjectContext ctx, MainAssetStore mainAssetStore)
        {
            if (_config.LoadOnStart)
            {
                await mainAssetStore.LoadAll();
            }
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