using System;
using IPA.Utilities;
using SaberFactory.Helpers;
using SiraUtil;
using UnityEngine;

namespace SaberFactory.Instances.Trail
{
    /// <summary>
    /// Class for interfacing with the direct trail rendering implementation
    /// </summary>
    internal class TrailHandler
    {
        public SFTrail TrailInstance { get; protected set; }

        protected SaberTrailRenderer _trailRenderer;
        protected InstanceTrailData _instanceTrailData;

        private readonly SaberTrail _backupTrail;

        public TrailHandler(GameObject gameobject)
        {
            TrailInstance = gameobject.AddComponent<SFTrail>();
        }

        public TrailHandler(GameObject gameobject, SaberTrail backupTrail) : this(gameobject)
        {
            _backupTrail = backupTrail;
        }

        public void CreateTrail()
        {
            if (_trailRenderer is null)
            {
                throw new ArgumentNullException(nameof(_trailRenderer), "Trail Renderer is not specified");
            }

            if (_instanceTrailData is null)
            {

                if (_backupTrail is null) return;

                var trailStart = TrailInstance.gameObject.CreateGameObject("Trail Start");
                var trailEnd = TrailInstance.gameObject.CreateGameObject("TrailEnd");
                trailEnd.transform.localPosition = new Vector3(0, 0, 1);

                var material = _trailRenderer.GetField<MeshRenderer, SaberTrailRenderer>("_meshRenderer").material;

                var trailInitData = new SFTrail.TrailInitData
                {
                    TrailColor = Color.white,
                    TrailLength = 15,
                    TrailPrefab = _trailRenderer,
                    Whitestep = 0.02f
                };

                TrailInstance.Setup(
                    trailInitData,
                    material,
                    trailStart.transform,
                    trailEnd.transform
                );
            }
            else
            {
                var trailInitData = new SFTrail.TrailInitData
                {
                    TrailColor = Color.white,
                    TrailLength = _instanceTrailData.Length,
                    TrailPrefab = _trailRenderer,
                    Whitestep = _instanceTrailData.WhiteStep
                };

                TrailInstance.Setup(
                    trailInitData,
                    _instanceTrailData.Material.Material,
                    _instanceTrailData.PointStart,
                    _instanceTrailData.PointEnd
                );
            }
        }

        public void DestroyTrail()
        {
            TrailInstance.TryDestroy();
        }

        public void SetPrefab(SaberTrailRenderer trailRenderer)
        {
            _trailRenderer = trailRenderer;
        }

        public void SetTrailData(InstanceTrailData instanceTrailData)
        {
            _instanceTrailData = instanceTrailData;
        }

        public void SetColor(Color color)
        {
            if (TrailInstance is SFTrail sfTrail)
            {
                sfTrail.Color = color;
            }
            else
            {
                TrailInstance?.SetField("_color", color);
            }
        }
    }
}