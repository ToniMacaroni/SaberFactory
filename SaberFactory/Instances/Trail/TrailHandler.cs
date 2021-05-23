using IPA.Utilities;
using SaberFactory.Configuration;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.Instances.Trail
{
    /// <summary>
    /// Class for interfacing with the direct trail rendering implementation
    /// </summary>
    internal class TrailHandler : ITrailHandler
    {
        public SFTrail TrailInstance { get; protected set; }

        private const float _samplingStepMultiplier = 1.8f;

        protected InstanceTrailData _instanceTrailData;

        private readonly SaberTrail _backupTrail;
        private bool _canColorMaterial;

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
            if (_instanceTrailData is null)
            {

                if (_backupTrail is null) return;

                var trailStart = TrailInstance.gameObject.CreateGameObject("Trail Start");
                var trailEnd = TrailInstance.gameObject.CreateGameObject("TrailEnd");
                trailEnd.transform.localPosition = new Vector3(0, 0, 1);

                var trailRenderer = _backupTrail.GetField<SaberTrailRenderer, SaberTrail>("_trailRendererPrefab");

                var material = trailRenderer.GetField<MeshRenderer, SaberTrailRenderer>("_meshRenderer").material;

                var trailInitDataVanilla = new TrailInitData
                {
                    TrailColor = Color.white,
                    TrailLength = 15,
                    Whitestep = 0.02f,
                    Granularity = trailConfig.Granularity,
                    SamplingFrequency = trailConfig.SamplingFrequency,
                    SamplingStepMultiplier = _samplingStepMultiplier
                };

                TrailInstance.Setup(
                    trailInitDataVanilla,
                    material,
                    trailStart.transform,
                    trailEnd.transform
                );
                return;
            }

            var trailInitData = new TrailInitData
            {
                TrailColor = Color.white,
                TrailLength = _instanceTrailData.Length,
                Whitestep = _instanceTrailData.WhiteStep,
                Granularity = trailConfig.Granularity,
                SamplingFrequency = trailConfig.SamplingFrequency,
                SamplingStepMultiplier = _samplingStepMultiplier,
            };

            var (pointStart, pointEnd) = _instanceTrailData.GetPoints();

            TrailInstance.Setup(
                trailInitData,
                _instanceTrailData.Material.Material,
                pointStart,
                pointEnd
            );

            _canColorMaterial = IsMaterialColorable(_instanceTrailData.Material.Material);
        }

        public void DestroyTrail()
        {
            TrailInstance.TryDestoryImmediate();
        }

        public void SetTrailData(InstanceTrailData instanceTrailData)
        {
            _instanceTrailData = instanceTrailData;
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

        public void SetColor(Color color)
        {
            if (TrailInstance is {})
            {
                TrailInstance.Color = color;
            }

            if (_canColorMaterial)
            {
                _instanceTrailData.Material.Material.color = color;
            }
        }
    }
}