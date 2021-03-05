using System;
using System.IO;
using Newtonsoft.Json;
using SaberFactory.Helpers;
using Zenject;

namespace SaberFactory.Configuration
{
    public class TrailConfig : IInitializable, IDisposable
    {
        public int Granularity { get; set; } = 70;

        public int SamplingFrequency { get; set; } = 90;

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
            Granularity = 70;
            SamplingFrequency = 90;
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