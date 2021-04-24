using CustomSaber;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.Instances.Trail
{
    internal class CustomSaberTrailHandler
    {
        public AltTrail TrailInstance { get; protected set; }

        private readonly CustomTrail _customTrail;
        private bool _canColorMaterial;

        public CustomSaberTrailHandler(GameObject gameobject, CustomTrail customTrail)
        {
            TrailInstance = gameobject.AddComponent<AltTrail>();
            _customTrail = customTrail;
        }

        public void CreateTrail()
        {
            if (_customTrail.PointStart == null || _customTrail.PointEnd == null)
            {
                Debug.LogWarning("Secondary trail on saber doesn't seem to have a positional transform");
                return;
            }

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
                _customTrail.PointStart,
                _customTrail.PointEnd,
                _customTrail.TrailMaterial
            );

            _canColorMaterial = IsMaterialColorable(_customTrail.TrailMaterial);
        }

        private bool IsMaterialColorable(Material material)
        {
            if (material is null || !material.HasProperty(MaterialProperties.MainColor))
            {
                return false;
            }

            if (material.TryGetFloat(MaterialProperties.CustomColors, out var val))
            {
                if (val > 0)
                {
                    return true;
                }
            }
            else if (material.TryGetFloat(MaterialProperties.Glow, out val) && val > 0)
            {
                return true;
            }
            else if (material.TryGetFloat(MaterialProperties.Bloom, out val) && val > 0)
            {
                return true;
            }

            return false;
        }

        public void DestroyTrail()
        {
            TrailInstance.TryDestoryImmediate();
        }

        public void SetColor(Color color)
        {
            if (TrailInstance is { })
            {
                TrailInstance.MyColor = color;
            }

            if (_canColorMaterial)
            {
                _customTrail.TrailMaterial.SetMainColor(color);
            }
        }
    }
}