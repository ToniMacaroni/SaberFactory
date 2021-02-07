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

        public static bool TryGetMainTexture(this Material material, out Texture tex)
        {
            return TryGetTexture(material, "_MainTex", out tex);
        }

        public static bool TryGetFloat(this Material material, string propName, out float val)
        {
            val = 0;
            if (!material.HasProperty(propName)) return false;
            val = material.GetFloat(propName);
            return true;
        }
    }
}