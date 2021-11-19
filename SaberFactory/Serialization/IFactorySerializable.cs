using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SaberFactory.Serialization
{
    internal interface IFactorySerializable
    {
        public Task FromJson(JObject obj, Serializer serializer);

        public Task<JToken> ToJson(Serializer serializer);
    }
}