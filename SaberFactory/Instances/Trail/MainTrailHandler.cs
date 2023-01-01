using IPA.Utilities;
using SaberFactory.Configuration;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.Instances.Trail
{
    internal class MainTrailHandler : ITrailHandler
    {
        public SFTrail TrailInstance { get; }

        protected InstanceTrailData InstanceTrailData;

        private readonly SaberTrail _backupTrail;
        private bool _canColorMaterial;

        public MainTrailHandler(GameObject gameobject)
        {
            TrailInstance = gameobject.AddComponent<SFTrail>();
        }

        public MainTrailHandler(GameObject gameobject, SaberTrail backupTrail) : this(gameobject)
        {
            _backupTrail = backupTrail;
        }

        public void CreateTrail(TrailConfig trailConfig, bool editor)
        {
            if (InstanceTrailData is null)
            {
                if (_backupTrail is null)
                {
                    return;
                }

                var trailStart = TrailInstance.gameObject.CreateGameObject("Trail StartNew");
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

            if (InstanceTrailData.Length.Value < 1)
            {
                return;
            }

            var trailInitData = new TrailInitData
            {
                TrailColor = Color.white,
                TrailLength = InstanceTrailData.Length.Value,
                Whitestep = InstanceTrailData.Whitestep.Value,
                Granularity = trailConfig.Granularity,
                SamplingFrequency = trailConfig.SamplingFrequency
            };

            var (pointStart, pointEnd) = InstanceTrailData.GetPoints();

            if (pointStart == null || pointEnd == null)
            {
                Debug.LogWarning("Primary trail on saber doesn't seem to have a positional transform");
                return;
            }

            TrailInstance.Setup(
                trailInitData,
                pointStart,
                pointEnd,
                InstanceTrailData.Material.Material,
                editor
            );

            if (!trailConfig.OnlyUseVertexColor)
            {
                _canColorMaterial = MaterialHelpers.IsMaterialColorable(InstanceTrailData.Material.Material);
            }
        }

        public void DestroyTrail(bool immediate = false)
        {
            if (immediate)
            {
                TrailInstance.TryDestoryImmediate();
            }
            else
            {
                TrailInstance.TryDestroy();
            }
        }

        public void SetTrailData(InstanceTrailData instanceTrailData)
        {
            InstanceTrailData = instanceTrailData;
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