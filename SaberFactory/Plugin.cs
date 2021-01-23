using System;
using System.IO;
using System.Reflection;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Utilities;
using SaberFactory.Configuration;
using SaberFactory.Helpers;
using SaberFactory.Installers;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace SaberFactory
{

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        private IPALogger _logger;

        [Init]
        public void Init(IPALogger logger, Config conf, Zenjector zenjector)
        {
            _logger = logger;

            var pluginConfig = conf.Generated<PluginConfig>();
            var saberFactoryDir = new DirectoryInfo(UnityGame.UserDataPath).CreateSubdirectory("Saber Factory");
            LoadCSDescriptors();

            zenjector.OnApp<AppInstaller>().WithParameters(logger, pluginConfig, saberFactoryDir);
            zenjector.OnMenu<Installers.MenuInstaller>();
            zenjector.OnGame<GameInstaller>(false);
        }

        [OnStart]
        public void OnApplicationStart()
        {
        }

        [OnExit]
        public void OnApplicationQuit()
        {
        }

        private async void LoadCSDescriptors()
        {
            try
            {
                Assembly.Load(await Readers.ReadResourceAsync("SaberFactory.Resources.CustomSaberComponents.dll"));
            }
            catch (Exception)
            {
                _logger.Info("Couldn't load custom saber descriptors");
            }
        }
    }
}
