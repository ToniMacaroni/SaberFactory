using System;
using SaberFactory.Helpers;
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
            TypeValueRetriever valueRetriever = null;
            foreach (var component in gameobject.GetComponentsInChildren<Component>())
            {
                if (!component) continue;
                Type type = component.GetType();
                if (type.Name == "CustomTrail")
                {
                    valueRetriever = new TypeValueRetriever(type, component);
                    break;
                }
            }

            if (valueRetriever == null) return default;

            var data = new CustomSaberTrailData
            {
                PointStart = valueRetriever.GetValue<Transform>("PointStart"),
                PointEnd = valueRetriever.GetValue<Transform>("PointEnd"),
                Material = valueRetriever.GetValue<Material>("TrailMaterial"),
                Length = valueRetriever.GetValue<int>("Length")
            };

            return data;
        }
    }
}