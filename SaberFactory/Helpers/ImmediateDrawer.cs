using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaberFactory.Helpers
{
    internal class ImmediateDrawer
    {
        public bool IsInitialized { get; private set; }
        
        public ImmediateDrawer()
        {
            var prim = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _ballmesh = prim.GetComponent<MeshFilter>().mesh;
            Object.Destroy(prim);
            
            var shader = FindShader();
            if (!shader)
            {
                return;
            }
            
            _mat = new Material(shader);
            IsInitialized = true;
        }
        
        public void DrawBall(Vector3 pos, float size, Color color)
        {
            _mat.color = color;
            _mat.SetPass(0);
            Graphics.DrawMesh(_ballmesh, Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one * size), _mat, 0);
        }

        public void DrawSmallBall(Vector3 pos, Color? color = null)
        {
            if(color == null)
                color = Color.red;
            DrawBall(pos, 0.05f, color.Value);
        }

        private Shader FindShader()
        {
            var possibleShaders = new string[]
            {
                "BeatSaber/Unlit Glow",
                "Standard",
            };

            return Resources.FindObjectsOfTypeAll<Shader>().FirstOrDefault(x=> possibleShaders.Contains(x.name));
        }

        private Mesh _ballmesh;
        private Material _mat;
    }
}