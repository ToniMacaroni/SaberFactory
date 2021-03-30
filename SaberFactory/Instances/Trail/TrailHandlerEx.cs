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

        public TrailHandlerEx(GameObject gameobject)
        {
            TrailInstance = gameobject.AddComponent<AltTrail>();
        }

        public TrailHandlerEx(GameObject gameobject, SaberTrail backupTrail) : this(gameobject)
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
                    Granularity = trailConfig.Granularity
                };

                TrailInstance.Setup(trailInitDataVanilla, trailStart.transform, trailEnd.transform, material);
                return;
            }

            var trailInitData = new TrailInitData
            {
                TrailColor = Color.white,
                TrailLength = _instanceTrailData.Length,
                Whitestep = _instanceTrailData.WhiteStep,
                Granularity = trailConfig.Granularity
            };

            Transform pointStart = _instanceTrailData.IsTrailReversed
                ? _instanceTrailData.PointEnd
                : _instanceTrailData.PointStart;

            Transform pointEnd = _instanceTrailData.IsTrailReversed
                ? _instanceTrailData.PointStart
                : _instanceTrailData.PointEnd;

            TrailInstance.Setup(
                trailInitData,
                pointStart,
                pointEnd,
                _instanceTrailData.Material.Material
            );
        }

        public void DestroyTrail()
        {
            TrailInstance.TryDestoryImmediate();
        }

        public void SetTrailData(InstanceTrailData instanceTrailData)
        {
            _instanceTrailData = instanceTrailData;
        }

        public void SetColor(Color color)
        {
            if (TrailInstance is { })
            {
                TrailInstance.MyColor = color;
            }
        }
    }
}