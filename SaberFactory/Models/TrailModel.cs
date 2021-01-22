using SaberFactory.Instances;
using UnityEngine;

namespace SaberFactory.Models
{
    internal class TrailModel
    {
        public Vector3 TrailPosOffset;
        public float Width;
        public int Length;
        public MaterialDescriptor Material;
        public float Whitestep;

        public string TrailOrigin;

        public TrailModel(Vector3 trailPosOffset, float width, int length, MaterialDescriptor material, float whitestep, string trailOrigin = "")
        {
            TrailPosOffset = trailPosOffset;
            Width = width;
            Length = length;
            Material = material;
            Whitestep = whitestep;
            TrailOrigin = trailOrigin;
        }
    }
}