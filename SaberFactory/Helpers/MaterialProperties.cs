using UnityEngine;

namespace SaberFactory.Helpers
{
    internal static class MaterialProperties
    {
        public static readonly int MainColor = Shader.PropertyToID("_Color");
        public static readonly int MainTexture = Shader.PropertyToID("_MainTex");
        public static readonly int CustomColors = Shader.PropertyToID("_CustomColors");
        public static readonly int Glow = Shader.PropertyToID("_Glow");
        public static readonly int Bloom = Shader.PropertyToID("_Bloom");

        public static readonly int UserColorLeft = Shader.PropertyToID("_UserColorLeft");
        public static readonly int UserColorRight = Shader.PropertyToID("_UserColorRight");
    }
}