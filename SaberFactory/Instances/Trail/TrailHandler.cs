using System;
using IPA.Utilities;
using SaberFactory.Configuration;
using SaberFactory.Helpers;
using SaberFactory.Models;
using UnityEngine;

namespace SaberFactory.Instances.Trail
{
    /// <summary>
    /// Class for interfacing with the direct trail rendering implementation
    /// </summary>
    internal class TrailHandler
    {
        public SFTrail TrailInstance { get; protected set; }

        protected SaberTrailRenderer _trailRenderer;
        protected InstanceTrailData _instanceTrailData;

        private readonly SaberTrail _backupTrail;

        public TrailHandler(GameObject gameobject)
        {
            TrailInstance = gameobject.AddComponent<SFTrail>();
        }

        public TrailHandler(GameObject gameobject, SaberTrail backupTrail) : this(gameobject)
        {
            _backupTrail = backupTrail;
        }

        public void CreateTrail(TrailConfig trailConfig)
        {
            if (_trailRenderer is null)
            {
                throw new ArgumentNullException(nameof(_trailRenderer), "Trail Renderer is not specified");
            }

            if (_instanceTrailData is null)
            {

                if (_backupTrail is null) return;

                var trailStart = TrailInstance.gameObject.CreateGameObject("Trail Start");
                var trailEnd = TrailInstance.gameObject.CreateGameObject("TrailEnd");
                trailEnd.transform.localPosition = new Vector3(0, 0, 1);

                var material = _trailRenderer.GetField<MeshRenderer, SaberTrailRenderer>("_meshRenderer").material;

                var trailInitDataVanilla = new SFTrail.TrailInitData
                {
                    TrailColor = Color.white,
                    TrailLength = 15,
                    TrailPrefab = _trailRenderer,
                    Whitestep = 0.02f,
                    Granularity = trailConfig.Granularity,
                    SamplingFrequency = trailConfig.SamplingFrequency
                };

                TrailInstance.Setup(
                    trailInitDataVanilla,
                    material,
                    trailStart.transform,
                    trailEnd.transform
                );
                return;
            }

            var trailInitData = new SFTrail.TrailInitData
            {
                TrailColor = Color.white,
                TrailLength = _instanceTrailData.Length,
                TrailPrefab = _trailRenderer,
                Whitestep = _instanceTrailData.WhiteStep,
                Granularity = trailConfig.Granularity,
                SamplingFrequency = trailConfig.SamplingFrequency
            };

            Transform pointStart = _instanceTrailData.IsTrailReversed
                ? _instanceTrailData.PointEnd
                : _instanceTrailData.PointStart;

            Transform pointEnd = _instanceTrailData.IsTrailReversed
                ? _instanceTrailData.PointStart
                : _instanceTrailData.PointEnd;

            TrailInstance.Setup(
                trailInitData,
                _instanceTrailData.Material.Material,
                pointStart,
                pointEnd
            );
        }

        public void DestroyTrail()
        {
            TrailInstance.TryDestroy();
        }

        public void SetPrefab(SaberTrailRenderer trailRenderer)
        {
            _trailRenderer = trailRenderer;
        }

        public void SetTrailData(InstanceTrailData instanceTrailData)
        {
            _instanceTrailData = instanceTrailData;
        }

        public void SetColor(Color color)
        {
            if (TrailInstance is {})
            {
                TrailInstance.Color = color;
            }
        }
    }
}