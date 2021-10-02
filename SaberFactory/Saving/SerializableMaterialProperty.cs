using SaberFactory.Saving.Converters.SerializableTypes;
using UnityEngine;
using UnityEngine.Rendering;

namespace SaberFactory.Saving
{
    internal class SerializableMaterialProperty
    {
        public string Name;
        public ShaderPropertyType Type;
        public object Value;

        public static SerializableMaterialProperty Get(Material material, ShaderPropertyType type, string name)
        {
            object value = type switch
            {
                ShaderPropertyType.Float => material.GetFloat(name),
                ShaderPropertyType.Range => material.GetFloat(name),
                ShaderPropertyType.Color => SerializableColor.FromColor(material.GetColor(name)),
                ShaderPropertyType.Vector => SerializableVector4.FromVector(material.GetVector(name)),
                ShaderPropertyType.Texture => material.GetTexture(name)?.name,
                _ => null
            };

            return value == null ? null : new SerializableMaterialProperty { Name = name, Type = type, Value = value };
        }
    }
}