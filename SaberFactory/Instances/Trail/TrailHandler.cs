using System;
using SaberFactory.Helpers;
using SaberFactory.Models.CustomSaber;
using UnityEngine;

namespace SaberFactory.Instances.Trail
{
    internal class TrailHandler
    {
        public SFTrail TrailInstance { get; protected set; }

        protected SaberTrailRenderer _trailRenderer;
        protected InstanceTrailData _instanceTrailData;

        public TrailHandler(GameObject gameobject)
        {
            TrailInstance = gameobject.AddComponent<SFTrail>();
        }

        public void CreateTrail()
        {
            var trailInitData = new SFTrail.TrailInitData
            {
                TrailColor = Color.white,
                TrailLength = _instanceTrailData.Length,
                TrailPrefab = _trailRenderer,
                Whitestep = _instanceTrailData.WhiteStep
            };

            TrailInstance.Setup(
                trailInitData,
                _instanceTrailData.Material,
                _instanceTrailData.PointStart,
                _instanceTrailData.PointEnd
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
            if (!TrailInstance) return;
            TrailInstance.Color = color;
        }
    }
}