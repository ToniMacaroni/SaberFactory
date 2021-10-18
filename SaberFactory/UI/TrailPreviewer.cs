using System;
using System.Collections.Generic;
using HarmonyLib;
using SaberFactory.Helpers;
using SaberFactory.Instances.Trail;
using SiraUtil.Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaberFactory.UI
{
    /// <summary>
    ///     Class for previewing the saber trail in the editor
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

        public bool OnlyColorVertex
        {
            set { _sections.Do(x => x.OnlyColorVertex = value); }
        }

        private readonly SiraLog _logger;

        private readonly List<TrailPreviewSection> _sections = new List<TrailPreviewSection>();
        private GameObject _prefab;

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

        public void Create(Transform parent, InstanceTrailData trailData, bool onlyColorVertex)
        {
            _sections.Clear();
            var (pointStart, pointEnd) = trailData.GetPoints();
            _sections.Add(new TrailPreviewSection(0, parent, pointStart, pointEnd, _prefab) { OnlyColorVertex = onlyColorVertex });

            for (var i = 0; i < trailData.SecondaryTrails.Count; i++)
            {
                var trail = trailData.SecondaryTrails[i];
                if (trail.Trail.PointStart is null || trail.Trail.PointEnd is null) continue;
                _sections.Add(new TrailPreviewSection(i + 1, parent, trail.Trail.PointStart, trail.Trail.PointEnd, _prefab, trail)
                    { OnlyColorVertex = onlyColorVertex });
            }


            Material = trailData.Material.Material;
            Length = trailData.Length;
            UpdateWidth();
        }

        public void SetMaterial(Material mat)
        {
            if (_sections.Count < 1) return;
            _sections[0].SetMaterial(mat);
        }

        public void SetColor(Color color)
        {
            _sections.Do(x => x.SetColor(color));
        }

        public void UpdateWidth()
        {
            _sections.Do(x => x.UpdateWidth());
        }

        public Material GetMaterial()
        {
            if (_sections.Count < 1) return null;
            return _sections[0].GetMaterial();
        }

        public void SetLength(float val)
        {
            _sections.Do(x => x.SetLength(val));
        }

        public void Destroy()
        {
            _sections.Do(x => x.Destroy());
            _sections.Clear();
        }

        private class TrailPreviewSection
        {
            public int TrailIdx { get; }
            public bool IsPrimaryTrail => TrailIdx == 0;
            public bool OnlyColorVertex;

            private readonly GameObject _instance;
            private readonly Mesh _mesh;
            private readonly Transform _pointEnd;

            private readonly Transform _pointStart;
            private readonly Renderer _renderer;

            private readonly InstanceTrailData.SecondaryTrailHandler _trailHandler;
            private readonly Transform _transform;

            public TrailPreviewSection(
                int idx,
                Transform parent,
                Transform pointStart,
                Transform pointEnd,
                GameObject prefab,
                InstanceTrailData.SecondaryTrailHandler trailHandler = null)
            {
                TrailIdx = idx;

                _trailHandler = trailHandler;
                _pointStart = pointStart;
                _pointEnd = pointEnd;

                _instance = Object.Instantiate(prefab, _pointEnd.position, Quaternion.Euler(-90, 25, 0), parent);
                _instance.name = "Trail preview " + idx;
                _transform = _instance.transform;
                _renderer = _instance.GetComponentInChildren<Renderer>();
                _mesh = _instance.GetComponentInChildren<MeshFilter>().sharedMesh;
                _renderer.sortingOrder = 3;

                if (trailHandler is { }) SetMaterial(trailHandler.Trail.TrailMaterial);
            }

            public void SetMaterial(Material mat)
            {
                if (_renderer is null) return;
                _renderer.sharedMaterial = mat;
            }

            public Material GetMaterial()
            {
                return _renderer?.sharedMaterial;
            }

            public void SetColor(Color color)
            {
                if (_renderer.sharedMaterial is { })
                {
                    var mat = _renderer.sharedMaterial;
                    if (mat.HasCustomColorsEnabled() && !OnlyColorVertex) mat.SetMainColor(color);
                }

                var newColors = new Color[4];
                for (var i = 0; i < newColors.Length; i++) newColors[i] = color;

                _mesh.colors = newColors;
            }

            public void UpdateWidth()
            {
                var locPosStart = _instance.transform.InverseTransformPoint(_pointStart.position);
                var locPosEnd = _instance.transform.InverseTransformPoint(_pointEnd.position);

                var newVerts = new Vector3[4];
                newVerts[0] = new Vector3(0, 0, locPosStart.z); // bottom left
                newVerts[1] = new Vector3(0, 0, locPosEnd.z); // top left
                newVerts[2] = new Vector3(1, 0, locPosEnd.z); // top right
                newVerts[3] = new Vector3(1, 0, locPosStart.z); // bottom right
                _mesh.vertices = newVerts;
            }

            public void SetLength(float val)
            {
                if (_trailHandler is null)
                {
                    SetLengthInternal(val);
                    return;
                }

                _trailHandler.UpdateLength((int)val);
                SetLengthInternal(_trailHandler.Trail.Length);
            }

            private void SetLengthInternal(float val)
            {
                var currentScale = _transform.localScale;
                currentScale.x = val * 0.05f;
                _transform.localScale = currentScale;
            }

            public void Destroy()
            {
                _instance.TryDestroy();
            }
        }
    }
}