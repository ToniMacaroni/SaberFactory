using UnityEngine;

namespace SaberFactory.Instances
{
    internal struct TrailConstructionData
    {
        public Transform TopTransform;
        public Transform BottomTransform;
        public Material Material;
        public int Length;
        public float Whitestep;
    }
}