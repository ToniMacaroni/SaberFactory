using SaberFactory.Helpers;
using SaberFactory.Models.CustomSaber;
using UnityEngine;

namespace SaberFactory.Instances.Trail
{
    internal class TrailHandler
    {
        public SFTrail TrailInstance { get; protected set; }

        protected SaberTrailRenderer _trailRenderer;
        protected InstanceTrailData _InstanceTrailData;

        public TrailHandler(GameObject gameobject)
        {
            TrailInstance = gameobject.AddComponent<SFTrail>();
        }

        public void CreateTrail()
        {
            var trailInitData = new SFTrail.TrailInitData
            {
                TrailColor = Color.white,
                TrailLength = _InstanceTrailData.Length,
                TrailPrefab = _trailRenderer,
                Whitestep = _InstanceTrailData.WhiteStep
            };

            var (pointStart, pointEnd) = _InstanceTrailData.CopyPoints();

            TrailInstance.Setup(
                trailInitData,
                _InstanceTrailData.Material,
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
            _InstanceTrailData = instanceTrailData;
        }

        public void SetColor(Color color)
        {
            if (!TrailInstance) return;
            TrailInstance.Color = color;
        }
    }
}