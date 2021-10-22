using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SaberFactory.Helpers;

namespace SaberFactory.Models.PropHandler
{
    /// <summary>
    ///     Class to store transform data
    /// </summary>
    internal class TransformPropertyBlock : IFactorySerializable
    {
        public float Width { get; set; } = 1;

        public float Offset { get; set; }

        public float Rotation { get; set; }

        public async Task FromJson(JObject obj, Serializer serializer)
        {
            obj.Populate(this);
        }

        public async Task<JToken> ToJson(Serializer serializer)
        {
            return JObject.FromObject(this, Serializer.JsonSerializer);
        }
    }
}