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
                ShaderPropertyType.Texture => material.GetTexture(name).name,
                _ => null
            };

            return value == null ? null : new SerializableMaterialProperty {Name = name, Type = type, Value = value};
        }
    }

    internal class SerializableVector4
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Vector4 ToVector()
        {
            return new Vector4(X, Y, Z, W);
        }

        public static SerializableVector4 FromVector(Vector4 vec)
        {
            return new SerializableVector4
            {
                X = vec.x,
                Y = vec.y,
                Z = vec.z,
                W = vec.w
            };
        }
    }

    internal class SerializableColor
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public Color ToColor()
        {
            return new Color(R, G, B, A);
        }

        public static SerializableColor FromColor(Color color)
        {
            return new SerializableColor
            {
                R = color.r,
                G = color.g,
                B = color.b,
                A = color.a
            };
        }
    }
}