using System;
using SaberFactory.Helpers;
using SaberFactory.Instances.Trail;
using SiraUtil.Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaberFactory.UI
{
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

        public float Width
        {
            set => SetWidth(value);
        }

        private readonly SiraLog _logger;
        private GameObject _prefab;

        private GameObject _instance;
        private Transform _transform;
        private Renderer _renderer;

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
            _instance = Object.Instantiate(_prefab, trailData.EndPos, Quaternion.Euler(-90, 0, 0), parent);
            _transform = _instance.transform;
            _renderer = _instance.GetComponentInChildren<Renderer>();

            Material = trailData.Material;
            Length = trailData.Length;
            Width = trailData.Width;
        }

        public void SetMaterial(Material mat)
        {
            if (!_renderer) return;
            _renderer.material = mat;
        }

        public Material GetMaterial()
        {
            if (!_renderer) return null;
            return _renderer.material;
        }

        public void SetLength(float val)
        {
            var currentScale = _transform.localScale;
            currentScale.x = val*0.1f;
            _transform.localScale = currentScale;
        }

        public void SetWidth(float val)
        {
            var currentScale = _transform.localScale;
            currentScale.z = val;
            _transform.localScale = currentScale;
        }

        public void Destroy()
        {
            _instance.TryDestroy();
        }
    }
}