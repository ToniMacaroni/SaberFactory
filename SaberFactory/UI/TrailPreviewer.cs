using System;
using SaberFactory.Helpers;
using SaberFactory.Instances.Trail;
using SiraUtil.Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaberFactory.UI
{
    /// <summary>
    /// Class for previewing the saber trail in the editor
    /// </summary>
    internal class TrailPreviewer
    {
        public Material Material
        {
            get => GetMaterial();
            set => SetMaterial(value);
        }

        public float Length
        {
            set => SetLength(value);
        }

        private readonly SiraLog _logger;
        private GameObject _prefab;

        private GameObject _instance;
        private Transform _transform;
        private Renderer _renderer;
        private Mesh _mesh;

        private Transform _pointStart;
        private Transform _pointEnd;

        public TrailPreviewer(SiraLog logger, EmbeddedAssetLoader assetLoader)
        {
            _logger = logger;
            LoadPrefab(assetLoader);
        }

        private async void LoadPrefab(EmbeddedAssetLoader assetLoader)
        {
            try
            {
                _prefab = await assetLoader.LoadAsset<GameObject>("TrailPlane");
            }
            catch (Exception)
            {
                _logger.Error("Couldn't load trail plane");
            }
        }

        public void Create(Transform parent, InstanceTrailData trailData)
        {
            (_pointStart, _pointEnd) = trailData.GetPoints();

            _instance = Object.Instantiate(_prefab, trailData.PointEnd.position, Quaternion.Euler(-90, 25, 0), parent);
            _transform = _instance.transform;
            _renderer = _instance.GetComponentInChildren<Renderer>();
            _mesh = _instance.GetComponentInChildren<MeshFilter>().sharedMesh;

            Material = trailData.Material.Material;
            Length = trailData.Length;
            UpdateWidth();
        }

        public void SetMaterial(Material mat)
        {
            if (!_renderer) return;
            _renderer.material = mat;
        }

        public void SetColor(Color color)
        {
            var newColors = new Color[4];
            for (int i = 0; i < newColors.Length; i++)
            {
                newColors[i] = color;
            }

            _mesh.colors = newColors;
        }

        public void UpdateWidth()
        {
            var locPosStart = _instance.transform.InverseTransformPoint(_pointStart.position);
            var locPosEnd = _instance.transform.InverseTransformPoint(_pointEnd.position);

            var newVerts = new Vector3[4];
            newVerts[0] = new Vector3(0, 0, locPosStart.z); // bottom left
            newVerts[1] = new Vector3(0, 0, locPosEnd.z); // top left
            newVerts[2] = new Vector3(1f, 0, locPosEnd.z); // top right
            newVerts[3] = new Vector3(1f, 0, locPosStart.z); // bottom right
            _mesh.vertices = newVerts;
        }

        public Material GetMaterial()
        {
            if (!_renderer) return null;
            return _renderer.material;
        }

        public void SetLength(float val)
        {
            var currentScale = _transform.localScale;
            currentScale.x = val*0.05f;
            _transform.localScale = currentScale;
        }

        public void Destroy()
        {
            _instance.TryDestroy();
        }
    }
}