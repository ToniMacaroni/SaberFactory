using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SaberFactory.Helpers;
using SiraUtil.Logging;
using SiraUtil.Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaberFactory
{
    internal class EmbeddedAssetLoader : IDisposable
    {
        public static readonly string BUNDLE_PATH = "SaberFactory.Resources.assets";

        private readonly SiraLog _logger;
        private AssetBundle _assetBundle;
        private Task<bool> _loadingTask;

        private EmbeddedAssetLoader(SiraLog logger)
        {
            _logger = logger;
        }

        public void Dispose()
        {
            if (_assetBundle)
            {
                _assetBundle.Unload(true);
            }
        }

        public async Task<T> LoadAsset<T>(string name) where T : Object
        {
            if (!await CheckLoaded())
            {
                return null;
            }

            return await _assetBundle.LoadAssetFromAssetBundleAsync<T>(name);
        }

        public async Task<List<T>> LoadAssets<T>(params string[] names) where T : Object
        {
            if (!await CheckLoaded())
            {
                return null;
            }

            var assets = new List<T>();

            foreach (var name in names)
            {
                var asset = await _assetBundle.LoadAssetFromAssetBundleAsync<T>(name);
                if (asset)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        private async Task<bool> CheckLoaded()
        {
            if (_assetBundle)
            {
                return true;
            }

            _loadingTask ??= LoadBundle();

            await _loadingTask;

            return true;
        }

        private async Task<bool> LoadBundle()
        {
            var data = await Readers.ReadResourceAsync(BUNDLE_PATH);

            if (data == null)
            {
                _logger.Error($"Resource at {BUNDLE_PATH} doesn't exist");
                return false;
            }

            _assetBundle = await Readers.LoadAssetBundleAsync(data);

            if (_assetBundle == null)
            {
                _logger.Error("Couldn't load embedded AssetBundle");
                return false;
            }

#if false
            foreach (var assetName in _assetBundle.GetAllAssetNames())
            {
                var obj = _assetBundle.LoadAsset(assetName);
                Debug.LogWarning($"{assetName}: {obj.GetType().Name}");
            }
#endif

            return true;
        }
    }
}