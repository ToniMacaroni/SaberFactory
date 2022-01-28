using System.Collections.Generic;
using System.Linq;
using CustomSaber;
using SaberFactory.Instances;
using UnityEngine;

namespace SaberFactory.Helpers
{
    internal static class SaberHelpers
    {
        public static List<CustomTrail> GetTrails(GameObject saberObject)
        {
            if (saberObject is null)
            {
                return null;
            }

            return saberObject
                .GetComponentsInChildren<CustomTrail>()
                .Where(x => x.PointEnd != null && x.PointStart != null)
                .OrderByDescending(x => x.PointEnd.position.z)
                .ToList();
        }

        public static SaberInstance.SaberMonoBehaviour GetSaberMonoBehaviour(GameObject go)
        {
            return go.GetComponentInParent<SaberInstance.SaberMonoBehaviour>();
        }
    }
}