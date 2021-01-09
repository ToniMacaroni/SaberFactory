using SaberFactory.Helpers;
using SaberFactory.Instances.Trail;
using UnityEngine;

namespace SaberFactory.Instances.CustomSaber
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
                TrailLength = _trailConstructionData.Length,
                TrailPrefab = _trailRenderer,
                Whitestep = _trailConstructionData.Whitestep
            };
            TrailInstance.Setup(trailInitData, _trailConstructionData.Material, _trailConstructionData.BottomTransform,
                _trailConstructionData.TopTransform);
        }

        public override void DestroyTrail()
        {
            TrailInstance.TryDestroy();
        }
    }
}