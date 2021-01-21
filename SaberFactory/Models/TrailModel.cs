using UnityEngine;

namespace SaberFactory.Models
{
    internal class TrailModel
    {
        public Vector3 TrailPosOffset;
        public float Width;
        public int Length;
        public Material Material;
        public float Whitestep;

        public TrailModel(Vector3 trailPosOffset, float width, int length, Material material, float whitestep)
        {
            TrailPosOffset = trailPosOffset;
            Width = width;
            Length = length;
            Material = material;
            Whitestep = whitestep;
        }
    }
}