using System;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SaberFactory.Configuration;
using SaberFactory.Installers;
using SiraUtil.Zenject;
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
        private const string HarmonyId = "com.tonimacaroni.saberfactory";

        private IPALogger _logger;
        private Harmony _harmony;

        [Init]
        public async void Init(IPALogger logger, Config conf, Zenjector zenjector)
        {
            _logger = logger;

            _harmony = new Harmony(HarmonyId);

            var pluginConfig = conf.Generated<PluginConfig>();

            if(!await LoadCsDescriptors()) return;

            zenjector.OnApp<PluginAppInstaller>().WithParameters(logger, pluginConfig);
            zenjector.OnMenu<PluginMenuInstaller>();
            zenjector.OnGame<PluginGameInstaller>(false);
        }

        [OnEnable]
        public void OnEnable()
        {
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        [OnDisable]
        public void OnDisable()
        {
            _harmony.UnpatchAll(HarmonyId);
        }

        /// <summary>
        /// Load the SaberDecriptor / CustomTrail / EventManager classes
        /// from the CustomSaber namespace so they can be accessed in Saber Factory
        /// </summary>
        private async Task<bool> LoadCsDescriptors()
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
