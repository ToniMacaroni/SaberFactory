using CustomSaber;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.Instances.Trail
{
    internal class CustomSaberTrailHandler
    {
        public SFTrail TrailInstance { get; protected set; }

        private readonly CustomTrail _customTrail;

        public CustomSaberTrailHandler(GameObject gameobject, CustomTrail customTrail)
        {
            TrailInstance = gameobject.AddComponent<SFTrail>();
            _customTrail = customTrail;
        }

        public void CreateTrail()
        {
            var trailInitData = new TrailInitData
            {
                TrailColor = Color.white,
                TrailLength = _customTrail.Length,
                Whitestep = 0,
                Granularity = 60,
                SamplingFrequency = 80,
                SamplingStepMultiplier = 1,
            };

            TrailInstance.Setup(
                trailInitData,
                _customTrail.TrailMaterial,
                _customTrail.PointStart,
                _customTrail.PointEnd
            );
        }

        public void DestroyTrail()
        {
            TrailInstance.TryDestoryImmediate();
        }

        public void SetColor(Color color)
        {
            if (TrailInstance is { })
            {
                TrailInstance.Color = color;
            }
        }
    }
}