using IPA.Utilities;
using SaberFactory.Configuration;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.Instances.Trail
{
    internal class TrailHandlerEx : ITrailHandler
    {
        public AltTrail TrailInstance { get; protected set; }

        protected InstanceTrailData _instanceTrailData;

        private readonly SaberTrail _backupTrail;
        private bool _canColorMaterial;

        public TrailHandlerEx(GameObject gameobject)
        {
            TrailInstance = gameobject.AddComponent<AltTrail>();
        }

        public TrailHandlerEx(GameObject gameobject, SaberTrail backupTrail) : this(gameobject)
        {
            _backupTrail = backupTrail;
        }

        public void CreateTrail(TrailConfig trailConfig, bool editor)
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
                    Granularity = trailConfig.Granularity
                };

                TrailInstance.Setup(trailInitDataVanilla, trailStart.transform, trailEnd.transform, material, editor);
                return;
            }

            if (_instanceTrailData.Length < 1) return;

            var trailInitData = new TrailInitData
            {
                TrailColor = Color.white,
                TrailLength = _instanceTrailData.Length,
                Whitestep = _instanceTrailData.WhiteStep,
                Granularity = trailConfig.Granularity
            };

            var (pointStart, pointEnd) = _instanceTrailData.GetPoints();

            if (pointStart == null || pointEnd == null)
            {
                Debug.LogWarning("Primary trail on saber doesn't seem to have a positional transform");
                return;
            }

            TrailInstance.Setup(
                trailInitData,
                pointStart,
                pointEnd,
                _instanceTrailData.Material.Material,
                editor
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
            if (TrailInstance is { })
            {
                TrailInstance.MyColor = color;
            }

            if (_canColorMaterial)
            {
                _instanceTrailData.Material.Material.SetMainColor(color);
            }
        }
    }
}