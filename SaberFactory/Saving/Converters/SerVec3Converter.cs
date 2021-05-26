using System;
using Newtonsoft.Json;
using SaberFactory.Saving.Converters.SerializableTypes;
using UnityEngine;

namespace SaberFactory.Saving.Converters
{
    internal class SerVec3Converter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, SerializableVector3.FromVector(value));
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return serializer.Deserialize<SerializableVector3>(reader).ToVector();
        }
    }
}