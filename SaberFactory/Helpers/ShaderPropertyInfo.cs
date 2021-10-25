using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace SaberFactory.Helpers
{
    internal class ShaderPropertyInfo
    {
        public readonly List<ShaderColor> Colors = new List<ShaderColor>();
        public readonly List<ShaderFloat> Floats = new List<ShaderFloat>();
        public readonly List<ShaderRange> Ranges = new List<ShaderRange>();
        public readonly List<ShaderTexture> Textures = new List<ShaderTexture>();
        public readonly List<ShaderVector> Vectors = new List<ShaderVector>();

        public ShaderPropertyInfo(Shader shader)
        {
            for (var i = 0; i < shader.GetPropertyCount(); i++)
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
            if (Find(Ranges, name, out var prop))
            {
                return prop;
            }

            if (Find(Floats, name, out prop))
            {
                return prop;
            }

            if (Find(Vectors, name, out prop))
            {
                return prop;
            }

            if (Find(Textures, name, out prop))
            {
                return prop;
            }

            if (Find(Colors, name, out prop))
            {
                return prop;
            }

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

        internal class ShaderFloat : BaseProperty
        {
            public ShaderFloat(Shader shader, int idx) : base(shader, idx)
            { }

            public override object GetValue(Material mat)
            {
                return mat.GetFloat(PropId);
            }

            public override void SetValue(Material mat, object value)
            {
                mat.SetFloat(PropId, (float)value);
            }

            public override void FromJson(JToken token, Material mat, params object[] args)
            {
                SetValue(mat, token.ToObject<float>());
            }
        }

        internal class ShaderVector : BaseProperty
        {
            public ShaderVector(Shader shader, int idx) : base(shader, idx)
            { }

            public override object GetValue(Material mat)
            {
                return mat.GetVector(PropId);
            }

            public override void SetValue(Material mat, object value)
            {
                mat.SetVector(PropId, (Vector4)value);
            }

            public override void FromJson(JToken token, Material mat, params object[] args)
            {
                SetValue(mat, token.ToObject<Vector4>(Serializer.JsonSerializer));
            }

            public override JToken ToJson(Material mat)
            {
                return JArray.FromObject(GetValue(mat), Serializer.JsonSerializer);
            }
        }

        internal class ShaderTexture : BaseProperty
        {
            public ShaderTexture(Shader shader, int idx) : base(shader, idx)
            { }

            public override object GetValue(Material mat)
            {
                return mat.GetTexture(PropId);
            }

            public override void SetValue(Material mat, object value)
            {
                mat.SetTexture(PropId, (Texture)value);
            }

            public override void FromJson(JToken token, Material mat, params object[] args)
            {
                if (args[0] is null)
                {
                    return;
                }

                SetValue(mat, args[0]);
            }

            public override JToken ToJson(Material mat)
            {
                var tex = (Texture)GetValue(mat);
                return JToken.FromObject(tex.name);
            }
        }

        internal class ShaderColor : BaseProperty
        {
            public ShaderColor(Shader shader, int idx) : base(shader, idx)
            { }

            public override object GetValue(Material mat)
            {
                return mat.GetColor(PropId);
            }

            public override void SetValue(Material mat, object color)
            {
                mat.SetColor(PropId, (Color)color);
            }

            public override void FromJson(JToken token, Material mat, params object[] args)
            {
                SetValue(mat, token.ToObject<Color>(Serializer.JsonSerializer));
            }

            public override JToken ToJson(Material mat)
            {
                return JArray.FromObject(GetValue(mat), Serializer.JsonSerializer);
            }
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

            public abstract object GetValue(Material mat);

            public abstract void SetValue(Material mat, object newValue);

            public virtual void FromJson(JToken token, Material mat, params object[] args)
            {
                SetValue(mat, token.ToObject<object>());
            }

            public virtual JToken ToJson(Material mat)
            {
                return JToken.FromObject(GetValue(mat));
            }
        }
    }
}