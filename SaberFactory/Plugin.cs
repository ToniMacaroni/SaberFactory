using System;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Utilities;
using SaberFactory.Configuration;
using SaberFactory.Installers;
using SiraUtil.Zenject;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using SaberFactory.Helpers;
using IPALogger = IPA.Logging.Logger;

namespace SaberFactory
{

    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        private IPALogger _logger;
        private Harmony _harmony;

        [Init]
        public async void Init(IPALogger logger, Config conf, Zenjector zenjector)
        {
            _logger = logger;

            _harmony = new Harmony("com.tonimacaroni.saberfactory");

            var pluginConfig = conf.Generated<PluginConfig>();

            // Only create the folder if it's enabled
            // since some people don't want to have the folder in the top game directory
            if (pluginConfig.CreateCustomSabersFolder)
            {
                Directory.CreateDirectory(Path.Combine(UnityGame.InstallPath, "CustomSabers"));
            }

            if(!await LoadCSDescriptors()) return;

            zenjector.OnApp<AppInstaller>().WithParameters(logger, pluginConfig);
            zenjector.OnMenu<Installers.MenuInstaller>();
            zenjector.OnGame<GameInstaller>(false);
        }

        [OnStart]
        public void OnApplicationStart()
        {
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            _harmony.UnpatchAll();
        }

        /// <summary>
        /// Load the SaberDecriptor / CustomTrail / EventManager classes
        /// from the CustomSaber namespace so they can be accessed in Saber Factory
        /// </summary>
        private async Task<bool> LoadCSDescriptors()
        {
            try
            {
                Assembly.Load(await Readers.ReadResourceAsync("SaberFactory.Resources.CustomSaberComponents.dll"));
                return true;
            }
            catch (Exception)
            {
                _logger.Info("Couldn't load custom saber descriptors");
                return false;
            }
        }
    }
}
