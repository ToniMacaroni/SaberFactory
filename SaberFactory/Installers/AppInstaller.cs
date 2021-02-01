using IPA.Logging;
using SaberFactory.Configuration;
using SaberFactory.DataStore;
using SaberFactory.Instances;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SaberFactory.Saving;
using SaberFactory.UI.Lib.BSML;
using SiraUtil;
using System.IO;
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

            Container.Bind<PresetSaveManager>().AsSingle().WithArguments(_saberFactoryDir.CreateSubdirectory("Presets"));
            Container.Bind<CommonResources>().AsSingle();

            Container.BindInterfacesAndSelfTo<EmbeddedAssetLoader>().AsSingle();

            Container.Bind<CustomSaberModelLoader>().AsSingle();

            Container.Bind<TextureStore>().AsSingle().WithArguments(_saberFactoryDir.CreateSubdirectory("Textures"));

            Container.BindInterfacesAndSelfTo<MainAssetStore>().AsSingle()
                .OnInstantiated<MainAssetStore>(OnMainAssetStoreInstansiated);


            // Model stuff
            Container.Bind<SaberModel>().WithId(ESaberSlot.Left).AsCached().WithArguments(ESaberSlot.Left);
            Container.Bind<SaberModel>().WithId(ESaberSlot.Right).AsCached().WithArguments(ESaberSlot.Right);

            Container.Bind<SaberSet>().AsSingle();

            InstallFactories();
        }

        private async void OnMainAssetStoreInstansiated(InjectContext ctx, MainAssetStore mainAssetStore)
        {
            if (_config.LoadOnStart)
            {
                await mainAssetStore.LoadAllMetaAsync(_config.AssetType);
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