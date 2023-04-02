using CustomSaber;
using SaberFactory.Configuration;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.Instances.Trail
{
    /// <summary>
    /// Used for secondary trails
    /// </summary>
    internal class CustomSaberTrailHandler
    {
        public SFTrail TrailInstance { get; protected set; }

        private readonly CustomTrail _customTrail;
        private bool _canColorMaterial;
        private PlayerTransforms _playerTransforms;

        public CustomSaberTrailHandler(GameObject gameobject, CustomTrail customTrail, PlayerTransforms playerTransforms)
        {
            TrailInstance = gameobject.AddComponent<SFTrail>();
            _customTrail = customTrail;
            _playerTransforms = playerTransforms;
        }

        public void CreateTrail(TrailConfig trailConfig, bool editor)
        {
            if (_customTrail.PointStart == null || _customTrail.PointEnd == null)
            {
                Debug.LogWarning("Secondary trail on saber doesn't seem to have a positional transform");
                return;
            }

            if (_customTrail.Length < 1)
            {
                return;
            }

            var trailInitData = new TrailInitData
            {
                TrailColor = Color.white,
                TrailLength = _customTrail.Length,
                Whitestep = 0,
                Granularity = 60,
                SamplingFrequency = 80,
                SamplingStepMultiplier = 1
            };

            TrailInstance.Setup(
                trailInitData,
                _customTrail.PointStart,
                _customTrail.PointEnd,
                _customTrail.TrailMaterial,
                editor
            );
            
            TrailInstance.PlayerTransforms = _playerTransforms;

            if (!trailConfig.OnlyUseVertexColor)
            {
                _canColorMaterial = MaterialHelpers.IsMaterialColorable(_customTrail.TrailMaterial);
            }
        }

        public void SetRelativeMode(bool active)
        {
            TrailInstance.RelativeMode = active;
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

            if (_canColorMaterial)
            {
                TrailInstance.SetMaterialBlock(MaterialHelpers.ColorBlock(color));
            }
        }
    }
}