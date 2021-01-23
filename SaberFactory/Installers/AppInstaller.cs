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
using System;
using System.IO;
using System.Reflection;
using Zenject;

namespace SaberFactory.Installers
{
    internal class AppInstaller : Installer
    {
        private readonly Logger _logger;
        private readonly PluginConfig _conf;
        private readonly DirectoryInfo _saberFactoryDir;

        private AppInstaller(Logger logger, PluginConfig conf, DirectoryInfo saberFactoryDir)
        {
            _conf = conf;
            _logger = logger;
            _saberFactoryDir = saberFactoryDir;
        }

        public override void InstallBindings()
        {
            if (_conf.FirstLaunch)
            {
                _conf.FirstLaunch = false;
                _conf.RuntimeFirstLaunch = true;
            }

            LoadCSComponents();

            Container.BindLoggerAsSiraLogger(_logger);
            Container.BindInstance(_conf).AsSingle();

            Container.Bind<SaveManager>().AsSingle().WithArguments(_saberFactoryDir.CreateSubdirectory("Presets"));
            Container.BindInterfacesAndSelfTo<CustomComponentHandler>().AsSingle();
            Container.Bind<CommonResources>().AsSingle();

            Container.Bind<EmbeddedAssetLoader>().AsSingle();

            Container.Bind<CustomSaberModelLoader>().AsSingle();

            Container.Bind<TextureStore>().AsSingle();

            if (_conf.LoadOnStart)
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

        private async void LoadCSComponents()
        {
            try
            {
                Assembly.Load(await Readers.ReadResourceAsync("SaberFactory.Resources.CustomSaberComponents.dll"));
            }
            catch (Exception )
            {
                _logger.Info("Couldn't load custom saber components");
            }
        }
    }
}