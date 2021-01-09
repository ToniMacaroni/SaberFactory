using CustomSaber;
using UnityEngine;

namespace SaberFactory.Models.CustomSaber
{
    internal struct CustomSaberTrailData
    {
        public Transform PointStart;
        public Transform PointEnd;
        public Material Material;
        public int Length;

        public CustomSaberTrailData FromCustomSaber(GameObject gameobject)
        {
            var saberTrail = gameobject.GetComponent<CustomTrail>();

            if (!saberTrail) return default;

            var data = new CustomSaberTrailData
            {
                PointStart = saberTrail.PointStart,
                PointEnd = saberTrail.PointEnd,
                Material = saberTrail.TrailMaterial,
                Length = saberTrail.Length
            };

            return data;
        }
    }
}