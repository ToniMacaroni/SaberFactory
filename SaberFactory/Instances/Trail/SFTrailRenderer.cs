using UnityEngine;

namespace SaberFactory.Instances.Trail
{
    internal class SFTrailRenderer : SaberTrailRenderer
    {
        public MeshRenderer MeshRenderer
        {
            get => _meshRenderer;
            set => _meshRenderer = value;
        }

        public MeshFilter MeshFilter
        {
            get => _meshFilter;
            set => _meshFilter = value;
        }

        public static SFTrailRenderer Create()
        {
            var go = new GameObject("SFTrailRenderer");
            var renderer = go.AddComponent<SFTrailRenderer>();
            renderer.MeshRenderer = go.AddComponent<MeshRenderer>();
            renderer.MeshFilter = go.AddComponent<MeshFilter>();
            return renderer;
        }

        public void SetMaterial(Material material)
        {
            _meshRenderer.material = material;
        }
    }
}