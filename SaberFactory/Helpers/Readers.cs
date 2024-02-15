using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AssetBundleLoadingTools.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaberFactory.Helpers
{
    public static class Readers
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

        public static byte[] ReadStream(this Stream stream)
        {
            var result = new byte[stream.Length];
            stream.Read(result, 0, result.Length);
            return result;
        }

        public static async Task<byte[]> ReadResourceAsync(string path)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            if (stream == null)
            {
                return null;
            }

            return await ReadStreamAsync(stream);
        }

        public static byte[] ReadResource(string path)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            if (stream == null)
            {
                return null;
            }

            return ReadStream(stream);
        }

        public static string BytesToString(this byte[] data)
        {
            return Encoding.UTF8.GetString(data, data[0] == 0xef?3:0, data.Length-(data[0] == 0xef?3:0));
        }

        public static async Task<Texture2D> ReadTexture(string path)
        {
            var data = await ReadFileAsync(path);
            var tex = new Texture2D(2, 2);
            tex.LoadImage(data);
            return tex;
        }

        public static async Task<AssetBundle> LoadAssetBundleAsync(byte[] data)
        {
            var taskSource = new TaskCompletionSource<AssetBundle>();

            var asetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(data);
            asetBundleCreateRequest.completed += delegate
            {
                var assetBundle = asetBundleCreateRequest.assetBundle;
                taskSource.TrySetResult(assetBundle);
            };
            return await taskSource.Task;
        }

        public static async Task<T> LoadAssetFromAssetBundleAsync<T>(this AssetBundle assetBundle, string assetName) where T : Object
        {
            var taskSource = new TaskCompletionSource<T>();

            var assetBundleRequest = assetBundle.LoadAssetAsync<T>(assetName);
            assetBundleRequest.completed += delegate
            {
                var asset = (T)assetBundleRequest.asset;

                taskSource.TrySetResult(asset);
            };

            return await taskSource.Task;
        }

        public static async Task<Tuple<T, AssetBundle>> LoadAssetFromAssetBundleAsync<T>(byte[] bundleData, string assetName) where T : Object
        {
            var assetBundle = await LoadAssetBundleAsync(bundleData);
            if (assetBundle == null)
            {
                return null;
            }

            var asset = await assetBundle.LoadAssetFromAssetBundleAsync<T>(assetName);

            if (asset == null)
            {
                assetBundle.Unload(true);
            }

            return new Tuple<T, AssetBundle>(asset, assetBundle);
        }

        public static async Task<Tuple<T, AssetBundle>> LoadAssetFromAssetBundleAsync<T>(string path, string assetName) where T : Object
        {
            var tcs = new TaskCompletionSource<Tuple<T, AssetBundle>>();

            var createRequest = AssetBundle.LoadFromFileAsync(path);
            createRequest.completed += delegate
            {
                if (!createRequest.assetBundle)
                {
                    tcs.SetResult(null);
                    return;
                }

                var assetRequest = createRequest.assetBundle.LoadAssetAsync<T>(assetName);
                assetRequest.completed += delegate
                {
                    Object asset = assetRequest.asset;

                    if (asset == null)
                    {
                        createRequest.assetBundle.Unload(true);
                        tcs.SetResult(null);
                        return;
                    }

                    tcs.SetResult(new Tuple<T, AssetBundle>((T)asset, createRequest.assetBundle));
                };
            };

            return await tcs.Task;
        }
    }
}