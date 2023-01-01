using System;
using Newtonsoft.Json;
using SaberFactory.AssetProperties;
using UnityEngine;

namespace SaberFactory.Serialization.Converters;

internal class AssetPropertyConverter : JsonConverter<AssetProperty>
{
    public override void WriteJson(JsonWriter writer, AssetProperty value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value.GetSerializableObject());
    }

    public override AssetProperty ReadJson(JsonReader reader, Type objectType, AssetProperty existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (hasExistingValue)
        {
            existingValue.SetSerializableObject(serializer, reader);
            return existingValue;
        }

        return null;
    }
}