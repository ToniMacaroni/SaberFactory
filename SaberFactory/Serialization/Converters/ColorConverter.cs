using System;
using Newtonsoft.Json;
using UnityEngine;

namespace SaberFactory.Serialization.Converters
{
    internal class ColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, new[] { value.r, value.g, value.b, value.a });
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var fArr = serializer.Deserialize<float[]>(reader);
            return new Color(fArr[0], fArr[1], fArr[2], fArr[3]);
        }
    }
}