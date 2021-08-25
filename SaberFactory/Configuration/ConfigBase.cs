using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using SaberFactory.Helpers;
using Zenject;

namespace SaberFactory.Configuration
{
    public abstract class ConfigBase : IInitializable, IDisposable
    {
        [JsonIgnore]
        public bool Exists => ConfigFile.Exists;

        [JsonIgnore]
        public bool LoadOnInit = true;

        [JsonIgnore]
        public bool SaveOnDispose = true;

        protected readonly FileInfo ConfigFile;

        private readonly Dictionary<PropertyInfo, object> _originalValues = new Dictionary<PropertyInfo, object>();

        protected ConfigBase(SFDirectories sfDirs, string fileName)
        {
            ConfigFile = sfDirs.SaberFactoryDir.GetFile(fileName);
        }

        public void Revert()
        {
            foreach (var originalValue in _originalValues)
            {
                originalValue.Key.SetValue(this, originalValue.Value);
            }
        }

        public void Load()
        {
            if (!Exists) return;
            JsonConvert.PopulateObject(ConfigFile.ReadText(), this);
        }

        public void Save()
        {
            ConfigFile.WriteText(JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public void Initialize()
        {
            // store original values for reverting feature
            foreach (var propertyInfo in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                _originalValues.Add(propertyInfo, propertyInfo.GetValue(this));
            }

            if (LoadOnInit) Load();
        }

        public void Dispose()
        {
            if (SaveOnDispose) Save();
        }
    }
}