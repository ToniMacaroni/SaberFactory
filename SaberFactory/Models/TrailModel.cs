using System.Collections.Generic;
using CustomSaber;
using SaberFactory.Instances;
using SaberFactory.Saving;
using UnityEngine;

namespace SaberFactory.Models
{
    /// <summary>
    /// Stores information on how to build a trail
    /// </summary>
    internal class TrailModel
    {
        [MapSerialize]
        public Vector3 TrailPosOffset;

        [MapSerialize]
        public float Width;

        [MapSerialize]
        public int Length;
        
        public int OriginalLength { get; private set; }

        public MaterialDescriptor Material;

        [MapSerialize]
        public float Whitestep;

        [MapSerialize]
        public string TrailOrigin;

        [MapSerialize]
        public bool ClampTexture;

        [MapSerialize]
        public bool Flip;

        public TextureWrapMode? OriginalTextureWrapMode;

        // for custom sabers with multiple trails
        public List<CustomTrail> TrailOriginTrails;

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
            OriginalLength = length;
            Material = material;
            Whitestep = whitestep;
            OriginalTextureWrapMode = originalTextureWrapMode;
            TrailOrigin = trailOrigin;
        }

        public TrailModel()
        {
            
        }

        public void CopyFrom(TrailModel other)
        {
            TrailPosOffset = other.TrailPosOffset;
            Width = other.Width;
            Length = other.Length;
            Material ??= new MaterialDescriptor(null);
            Material.Material = other.Material.Material;
            Whitestep = other.Whitestep;
            TrailOrigin = other.TrailOrigin;
            ClampTexture = other.ClampTexture;
            Flip = other.Flip;
            OriginalLength = other.OriginalLength;
        }
    }
}