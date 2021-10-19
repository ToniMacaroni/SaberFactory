using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaberFactory.DataStore;
using SaberFactory.Models;
using SaberFactory.Saving.Converters;
using UnityEngine;
using Zenject;

namespace SaberFactory.Helpers
{
    internal class Serializer
    {
        public static readonly JsonSerializer JsonSerializer = new JsonSerializer();

        [Inject] private readonly MainAssetStore _mainAssetStore = null;
        [Inject] private readonly ShaderPropertyCache _shaderPropertyCache = null;
        [Inject] private readonly TextureStore _textureStore = null;

        static Serializer()
        {
            JsonSerializer.Converters.Add(new Vec2Converter());
            JsonSerializer.Converters.Add(new Vec3Converter());
            JsonSerializer.Converters.Add(new Vec4Converter());
            JsonSerializer.Converters.Add(new ColorConverter());
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
            return await _mainAssetStore[pathTkn.ToObject<string>()];
        }

        public JToken SerializeMaterial(Material material)
        {
            var result = new JObject();
            var shaderInfo = _shaderPropertyCache[material.shader];

            foreach (var prop in shaderInfo.GetAll())
            {
                result.Add(prop.Name, prop.ToJson(material));
            }

            return result;
        }

        public async Task LoadMaterial(JObject ser, Material material)
        {
            var shaderInfo = _shaderPropertyCache[material.shader];
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