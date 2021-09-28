using HarmonyLib;
using SaberFactory.Configuration;
using UnityEngine;
using SaberFactory.Helpers;

namespace SaberFactory.Instances.PostProcessors
{
    internal class MainSaberPostProcessor : ISaberPostProcessor
    {
        private readonly PluginConfig _config;

        internal MainSaberPostProcessor(PluginConfig config)
        {
            _config = config;
        }
        
        public void ProcessSaber(GameObject saberObject)
        {
            saberObject.SetLayer<Renderer>(12);
            saberObject.GetComponentsInChildren<Collider>().Do(x=>x.enabled = false);
            saberObject.GetComponentsInChildren<AudioSource>(true).Do(x=>x.volume = x.volume*_config.SaberAudioVolumeMultiplier);
        }
    }
}