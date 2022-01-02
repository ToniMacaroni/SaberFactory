using System;
using System.IO;
using System.Threading.Tasks;
using SaberFactory.Helpers;
using SaberFactory.Models;
using UnityEngine;

namespace SaberFactory.Serialization
{
    public class PresetSaveManager
    {
        private readonly DirectoryInfo _presetDir;
        private readonly Serializer _serializer;

        private PresetSaveManager(PluginDirectories pluginDirs, Serializer serializer)
        {
            _serializer = serializer;
            _presetDir = pluginDirs.PresetDir;
        }

        public event Action OnSaberLoaded;

        public async Task SaveSaber(SaberSet saberSet, string fileName)
        {
            var file = _presetDir.GetFile(fileName);
            await saberSet.SaveToFile(_serializer, file.FullName);
        }

        public async Task LoadSaber(SaberSet saberSet, string fileName)
        {
            var file = _presetDir.GetFile(fileName);
            if (!file.Exists)
            {
                return;
            }

            await saberSet.LoadFromFile(_serializer, file.FullName);
            OnSaberLoaded?.Invoke();
        }
    }
}