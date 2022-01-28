using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SaberFactory.Helpers;
using SaberFactory.Serialization;

namespace SaberFactory.Models.PropHandler
{
    /// <summary>
    ///     Class to store transform data
    /// </summary>
    public class TransformPropertyBlock : IFactorySerializable
    {
        public float Width { get; set; } = 1;

        public float Offset { get; set; }

        public float Rotation { get; set; }

        public Task FromJson(JObject obj, Serializer serializer)
        {
            obj.Populate(this);
            return Task.CompletedTask;
        }

        public Task<JToken> ToJson(Serializer serializer)
        {
            return Task.FromResult<JToken>(JObject.FromObject(this, Serializer.JsonSerializer));
        }
    }
}