using System;
using Newtonsoft.Json;
using UnityEngine;

namespace SaberFactory.Saving.Converters
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