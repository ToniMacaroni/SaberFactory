using UnityEngine;

namespace SaberFactory.Helpers
{
    internal static class CommonHelpers
    {
        public static SaberType ToSaberType(this ESaberSlot saberSlot)
        {
            return saberSlot == ESaberSlot.Left ? SaberType.SaberA : SaberType.SaberB;
        }

        public static void SetLayer(this GameObject obj, int layer)
        {
            if (obj == null)
            {
                return;
            }

            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                if (child == null)
                {
                    continue;
                }
                SetLayer(child.gameObject, layer);
            }
        }

        public static void SetLayer<T>(this GameObject obj, int layer) where T : Component
        {
            if (obj == null)
            {
                return;
            }

            foreach (var comp in obj.GetComponentsInChildren<T>())
            {
                comp.gameObject.layer = layer;
            }
        }

        public static T Cast<T>(this object obj)
        {
            return (T) obj;
        }

        public static T CastChecked<T>(this object obj)
        {
            if (obj is T ret) return ret;
            return default;
        }
    }
}