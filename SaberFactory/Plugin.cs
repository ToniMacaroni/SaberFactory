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
using IPA.Config.Data;
using IPA.Config.Stores.Attributes;
using SaberFactory.Helpers;
using SiraUtil.Converters;
using UnityEngine;
using UnityEngine.UI;
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

            // Only create the folder if it's enabled
            // since some people don't want to have the folder in the top game directory
            if (pluginConfig.CreateCustomSabersFolder)
            {
                Directory.CreateDirectory(Path.Combine(UnityGame.InstallPath, "CustomSabers"));
            }

            if(!await LoadCsDescriptors()) return;

            zenjector.OnApp<AppInstaller>().WithParameters(logger, pluginConfig);
            zenjector.OnMenu<Installers.MenuInstaller>();
            zenjector.OnGame<GameInstaller>(false);
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
