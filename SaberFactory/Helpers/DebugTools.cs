using UnityEngine;

namespace SaberFactory.Helpers
{
    public static class DebugTools
    {
        public static void LogError(object message)
        {
            Debug.LogError(message);
        }

        public static GameObject CreateBall(BallOptions options)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = options.Pos;
            go.transform.SetParent(options.Parent, false);
            go.transform.localScale = Vector3.one * (options.Size ?? 0.03f);
            if (options.Color.HasValue) go.GetComponent<Renderer>().material.color = options.Color.Value.ColorWithAlpha(0);
            return go;
        }

        public struct BallOptions
        {
            public Vector3 Pos;
            public float? Size;
            public Transform Parent;
            public Color? Color;
        }
    }
}