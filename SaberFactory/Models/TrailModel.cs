using SaberFactory.Instances;
using UnityEngine;

namespace SaberFactory.Models
{
    /// <summary>
    /// Stores information on how to build a trail
    /// </summary>
    internal class TrailModel
    {
        public Vector3 TrailPosOffset;
        public float Width;
        public int Length;
        public MaterialDescriptor Material;
        public float Whitestep;

        public string TrailOrigin;
        public bool ClampTexture;
        public TextureWrapMode? OriginalTextureWrapMode;

        public TrailModel(
            Vector3 trailPosOffset,
            float width,
            int length,
            MaterialDescriptor material,
            float whitestep,
            TextureWrapMode? originalTextureWrapMode,
            string trailOrigin = "")
        {
            TrailPosOffset = trailPosOffset;
            Width = width;
            Length = length;
            Material = material;
            Whitestep = whitestep;
            OriginalTextureWrapMode = originalTextureWrapMode;
            TrailOrigin = trailOrigin;
        }
    }
}