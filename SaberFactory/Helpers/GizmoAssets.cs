using System.Threading.Tasks;
using SaberFactory.DataStore;
using SaberFactory.Gizmo;
using SaberFactory.Serialization;
using UnityEngine;
using Zenject;

namespace SaberFactory.Helpers
{
    internal class GizmoAssets : IInitializable
    {
        private readonly EmbeddedAssetLoader _assetLoader;
        private Material _gizmoMaterial;
        
        public Mesh PositionMesh { get; private set; }
        public Mesh RotationMesh { get; private set; }
        public Mesh ScalingMesh { get; private set; }
        
        public GizmoAssets(EmbeddedAssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public void Activate()
        {
            GizmoDrawer.Activate(_gizmoMaterial);
        }

        public void Deactivate()
        {
            GizmoDrawer.Deactivate();
        }

        public async void Initialize()
        {
            GizmoDrawer.Init();

            var shader = await _assetLoader.LoadAsset<Shader>("sh_sfglow_doublesided.shader");
            _gizmoMaterial = new Material(shader);

            PositionMesh = await LoadMesh("PositionGizmo");
            RotationMesh = await LoadMesh("RotationGizmo");
            ScalingMesh = await LoadMesh("ScalingGizmo");

            PositionGizmo.PositionMesh = PositionMesh;
            RotationGizmo.RotationMesh = RotationMesh;
            ScaleGizmo.ScalingMesh = ScalingMesh;

            GizmoDrawer.Init();
        }

        private async Task<Mesh> LoadMesh(string name)
        {
            return (await _assetLoader.LoadAsset<GameObject>(name)).GetComponentInChildren<MeshFilter>().sharedMesh;
        }
    }
}