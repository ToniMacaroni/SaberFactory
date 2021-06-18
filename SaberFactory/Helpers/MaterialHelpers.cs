using UnityEngine;

namespace SaberFactory.Helpers
{
    internal static class MaterialHelpers
    {
        public static bool TryGetTexture(this Material material, string propName, out Texture tex)
        {
            tex = null;
            if (!material.HasProperty(propName)) return false;
            tex = material.GetTexture(propName);
            return tex != null;
        }

        public static bool TryGetTexture(this Material material, int propId, out Texture tex)
        {
            tex = null;
            if (!material.HasProperty(propId)) return false;
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
            if (!material.HasProperty(propName)) return false;
            val = material.GetFloat(propName);
            return true;
        }

        public static bool TryGetFloat(this Material material, int propId, out float val)
        {
            val = 0;
            if (!material.HasProperty(propId)) return false;
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
    }

    internal static class MaterialProperties
    {
        public static readonly int MainColor = Shader.PropertyToID("_Color");
        public static readonly int MainTexture = Shader.PropertyToID("_MainTex");
        public static readonly int CustomColors = Shader.PropertyToID("_CustomColors");
        public static readonly int Glow = Shader.PropertyToID("_Glow");
        public static readonly int Bloom = Shader.PropertyToID("_Bloom");
    }

    internal static class MaterialAttributes
    {
        public static readonly string SfNoPreview = "SFNoPreview";
        public static readonly string HideInSf = "HideInSF";
    }
}