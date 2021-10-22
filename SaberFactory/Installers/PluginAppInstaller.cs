using System;
using System.Linq;
using IPA.Loader;
using IPA.Logging;
using SaberFactory.Configuration;
using SaberFactory.DataStore;
using SaberFactory.Helpers;
using SaberFactory.Instances;
using SaberFactory.Instances.PostProcessors;
using SaberFactory.Instances.Trail;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SaberFactory.Saving;
using SaberFactory.UI.CustomSaber.Views;
using SiraUtil;
using Zenject;

namespace SaberFactory.Installers
{
    internal class PluginAppInstaller : Installer
    {
        private readonly PluginConfig _config;
        private readonly Logger _logger;
        private readonly PluginMetadata _metadata;

        private PluginAppInstaller(Logger logger, PluginConfig config, PluginMetadata metadata)
        {
            _logger = logger;
            _config = config;
            _metadata = metadata;
        }

        public override void InstallBindings()
        {
            if (_config.FirstLaunch)
            {
                _config.FirstLaunch = false;
                _config.RuntimeFirstLaunch = true;
            }

            var rtOptions = new LaunchOptions();

            if (Environment.GetCommandLineArgs().Any(x => x.ToLower() == "fpfc"))
            {
                rtOptions.FPFC = true;
                AltTrail.CapFps = true;
            }

            Container.BindInstance(rtOptions).AsSingle();

            Container.Bind<PluginDirectories>().AsSingle();

            Container.BindInstance(_metadata).WithId(nameof(SaberFactory)).AsCached();
            Container.BindLoggerAsSiraLogger(_logger);
            Container.BindInstance(_config).AsSingle();
            Container.Bind<PluginManager>().AsSingle();

            Serializer.Install(Container);
            Container.Bind<ShaderPropertyCache>().AsSingle();
            Container.Bind<PresetSaveManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<TrailConfig>().AsSingle();

            Container.BindInterfacesAndSelfTo<EmbeddedAssetLoader>().AsSingle();

            Container.Bind<CustomSaberModelLoader>().AsSingle();

            Container.Bind<TextureStore>().AsSingle();

            Container.BindInterfacesAndSelfTo<MainAssetStore>().AsSingle()
                .OnInstantiated<MainAssetStore>(OnMainAssetStoreInstantiated);


            // Model stuff
            Container.Bind<SaberModel>().WithId(ESaberSlot.Left).AsCached().WithArguments(ESaberSlot.Left);
            Container.Bind<SaberModel>().WithId(ESaberSlot.Right).AsCached().WithArguments(ESaberSlot.Right);

            Container.Bind<SaberSet>().AsSingle();

            Container.Bind<SaberFileWatcher>().AsSingle();

            InstallFactories();
            InstallMiddlewares();
        }

        private async void OnMainAssetStoreInstantiated(InjectContext ctx, MainAssetStore mainAssetStore)
        {
            await mainAssetStore.LoadAllMetaAsync(_config.AssetType);
        }

        private void InstallMiddlewares()
        {
            //Container.Bind<ISaberMiddleware>().To(x => x.AllTypes().DerivingFrom<ISaberMiddleware>()).AsSingle();
            Container.Bind<ISaberPostProcessor>().To(typeof(MainSaberPostProcessor)).AsSingle();
        }

        private void InstallFactories()
        {
            Container.BindFactory<StoreAsset, CustomSaberModel, CustomSaberModel.Factory>();

            Container.BindFactory<BasePieceModel, BasePieceInstance, BasePieceInstance.Factory>()
                .FromFactory<InstanceFactory>();
            Container.BindFactory<SaberModel, SaberInstance, SaberInstance.Factory>();
        }
    }
}