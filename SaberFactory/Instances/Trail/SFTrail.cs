using System;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.Instances.Trail
{
    /// <summary>
    ///     Class used for rendering the trail on the saber
    /// </summary>
    internal class SFTrail : SaberTrail
    {
        public Color Color
        {
            get => _color;
            set => _color = value;
        }

        [SerializeField] private Material _customMaterial;

        [SerializeField] private Transform _end;

        [SerializeField] private Transform _start;

        [SerializeField] private int _trailLength;

        [SerializeField] private float _trailWidth;
        [SerializeField] private float _whitestep;

        // Don't let the SaberTrail class instantiate from the renderer prefab (since it's null at this point)
        public override void Awake()
        {
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

            var num = Mathf.RoundToInt((TimeHelper.time - _lastTrailElementTime) / _sampleStep);
            for (var i = 0; i < num; i++)
            {
                _lastTrailElementTime = TimeHelper.time;
                _trailElementCollection.MoveTailToHead();
                _trailElementCollection.head.SetData(start, end, _lastTrailElementTime);
            }

            _trailElementCollection.UpdateDistances();
            _trailRenderer.UpdateMesh(_trailElementCollection, _color);
        }

        public override void OnEnable()
        {
            if (_inited)
            {
                _trailRenderer.UpdateMesh(_trailElementCollection, _color);
            }

            if (_trailRenderer)
            {
                _trailRenderer.enabled = true;
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

            _trailDuration = _trailLength * 0.01f;
            _whiteSectionMaxDuration = _whitestep;

            _granularity = initData.Granularity;
            _samplingFrequency = initData.SamplingFrequency;

            _trailRenderer = SFTrailRenderer.Create();
        }

        public override void Init()
        {
            _sampleStep = 1f / _samplingFrequency;
            var bottomPos = _start.position;
            var topPos = _end.position;
            var capacity = Mathf.CeilToInt(_samplingFrequency * _trailDuration);
            _trailElementCollection = new TrailElementCollection(capacity, bottomPos, topPos, TimeHelper.time);
            var trailWidth = _trailWidth;
            _whiteSectionMaxDuration = Math.Min(_whiteSectionMaxDuration, _trailDuration);
            _lastZScale = transform.lossyScale.z;
            _trailRenderer.Cast<SFTrailRenderer>().Init(trailWidth, _trailDuration, _granularity, _whiteSectionMaxDuration);
            _inited = true;

            SetMaterial(_customMaterial);
        }

        public override float GetTrailWidth(BladeMovementDataElement lastAddedData)
        {
            return _trailWidth;
        }

        public void SetMaterial(Material newMaterial)
        {
            _customMaterial = newMaterial;
            _trailRenderer?.Cast<SFTrailRenderer>().SetMaterial(_customMaterial);
        }
    }
}