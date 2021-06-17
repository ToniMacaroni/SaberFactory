using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;

namespace SaberFactory.Helpers
{
    internal class ShaderPropertyInfo
    {
        public List<ShaderRange> Ranges = new List<ShaderRange>();
        public List<ShaderFloat> Floats = new List<ShaderFloat>();
        public List<ShaderVector> Vectors = new List<ShaderVector>();
        public List<ShaderTexture> Textures = new List<ShaderTexture>();
        public List<ShaderColor> Colors = new List<ShaderColor>();

        public ShaderPropertyInfo(Shader shader)
        {
            for (int i = 0; i < shader.GetPropertyCount(); i++)
            {
                switch (shader.GetPropertyType(i))
                {
                    case ShaderPropertyType.Range:
                        Ranges.Add(new ShaderRange(shader, i));
                        break;
                    case ShaderPropertyType.Float:
                        Floats.Add(new ShaderFloat(shader, i));
                        break;
                    case ShaderPropertyType.Vector:
                        Vectors.Add(new ShaderVector(shader, i));
                        break;
                    case ShaderPropertyType.Texture:
                        Textures.Add(new ShaderTexture(shader, i));
                        break;
                    case ShaderPropertyType.Color:
                        Colors.Add(new ShaderColor(shader, i));
                        break;
                }
            }
        }

        public List<BaseProperty> GetAll()
        {
            var result = new List<BaseProperty>();
            result.AddRange(Ranges);
            result.AddRange(Floats);
            result.AddRange(Vectors);
            result.AddRange(Textures);
            result.AddRange(Colors);
            return result;
        }

        public BaseProperty FindFromAll(string name)
        {
            if (Find(Ranges, name, out var prop)) return prop;
            if(Find(Floats, name, out prop)) return prop;
            if(Find(Vectors, name, out prop)) return prop;
            if(Find(Textures, name, out prop)) return prop;
            if(Find(Colors, name, out prop)) return prop;
            return null;
        }

        public bool Find<T>(List<T> list, string name, out BaseProperty prop) where T : BaseProperty
        {
            foreach (var p in list)
            {
                if (p.Name == name)
                {
                    prop = p;
                    return true;
                }
            }

            prop = null;
            return false;
        }

        internal class ShaderRange : ShaderFloat
        {
            public float Min { get; }
            public float Max { get; }

            public ShaderRange(Shader shader, int idx) : base(shader, idx)
            {
                var range = shader.GetPropertyRangeLimits(idx);
                Min = range.x;
                Max = range.y;
            }
        }

        internal class ShaderFloat : BaseProperty<float>
        {
            public ShaderFloat(Shader shader, int idx) : base(shader, idx)
            {
            }

            public override float GetValue(Material mat)
            {
                return mat.GetFloat(PropId);
            }

            public override void SetValue(Material mat, object value)
            {
                mat.SetFloat(PropId, (float)value);
            }
        }

        internal class ShaderVector : BaseProperty<Vector4>
        {
            public ShaderVector(Shader shader, int idx) : base(shader, idx)
            {
            }

            public override Vector4 GetValue(Material mat)
            {
                return mat.GetVector(PropId);
            }

            public override void SetValue(Material mat, object value)
            {
                mat.SetVector(PropId, (Vector4)value);
            }
        }

        internal class ShaderTexture : BaseProperty<Texture>
        {
            public ShaderTexture(Shader shader, int idx) : base(shader, idx)
            {
            }

            public override Texture GetValue(Material mat)
            {
                return mat.GetTexture(PropId);
            }

            public override void SetValue(Material mat, object value)
            {
                mat.SetTexture(PropId, (Texture)value);
            }
        }

        internal class ShaderColor : BaseProperty<Color>
        {
            public ShaderColor(Shader shader, int idx) : base(shader, idx)
            {
            }

            public override Color GetValue(Material mat)
            {
                return mat.GetColor(PropId);
            }

            public override void SetValue(Material mat, object color)
            {
                mat.SetColor(PropId, (Color)color);
            }
        }

        internal abstract class BaseProperty<T> : BaseProperty
        {
            protected BaseProperty(Shader shader, int idx) : base(shader, idx)
            {
            }

            public abstract T GetValue(Material mat);

            public abstract void SetValue(Material mat, object value);
        }

        internal abstract class BaseProperty
        {
            public string Name { get; }

            public string Description { get; }

            public int PropId { get; }

            public List<string> Attributes { get; }

            public int Index { get; }

            public ShaderPropertyType Type { get; }

            protected BaseProperty(Shader shader, int idx)
            {
                Index = idx;
                Name = shader.GetPropertyName(idx);
                PropId = Shader.PropertyToID(Name);
                Description = shader.GetPropertyDescription(idx);
                Attributes = shader.GetPropertyAttributes(idx).ToList();
                Type = shader.GetPropertyType(idx);
            }

            public bool HasAttribute(string name)
            {
                return Attributes.Contains(name);
            }
        }
    }
}