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
        private readonly PlayerTransforms _playerTransforms;
        private readonly SaberSettableSettings _saberSettableSettings;
        private bool _canColorMaterial;

        public CustomSaberTrailHandler(GameObject gameobject, CustomTrail customTrail, PlayerTransforms playerTransforms, SaberSettableSettings saberSettableSettings)
        {
            TrailInstance = gameobject.AddComponent<SFTrail>();
            _customTrail = customTrail;
            _playerTransforms = playerTransforms;
            _saberSettableSettings = saberSettableSettings;
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
            InitSettableSettings();

            if (!trailConfig.OnlyUseVertexColor)
            {
                _canColorMaterial = MaterialHelpers.IsMaterialColorable(_customTrail.TrailMaterial);
            }
        }
        
        private void UpdateRelativeMode()
        {
            TrailInstance.RelativeMode = _saberSettableSettings.RelativeTrailMode.Value;
        }

        private void InitSettableSettings()
        {
            if (_saberSettableSettings == null) return;

            UpdateRelativeMode();
            _saberSettableSettings.RelativeTrailMode.ValueChanged += UpdateRelativeMode;
        }

        private void UnInitSettableSettings()
        {
            if (_saberSettableSettings == null) return;

            _saberSettableSettings.RelativeTrailMode.ValueChanged -= UpdateRelativeMode;
        }

        public void DestroyTrail()
        {
            UnInitSettableSettings();
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