using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace SaberFactory.Helpers
{
    public static class AsyncReaders
    {
        public static async Task<byte[]> ReadFileAsync(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
            return await stream.ReadStreamAsync();
        }

        public static async Task<byte[]> ReadStreamAsync(this Stream stream)
        {
            var result = new byte[stream.Length];
            await stream.ReadAsync(result, 0, result.Length);
            return result;
        }

        public static async Task<byte[]> ReadResource(string path)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            return await ReadStreamAsync(stream);
        }

        public static async Task<AssetBundle> LoadAssetBundleAsync(byte[] data)
        {
            var taskSource = new TaskCompletionSource<AssetBundle>();

            AssetBundleCreateRequest asetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(data);
            asetBundleCreateRequest.completed += delegate
            {
                AssetBundle assetBundle = asetBundleCreateRequest.assetBundle;
                taskSource.TrySetResult(assetBundle);
            };
            return await taskSource.Task;
        }

        public static async Task<T> LoadAssetFromAssetBundleAsync<T>(this AssetBundle assetBundle, string assetName) where T : UnityEngine.Object
        {
            var taskSource = new TaskCompletionSource<T>();

            AssetBundleRequest assetBundleRequest = assetBundle.LoadAssetAsync<T>(assetName);
            assetBundleRequest.completed += delegate
            {
                T asset = (T)assetBundleRequest.asset;

                taskSource.TrySetResult(asset);
            };

            return await taskSource.Task;
        }

        public static async Task<Tuple<T, AssetBundle>> LoadAssetFromAssetBundleAsync<T>(byte[] bundleData, string assetName) where T : UnityEngine.Object
        {
            var assetBundle = await LoadAssetBundleAsync(bundleData);
            if (assetBundle == null) return null;

            var asset = await assetBundle.LoadAssetFromAssetBundleAsync<T>(assetName);

            if (asset == null)
            {
                assetBundle.Unload(true);
            }

            return new Tuple<T, AssetBundle>(asset, assetBundle);
        }

        public static async Task<Tuple<T, AssetBundle>> LoadAssetFromAssetBundleAsync<T>(string path,
            string assetName) where T : UnityEngine.Object
        {
            var fileData = await ReadFileAsync(path);
            if (fileData == null) return null;
            return await LoadAssetFromAssetBundleAsync<T>(fileData, assetName);
        }
    }
}