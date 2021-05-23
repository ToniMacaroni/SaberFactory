using System;
using CustomSaber;
using SaberFactory.Helpers;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using UnityEngine;

namespace SaberFactory.Instances.Trail
{
    /// <summary>
    /// Class for managing an instance of a trail and saving / loading from a trail model
    /// </summary>
    internal class InstanceTrailData
    {
        public TrailModel TrailModel { get; }
        public Transform PointStart { get; }
        public Transform PointEnd { get; }

        public MaterialDescriptor Material => TrailModel.Material;
        public int Length
        {
            get => TrailModel.Length;
            set => SetLength(value);
        }

        public float WhiteStep
        {
            get => TrailModel.Whitestep;
            set => SetWhitestep(value);
        }

        public float Width
        {
            get => Mathf.Abs(PointEnd.localPosition.z - PointStart.localPosition.z);
            set => SetWidth(value);
        }

        public bool ClampTexture
        {
            get => TrailModel.ClampTexture;
            set => SetClampTexture(value);
        }

        public bool Flip
        {
            get => TrailModel.Flip;
            set => TrailModel.Flip = value;
        }

        // is used for automatic trail reversal with faulty saber setup
        private readonly bool _isTrailReversed;

        public InstanceTrailData(TrailModel trailModel, Transform pointStart, Transform pointEnd, bool isTrailReversed)
        {
            TrailModel = trailModel;
            PointStart = pointStart;
            PointEnd = pointEnd;
            _isTrailReversed = isTrailReversed;

            Init(trailModel);
        }

        public void Init(TrailModel trailModel)
        {
            SetClampTexture(trailModel.ClampTexture);
            SetWidth(trailModel.Width);
        }

        public void SetWidth(float width)
        {
            TrailModel.Width = width;
            var pos = PointStart.localPosition;
            pos.z = PointEnd.localPosition.z - width;
            PointStart.localPosition = pos;
        }

        public void SetLength(int length)
        {
            TrailModel.Length = length;
        }

        public void SetWhitestep(float whitestep)
        {
            TrailModel.Whitestep = whitestep;
        }

        public void SetClampTexture(bool shouldClampTexture)
        {
            TrailModel.ClampTexture = shouldClampTexture;
            if (TrailModel.OriginalTextureWrapMode.HasValue &&
                TrailModel.Material.IsValid &&
                TrailModel.Material.Material.TryGetMainTexture(out var tex))
            {
                tex.wrapMode = shouldClampTexture ? TextureWrapMode.Clamp : TrailModel.OriginalTextureWrapMode.GetValueOrDefault();
            }
                
        }

        public void RevertMaterialForCustomSaber(CustomSaberModel saber)
        {
            TrailModel.Material.Revert();

            var saberTrail = saber.StoreAsset.Prefab.GetComponent<CustomTrail>();
            if (saberTrail == null) return;

            saberTrail.TrailMaterial = TrailModel.Material.Material;
        }

        public void RevertMaterial()
        {
            TrailModel.Material.Revert();
        }

        public (Transform start, Transform end) GetPoints()
        {
            var pointStart = _isTrailReversed ? PointEnd : PointStart;
            var pointEnd = _isTrailReversed ? pointStart : PointEnd;

            return (Flip ? pointEnd : pointStart, Flip ? pointStart : pointEnd);
        }
    }
}