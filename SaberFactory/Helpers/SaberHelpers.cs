using System.Collections.Generic;
using System.Linq;
using CustomSaber;
using UnityEngine;

namespace SaberFactory.Helpers
{
    public static class SaberHelpers
    {
        public static List<CustomTrail> GetTrails(GameObject saberObject)
        {
            if (saberObject is null) return null;
            return saberObject
                .GetComponentsInChildren<CustomTrail>()
                .Where(x => x.PointEnd != null && x.PointStart != null)
                .OrderByDescending(x => x.PointEnd.position.z)
                .ToList();
        }
    }
}