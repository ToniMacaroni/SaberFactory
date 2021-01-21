using HMUI;
using UnityEngine;

namespace SaberFactory.Helpers
{
    public static class BaseGameTypeExtension
    {
        public static bool IsLeft(this SaberType saberType)
        {
            return saberType == SaberType.SaberA;
        }

        public static GameObject CreateGameObject(this Transform parent, string name, bool worldPositionStays = false)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, worldPositionStays);
            return go;
        }

        public static GameObject CreateGameObject(this GameObject parent, string name, bool worldPositionStays = false)
        {
            return parent.transform.CreateGameObject(name, worldPositionStays);
        }

        public static void TryDestroy(this Object obj)
        {
            if (!obj) return;
            Object.Destroy(obj);
        }

        public static RectTransform AsRectTransform(this Transform transform)
        {
            return transform as RectTransform;
        }

        public static void SetCurve(GameObject root, float radius)
        {
            foreach (var curvedCanvasSettingse in root.GetComponentsInChildren<CurvedCanvasSettings>())
            {
                curvedCanvasSettingse.SetRadius(radius);
            }
        }
    }
}