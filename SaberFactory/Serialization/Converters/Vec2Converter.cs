using System;
using Newtonsoft.Json;
using UnityEngine;

namespace SaberFactory.Serialization.Converters
{
    internal class Vec2Converter : JsonConverter<Vector2>
    {
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, new[] { value.x, value.y });
        }

        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var fArr = serializer.Deserialize<float[]>(reader);
            return new Vector2(fArr[0], fArr[1]);
        }
    }
}