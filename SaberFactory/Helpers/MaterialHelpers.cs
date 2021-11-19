using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace SaberFactory.Helpers
{
    internal static class MaterialHelpers
    {
        public static bool TryGetTexture(this Material material, string propName, out Texture tex)
        {
            tex = null;
            if (!material.HasProperty(propName))
            {
                return false;
            }

            tex = material.GetTexture(propName);
            return tex != null;
        }

        public static bool TryGetTexture(this Material material, int propId, out Texture tex)
        {
            tex = null;
            if (!material.HasProperty(propId))
            {
                return false;
            }

            tex = material.GetTexture(propId);
            return tex != null;
        }

        public static bool TryGetMainTexture(this Material material, out Texture tex)
        {
            return TryGetTexture(material, MaterialProperties.MainTexture, out tex);
        }

        public static bool TryGetFloat(this Material material, string propName, out float val)
        {
            val = 0;
            if (!material.HasProperty(propName))
            {
                return false;
            }

            val = material.GetFloat(propName);
            return true;
        }

        public static bool TryGetFloat(this Material material, int propId, out float val)
        {
            val = 0;
            if (!material.HasProperty(propId))
            {
                return false;
            }

            val = material.GetFloat(propId);
            return true;
        }

        public static void SetMainColor(this Material material, Color color)
        {
            if (material.HasProperty(MaterialProperties.MainColor))
            {
                material.SetColor(MaterialProperties.MainColor, color);
            }
        }

        public static bool HasCustomColorsEnabled(this Material material)
        {
            return material.TryGetFloat(MaterialProperties.CustomColors, out var customColors) && customColors > 0.5f;
        }

        public static IEnumerable<(object, int, ShaderPropertyType)> GetProperties(this Material material, string ignoredAttribute = null)
        {
            var shader = material.shader;
            var propCount = shader.GetPropertyCount();
            for (var i = 0; i < propCount; i++)
            {
                if (!string.IsNullOrEmpty(ignoredAttribute) &&
                    shader.GetPropertyAttributes(i).Contains(ignoredAttribute))
                {
                    continue;
                }

                var nameId = shader.GetPropertyNameId(i);
                var type = shader.GetPropertyType(i);
                yield return (material.GetProperty(nameId, type), nameId, type);
            }
        }

        public static object GetProperty(this Material material, int nameId, ShaderPropertyType type)
        {
            switch (type)
            {
                case ShaderPropertyType.Color:
                    return material.GetColor(nameId);
                case ShaderPropertyType.Vector:
                    return material.GetVector(nameId);
                case ShaderPropertyType.Float:
                    return material.GetFloat(nameId);
                case ShaderPropertyType.Range:
                    return material.GetFloat(nameId);
                case ShaderPropertyType.Texture:
                    return material.GetTexture(nameId);
            }

            return null;
        }

        public static void SetProperty(this Material material, int nameId, object obj)
        {
            var type = obj.GetType();
            if (type == typeof(Color))
            {
                material.SetColor(nameId, (Color)obj);
                return;
            }

            if (type == typeof(Vector2))
            {
                material.SetVector(nameId, (Vector2)obj);
                return;
            }

            if (type == typeof(Vector3))
            {
                material.SetVector(nameId, (Vector3)obj);
                return;
            }

            if (type == typeof(Vector4))
            {
                material.SetColor(nameId, (Vector4)obj);
                return;
            }

            if (type == typeof(float))
            {
                material.SetFloat(nameId, (float)obj);
                return;
            }

            if (type == typeof(Texture))
            {
                material.SetTexture(nameId, (Texture)obj);
            }
        }

        public static void SetProperty(this Material material, int nameId, object obj, ShaderPropertyType type)
        {
            switch (type)
            {
                case ShaderPropertyType.Color:
                    material.SetColor(nameId, (Color)obj);
                    return;
                case ShaderPropertyType.Vector:
                    var objType = obj.GetType();
                    if (objType == typeof(Vector2))
                    {
                        material.SetVector(nameId, (Vector2)obj);
                        return;
                    }

                    if (objType == typeof(Vector3))
                    {
                        material.SetVector(nameId, (Vector3)obj);
                        return;
                    }

                    if (objType == typeof(Vector4))
                    {
                        material.SetColor(nameId, (Vector4)obj);
                        return;
                    }

                    return;
                case ShaderPropertyType.Float:
                    material.SetFloat(nameId, (float)obj);
                    return;
                case ShaderPropertyType.Range:
                    material.SetFloat(nameId, (float)obj);
                    return;
                case ShaderPropertyType.Texture:
                    material.SetTexture(nameId, (Texture)obj);
                    return;
            }
        }
    }
}