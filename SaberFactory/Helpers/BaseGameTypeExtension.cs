using CustomSaber;
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

        public static void TryDestoryImmediate(this Object obj)
        {
            if (!obj) return;
            Object.DestroyImmediate(obj);
        }

        public static RectTransform AsRectTransform(this Transform transform)
        {
            return transform as RectTransform;
        }

        public static void SetCurve(GameObject root, float radius)
        {
            foreach (var curvedCanvasSettingse in root.GetComponentsInChildren<CurvedCanvasSettings>()) curvedCanvasSettingse.SetRadius(radius);
        }

        public static float GetTransfomWidth(Transform t1, Transform t2)
        {
            if (t1 == null || t2 == null) return 0;
            return Mathf.Abs(t1.localPosition.z - t2.localPosition.z);
        }

        public static float GetWidth(this CustomTrail trail)
        {
            if (trail == null) return 0;
            return GetTransfomWidth(trail.PointEnd, trail.PointStart);
        }

        public static void SetMaterial(this Renderer renderer, int index, Material material)
        {
            var mats = renderer.sharedMaterials;
            mats[index] = material;
            renderer.materials = mats;
        }

        public static Vector2 With(this Vector2 vec, float? x, float? y)
        {
            return new Vector2(x ?? vec.x, y ?? vec.y);
        }

        public static Vector3 With(this Vector3 vec, float? x, float? y, float? z)
        {
            return new Vector3(x ?? vec.x, y ?? vec.y, z ?? vec.z);
        }

        public static Color ColorFromArray(float[] arr)
        {
            return new Color(arr[0], arr[1], arr[2], arr[3]);
        }

        public static float[] ToArray(this Color clr)
        {
            return new[] { clr.r, clr.g, clr.b, clr.a };
        }

        public static float[] ToArray(this Vector2 vec)
        {
            return new[] { vec.x, vec.y };
        }

        public static float[] ToArray(this Vector3 vec)
        {
            return new[] { vec.x, vec.y, vec.z };
        }

        public static float[] ToArray(this Vector4 vec)
        {
            return new[] { vec.x, vec.y, vec.z, vec.w };
        }

        public static Vector2 ToVec2(this float[] fl)
        {
            return new Vector2(fl[0], fl[1]);
        }

        public static Vector3 ToVec3(this float[] fl)
        {
            return new Vector3(fl[0], fl[1], fl[2]);
        }

        public static Vector4 ToVec4(this float[] fl)
        {
            return new Vector4(fl[0], fl[1], fl[2], fl[3]);
        }
    }
}