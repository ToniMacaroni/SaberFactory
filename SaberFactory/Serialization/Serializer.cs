using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaberFactory.DataStore;
using SaberFactory.Helpers;
using SaberFactory.Models;
using SaberFactory.Serialization.Converters;
using UnityEngine;
using Zenject;

namespace SaberFactory.Serialization
{
    public class Serializer
    {
        public static readonly JsonSerializer JsonSerializer = new();

        [Inject] private readonly MainAssetStore _mainAssetStore = null;
        [Inject] private readonly TextureStore _textureStore = null;

        static Serializer()
        {
            JsonSerializer.Converters.Add(new Vec2Converter());
            JsonSerializer.Converters.Add(new Vec3Converter());
            JsonSerializer.Converters.Add(new Vec4Converter());
            JsonSerializer.Converters.Add(new ColorConverter());
            JsonSerializer.Converters.Add(new AssetPropertyConverter());
        }

        public static void Install(DiContainer container)
        {
            container.Bind<Serializer>().AsSingle();
        }

        public static T Cast<T>(object obj)
        {
            return ((JObject)obj).ToObject<T>(JsonSerializer);
        }

        public async Task<ModelComposition> LoadPiece(JToken pathTkn)
        {
            await _mainAssetStore.WaitForFinish();
            return await _mainAssetStore[new RelativePath(pathTkn.ToObject<string>())];
        }

        public JToken SerializeMaterial(Material material)
        {
            var result = new JObject();
            var shaderInfo = ShaderInfoCache.Get(material.shader);

            var addedPropNames = new HashSet<string>();

            foreach (var prop in shaderInfo.GetAll())
            {
                if (addedPropNames.Add(prop.Name))
                {
                    result.Add(prop.Name, prop.ToJson(material));
                }
            }

            return result;
        }

        public async Task LoadMaterial(JObject ser, Material material)
        {
            var shaderInfo = ShaderInfoCache.Get(material.shader);
            foreach (var prop in shaderInfo.GetAll())
            {
                var jProp = ser.Property(prop.Name);
                if (jProp is null)
                {
                    continue;
                }

                if (prop is ShaderPropertyInfo.ShaderTexture shaderTex)
                {
                    prop.FromJson(jProp.Value, material, await ResolveTexture(jProp.Value.ToObject<string>()));
                    continue;
                }

                prop.FromJson(jProp.Value, material);
            }
        }

        /// <summary>
        ///     Get Texture from texture store by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private async Task<Texture2D> ResolveTexture(string name)
        {
            await _textureStore.WaitForFinish();
            return (await _textureStore[name])?.Texture;
        }
    }
}