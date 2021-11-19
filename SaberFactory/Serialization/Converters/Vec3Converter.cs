using System;
using Newtonsoft.Json;
using UnityEngine;

namespace SaberFactory.Serialization.Converters
{
    internal class Vec3Converter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, new[] { value.x, value.y, value.z });
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var fArr = serializer.Deserialize<float[]>(reader);
            return new Vector3(fArr[0], fArr[1], fArr[2]);
        }
    }
}