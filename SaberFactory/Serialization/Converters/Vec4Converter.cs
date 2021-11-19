using System;
using Newtonsoft.Json;
using UnityEngine;

namespace SaberFactory.Serialization.Converters
{
    internal class Vec4Converter : JsonConverter<Vector4>
    {
        public override void WriteJson(JsonWriter writer, Vector4 value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, new[] { value.x, value.y, value.z, value.w });
        }

        public override Vector4 ReadJson(JsonReader reader, Type objectType, Vector4 existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var fArr = serializer.Deserialize<float[]>(reader);
            return new Vector4(fArr[0], fArr[1], fArr[2], fArr[3]);
        }
    }
}