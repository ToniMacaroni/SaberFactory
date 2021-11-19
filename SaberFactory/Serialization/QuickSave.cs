using System.IO;
using Newtonsoft.Json;

namespace SaberFactory.Serialization
{
    internal static class QuickSave
    {
        public static void SaveObject(object obj, string path, bool pretty = true)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(obj, pretty ? Formatting.Indented : Formatting.None));
        }

        public static T LoadObject<T>(string path)
        {
            if (!File.Exists(path))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
    }
}