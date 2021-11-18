using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SaberFactory.Serialization
{
    internal static class JHelpers
    {
        public static void Populate<T>(this JToken value, T target)
        {
            using var sr = value.CreateReader();
            Serializer.JsonSerializer.Populate(sr, target);
        }

        public static async Task SaveToFile(this IFactorySerializable value, Serializer serializer, string filename)
        {
            File.WriteAllText(filename, (await value.ToJson(serializer)).ToString());
        }

        public static async Task LoadFromFile(this IFactorySerializable value, Serializer serializer, string filename)
        {
            var obj = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(filename));
            await value.FromJson(obj, serializer);
        }
    }
}