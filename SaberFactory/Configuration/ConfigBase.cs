using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using SaberFactory.Helpers;
using UnityEngine;
using Zenject;

namespace SaberFactory.Configuration
{
    public abstract class ConfigBase : IInitializable, IDisposable
    {
        [JsonIgnore] public bool Exists => ConfigFile.Exists;

        [JsonIgnore] public bool LoadOnInit = true;

        [JsonIgnore] public bool SaveOnDispose = true;

        protected readonly FileInfo ConfigFile;

        private readonly Dictionary<PropertyInfo, object> _originalValues = new();

        private bool _didLoadingFail;

        protected ConfigBase(PluginDirectories pluginDirs, string fileName)
        {
            ConfigFile = pluginDirs.SaberFactoryDir.GetFile(fileName);
        }

        public void Dispose()
        {
            // if loading failed, we don't want to put the original values back
            // let the user fix the config file
            if (SaveOnDispose && !_didLoadingFail)
            {
                Save();
            }
        }

        public void Initialize()
        {
            // store original values for reverting feature
            foreach (var propertyInfo in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                _originalValues.Add(propertyInfo, propertyInfo.GetValue(this));
            }

            if (LoadOnInit)
            {
                Load();
            }
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
            if (!Exists)
            {
                return;
            }

            try
            {
                JsonConvert.PopulateObject(ConfigFile.ReadText(), this);
            }
            catch (Exception)
            {
                _didLoadingFail = true;
                Debug.LogError($"[Saber Factory Configs] Failed to load config file {ConfigFile.Name}");
            }
        }

        public void Save()
        {
            ConfigFile.WriteText(JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}