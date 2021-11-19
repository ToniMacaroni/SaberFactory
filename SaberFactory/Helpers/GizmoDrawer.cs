using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaberFactory.Helpers
{
    public class GizmoDrawer : MonoBehaviour
    {
        public readonly struct DrawCommand
        {
            private readonly Mesh _mesh;
            private readonly Matrix4x4 _matrix;
            private readonly Color _color;

            public DrawCommand(Mesh mesh, Color color, Matrix4x4 matrix)
            {
                _mesh = mesh;
                _matrix = matrix;
                _color = color;
            }

            public void Draw(Material m)
            {
                m.color = _color;
                m.SetFloat(GlowId, _color.a);
                m.SetPass(0);
                Graphics.DrawMeshNow(_mesh, _matrix);
            }
        }

        private static readonly List<GizmoDrawer> _drawers = new List<GizmoDrawer>();
        private static Dictionary<PrimitiveType, Mesh> _meshes;

        private static bool _initd;
        private static bool _active;

        public static void Init()
        {
            if (_initd)
            {
                return;
            }

            _meshes = new Dictionary<PrimitiveType, Mesh>();
            foreach (var pt in (PrimitiveType[])Enum.GetValues(typeof(PrimitiveType)))
            {
                var go = GameObject.CreatePrimitive(pt);
                var m = go.GetComponent<MeshFilter>().sharedMesh;
                DestroyImmediate(go);
                _meshes.Add(pt, m);
            }

            _initd = true;
        }

        public static void Activate(Material material)
        {
            if (_active || !_initd)
            {
                return;
            }

            foreach (var cam in FindObjectsOfType<Camera>())
            {
                AddDrawer(cam, material);
            }

            _active = true;
        }

        public static void Deactivate()
        {
            if (!_active)
            {
                return;
            }

            foreach (var drawer in _drawers)
            {
                Destroy(drawer);
            }

            _active = false;
        }

        private static GizmoDrawer AddDrawer(Camera cam, Material material)
        {
            if (cam == null)
            {
                return null;
            }

            var drawer = cam.gameObject.AddComponent<GizmoDrawer>();
            drawer.InitDrawer(material);
            _drawers.Add(drawer);
            return drawer;
        }

        public static void Draw(DrawCommand command)
        {
            if (!active || !Application.isPlaying)
            {
                return;
            }

            if (_drawers.Count == 0)
            {
                return;
            }

            foreach (var d in _drawers)
            {
                d._cmds.Add(command);
            }
        }

        public static void Draw(Mesh mesh, Color color, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Draw(new DrawCommand(mesh, color, Matrix4x4.TRS(position, rotation, scale)));
        }

        private static void Draw(PrimitiveType primitiveType, Color color, Matrix4x4 matrix)
        {
            Draw(new DrawCommand(_meshes[primitiveType], color, matrix));
        }

        public static bool active = true;

        public static void DrawSphere(Vector3 position, float radius, Color color)
        {
            Draw(
                PrimitiveType.Sphere, color,
                Matrix4x4.TRS(position, Quaternion.identity, Vector3.one * radius)
            );
        }

        public static void DrawBox(Vector3 position, Quaternion rotation, Vector3 size, Color color)
        {
            Draw(PrimitiveType.Cube, color, Matrix4x4.TRS(position, rotation, size));
        }
        
        private Material _mat;
        private readonly List<DrawCommand> _cmds = new List<DrawCommand>();
        private static readonly int GlowId = Shader.PropertyToID("_Glow");

        public void InitDrawer(Material material)
        {
            _mat = material;
        }

        private void OnDestroy()
        {
            _drawers.Remove(this);
        }

        private void OnPostRender()
        {
            if (_mat == null)
            {
                _cmds.Clear();
                return;
            }

            foreach (var c in _cmds)
            {
                c.Draw(_mat);
            }

            _cmds.Clear();
        }
    }
}