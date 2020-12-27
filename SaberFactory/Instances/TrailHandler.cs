using UnityEngine;

namespace SaberFactory.Instances
{
    internal abstract class TrailHandler
    {
        public SFTrail TrailInstance { get; protected set; }

        protected SaberTrailRenderer _trailRenderer;
        protected TrailConstructionData _trailConstructionData;

        protected TrailHandler(GameObject gameobject)
        {
            TrailInstance = gameobject.AddComponent<SFTrail>();
        }

        public abstract void CreateTrail();

        public void SetPrefab(SaberTrailRenderer trailRenderer)
        {
            _trailRenderer = trailRenderer;
        }

        public void SetConstructionData(TrailConstructionData trailConstructionData)
        {
            _trailConstructionData = trailConstructionData;
        }

        public void SetColor(Color color)
        {
            TrailInstance.Color = color;
        }
    }
}