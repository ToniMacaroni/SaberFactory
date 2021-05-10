using CustomSaber;
using SaberFactory.DataStore;
using SaberFactory.Helpers;
using SaberFactory.Instances;
using SaberFactory.Models.PropHandler;
using UnityEngine;
using Zenject;

namespace SaberFactory.Models.CustomSaber
{
    internal class CustomSaberModel : BasePieceModel
    {
        public TrailModel TrailModel
        {
            get
            {
                if (_trailModel == null)
                {
                    var trailModel = GrabTrail(false);
                    if (trailModel == null)
                    {
                        _hasTrail = false;
                        return null;
                    }

                    _trailModel = trailModel;
                }

                return _trailModel;
            }

            set => _trailModel = value;
        }

        public bool HasTrail
        {
            get
            {
                _hasTrail ??= Prefab != null && Prefab.GetComponent<CustomTrail>() != null;
                return _hasTrail.Value;
            }
        }

        public SaberDescriptor SaberDescriptor;

        private TrailModel _trailModel;
        private bool? _hasTrail;
        private bool _didReparentTrail;

        public CustomSaberModel(StoreAsset storeAsset) : base(storeAsset)
        {
            PropertyBlock = new CustomSaberPropertyBlock();
        }

        public override ModelMetaData GetMetaData()
        {
            return new ModelMetaData(SaberDescriptor.SaberName, SaberDescriptor.AuthorName,
                SaberDescriptor.CoverImage, false);
        }

        public override void SyncFrom(BasePieceModel otherModel)
        {
            base.SyncFrom(otherModel);
            var otherCs = (CustomSaberModel) otherModel;

            if (otherCs.HasTrail)
            {
                // backup current material
                var mat = TrailModel.Material.Material;

                TrailModel.CopyFrom(otherCs.TrailModel);
                if (string.IsNullOrWhiteSpace(TrailModel.TrailOrigin) ||
                    mat.shader.name == TrailModel.Material.Material.shader.name)
                {
                    // just copy props from other material to current
                    mat.CopyPropertiesFromMaterial(TrailModel.Material.Material);
                    TrailModel.Material.Material = mat;
                }
            }
        }

        public TrailModel GrabTrail(bool addTrailOrigin)
        {
            var trail = Prefab.GetComponent<CustomTrail>();

            if (trail == null) return null;

            TextureWrapMode wrapMode = default;
            if (trail.TrailMaterial != null && trail.TrailMaterial.TryGetMainTexture(out var tex))
            {
                wrapMode = tex.wrapMode;
            }

            FixTrailParents();

            return new TrailModel(
                Vector3.zero,
                trail.GetWidth(),
                trail.Length,
                new MaterialDescriptor(trail.TrailMaterial),
                0, wrapMode,
                addTrailOrigin ? StoreAsset.RelativePath : null);
        }

        /// <summary>
        /// Resets the trail using the original <see cref="CustomTrail"/> component
        /// </summary>
        public void ResetTrail()
        {
            TrailModel = GrabTrail(false);
        }

        /// <summary>
        /// Reparent trail transforms to specified parent
        /// so we don't have to care about scaling and rotations afterwards
        /// </summary>
        /// <param name="parent">Transform to parent the trail transforms to</param>
        /// <param name="trail"></param>
        public void FixTrailParents()
        {
            if (_didReparentTrail) return;
            _didReparentTrail = true;

            var trail = Prefab.GetComponent<CustomTrail>();

            if (trail is null) return;

            trail.PointStart.SetParent(Prefab.transform, true);
            trail.PointEnd.SetParent(Prefab.transform, true);
        }

        internal class Factory : PlaceholderFactory<StoreAsset, CustomSaberModel> {}
    }
}