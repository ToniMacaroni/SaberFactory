using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SaberFactory.Serialization;
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

        public Dictionary<string, BaseProperty> PropertyByName = new();

        public Shader Shader { get; }

        public ShaderPropertyInfo(Shader shader)
        {
            Shader = shader;
            for (var i = 0; i < shader.GetPropertyCount(); i++)
            {
                switch (shader.GetPropertyType(i))
                {
                    case ShaderPropertyType.Range:
                        var rangeProp = new ShaderRange(shader, i);
                        Ranges.Add(rangeProp);
                        PropertyByName[rangeProp.Name] = rangeProp;
                        break;
                    case ShaderPropertyType.Float:
                        var floatProp = new ShaderFloat(shader, i);
                        Floats.Add(floatProp);
                        PropertyByName[floatProp.Name] = floatProp;
                        break;
                    case ShaderPropertyType.Vector:
                        var vectorProp = new ShaderVector(shader, i);
                        Vectors.Add(vectorProp);
                        PropertyByName.Add(vectorProp.Name, vectorProp);
                        break;
                    case ShaderPropertyType.Texture:
                        var texProp = new ShaderTexture(shader, i);
                        Textures.Add(texProp);
                        PropertyByName[texProp.Name] = texProp;
                        break;
                    case ShaderPropertyType.Color:
                        var colProp = new ShaderColor(shader, i);
                        Colors.Add(colProp);
                        PropertyByName[colProp.Name] = colProp;
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

        public void SetAll(Material mat, List<object> values)
        {
            var allProps = GetAll();
            
            if (allProps.Count != values.Count)
            {
                Debug.LogWarning($"Property count does not match for {Shader.name}");
            }
            
            for (int i = 0; i < allProps.Count; i++)
            {
                allProps[i].SetValue(mat, values[i]);
            }
        }

        public BaseProperty FindByName(string name)
        {
            if (PropertyByName.TryGetValue(name, out var prop))
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
                return JToken.FromObject(tex != null ? tex.name : "");
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

    internal class CachedMaterialValues
    {
        private readonly ShaderPropertyInfo _shaderInfo;
        private readonly List<object> _values = new();

        public CachedMaterialValues(ShaderPropertyInfo shaderInfo)
        {
            _shaderInfo = shaderInfo;
        }

        public void CacheValues(Material material)
        {
            _values.Clear();
            foreach (var prop in _shaderInfo.GetAll())
            {
                _values.Add(prop.GetValue(material));
            }
        }

        public void SetValues(Material material)
        {
            _shaderInfo.SetAll(material, _values);
        }
    }
}