using UnityEngine;

namespace SaberFactory.Instances
{
    internal class CustomSaberTrailHandler : TrailHandler
    {
        public CustomSaberTrailHandler(GameObject gameobject) : base(gameobject)
        {

        }

        public override void CreateTrail()
        {
            var trailInitData = new SFTrail.TrailInitData
            {
                TrailColor = new Color(0.7f, 0.7f, 0.7f),
                TrailLength = 20,
                TrailPrefab = _trailRenderer,
                Whitestep = 0
            };
            TrailInstance.Setup(trailInitData, _trailConstructionData.Material, _trailConstructionData.BottomTransform,
                _trailConstructionData.TopTransform);
        }
    }
}