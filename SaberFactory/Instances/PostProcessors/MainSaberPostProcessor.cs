using HarmonyLib;
using SaberFactory.Configuration;
using SaberFactory.Helpers;
using SaberFactory.ProjectComponents;
using UnityEngine;

namespace SaberFactory.Instances.PostProcessors
{
    internal class MainSaberPostProcessor : ISaberPostProcessor
    {
        private readonly PluginConfig _config;

        internal MainSaberPostProcessor(PluginConfig config)
        {
            _config = config;
        }

        public void ProcessSaber(SaberInstance saberObject)
        {
            var gameobject = saberObject.GameObject;
            gameobject.GetComponentsInChildren<Collider>().Do(x => x.enabled = false);
            gameobject.GetComponentsInChildren<AudioSource>(true).Do(x => x.volume *= _config.SaberAudioVolumeMultiplier);
            gameobject.GetComponentsInChildren<Renderer>(true).Do(x => { x.sortingOrder = 3; });

            if (gameobject.GetComponentInChildren<SFSaberSound>() is { } saberSound)
            {
                saberSound.ConfigVolume = _config.SwingSoundVolume;
            }
        }
    }
}