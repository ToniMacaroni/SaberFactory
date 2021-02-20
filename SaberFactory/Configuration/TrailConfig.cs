using System;
using System.IO;
using Newtonsoft.Json;
using SaberFactory.Helpers;
using Zenject;

namespace SaberFactory.Configuration
{
    public class TrailConfig : IInitializable, IDisposable
    {
        public int Granularity { get; set; } = 60;

        public int SamplingFrequency { get; set; } = 80;

        private readonly FileInfo _configFile;

        public TrailConfig(DirectoryInfo configDir)
        {
            _configFile = configDir.GetFile("TrailConfig.json");
        }

        public void Load()
        {
            if (!_configFile.Exists) return;
            JsonConvert.PopulateObject(_configFile.ReadText(), this);
        }

        public void Save()
        {
            _configFile.WriteText(JsonConvert.SerializeObject(this));
        }

        public void Revert()
        {
            Granularity = 60;
            SamplingFrequency = 80;
        }

        public void Initialize()
        {
            Load();
        }

        public void Dispose()
        {
            Save();
        }
    }
}