using System;
using IPA.Utilities;
using UnityEngine;

namespace SaberFactory.Instances
{
    internal class SFTrail : SaberTrail
    {
        [SerializeField] private Material _customMaterial;

        [SerializeField]
        private Transform _end;

        [SerializeField]
        private Transform _start;

        [SerializeField]
        private float _trailWidth;

        [SerializeField] private int _trailLength;
        [SerializeField] private float _whitestep;

        [SerializeField] private SaberTrailRenderer _trailRendererCopy;

        public Color Color
        {
            get => _color;
            set => _color = value;
        }

        public override void Awake()
        {
            // Initialization For tricksaber
            if (_trailRenderer == null && _trailRendererCopy != null)
            {
                SetupProps();
                _trailRenderer = _trailRendererCopy = Instantiate(_trailRendererPrefab, Vector3.zero, Quaternion.identity);
            }
        }

        public void Setup(TrailInitData initData, Material trailMaterial, Transform start, Transform end)
        {
            Color = initData.TrailColor;
            _customMaterial = trailMaterial;
            _trailWidth = (end.position - start.position).magnitude;
            _start = start;
            _end = end;

            _trailLength = initData.TrailLength;
            _whitestep = initData.Whitestep;

            SetupProps();

            _trailRendererPrefab = initData.TrailPrefab;
            _trailRenderer = _trailRendererCopy = Instantiate(_trailRendererPrefab, Vector3.zero, Quaternion.identity);
        }

        private void SetupProps()
        {
            _trailDuration = _trailLength * 0.01f;
            _whiteSectionMaxDuration = _whitestep;
            _samplingFrequency = 80;
            _granularity = 60;
        }

        public override void OnEnable()
        {
            if (_inited)
            {
                _trailRenderer.UpdateMesh(this._trailElementCollection, this._color);
            }
            if (_trailRenderer)
            {
                _trailRenderer.enabled = true;
            }
        }

        public override void Init()
        {
            if (_trailRenderer == null) _trailRenderer = _trailRendererCopy;

            _sampleStep = 1f / _samplingFrequency;
            Vector3 bottomPos = _start.position;
            Vector3 topPos = _end.position;
            int capacity = Mathf.CeilToInt(_samplingFrequency * _trailDuration);
            _trailElementCollection = new TrailElementCollection(capacity, bottomPos, topPos, TimeHelper.time);
            float trailWidth = _trailWidth;
            _whiteSectionMaxDuration = Math.Min(_whiteSectionMaxDuration, _trailDuration);
            _lastZScale = transform.lossyScale.z;
            _trailRenderer.Init(trailWidth, _trailDuration, _granularity, _whiteSectionMaxDuration);
            _inited = true;
            SetMaterial(_customMaterial);
        }

        public override float GetTrailWidth(BladeMovementDataElement lastAddedData)
        {
            return _trailWidth;
        }

        public override void LateUpdate()
        {
            if (_framesPassed <= 4)
            {
                if (_framesPassed == 4)
                {
                    Init();
                }

                _framesPassed++;
                return;
            }

            var start = _start.position;
            var end = _end.position;

            int num = Mathf.RoundToInt((TimeHelper.time - _lastTrailElementTime) / _sampleStep);
            for (int i = 0; i < num; i++)
            {
                _lastTrailElementTime = TimeHelper.time;
                _trailElementCollection.MoveTailToHead();
                _trailElementCollection.head.SetData(start, end, _lastTrailElementTime);
            }

            _trailElementCollection.UpdateDistances();
            _trailRenderer.UpdateMesh(_trailElementCollection, _color);
        }

        public void SetMaterial(Material newMaterial)
        {
            _customMaterial = newMaterial;
            _trailRenderer.GetField<MeshRenderer, SaberTrailRenderer>("_meshRenderer").material = _customMaterial;
        }

        internal struct TrailInitData
        {
            public SaberTrailRenderer TrailPrefab;
            public int TrailLength;
            public float Whitestep;
            public Color TrailColor;
        }
    }
}