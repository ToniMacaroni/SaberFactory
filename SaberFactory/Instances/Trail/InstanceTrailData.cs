using System.Collections.Generic;
using System.Linq;
using CustomSaber;
using HarmonyLib;
using SaberFactory.Helpers;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using UnityEngine;

namespace SaberFactory.Instances.Trail
{
    /// <summary>
    ///     Class for managing an instance of a trail and saving / loading from a trail model
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

        public float Offset
        {
            get => TrailModel.TrailPosOffset.z;
            set
            {
                var pos = TrailModel.TrailPosOffset;
                pos.z = value;
                TrailModel.TrailPosOffset = pos;
                PointEnd.localPosition = pos;
                Width = Width;
            }
        }

        public float Width
        {
            get => Mathf.Abs(PointEnd.parent.localPosition.z - PointStart.localPosition.z);
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

        public bool HasMultipleTrails => SecondaryTrails.Count > 0;
        public List<SecondaryTrailHandler> SecondaryTrails { get; }

        // is used for automatic trail reversal with faulty saber setup
        private readonly bool _isTrailReversed;

        public InstanceTrailData(TrailModel trailModel, Transform pointStart, Transform pointEnd, bool isTrailReversed,
            List<CustomTrail> secondaryTrails = null)
        {
            TrailModel = trailModel;
            PointStart = pointStart;

            var newEnd = new GameObject("PointEnd").transform;
            newEnd.SetParent(pointEnd, false);
            PointEnd = newEnd;

            _isTrailReversed = isTrailReversed;

            SecondaryTrails = secondaryTrails?.Select(x => new SecondaryTrailHandler(x, trailModel.OriginalLength)).ToList() ??
                              new List<SecondaryTrailHandler>();
            SecondaryTrails.Do(x => x.UpdateLength(trailModel.Length));

            Init(trailModel);
        }

        private void Init(TrailModel trailModel)
        {
            SetClampTexture(trailModel.ClampTexture);
            SetWidth(trailModel.Width);
            Offset = Offset;
        }

        public void SetWidth(float width)
        {
            TrailModel.Width = width;
            var pos = PointStart.localPosition;
            pos.z = PointEnd.parent.localPosition.z - width;
            PointStart.localPosition = pos;
        }

        public void SetLength(int length)
        {
            TrailModel.Length = length;
            SecondaryTrails.Do(x => x.UpdateLength(length));
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
                tex.wrapMode = shouldClampTexture ? TextureWrapMode.Clamp : TrailModel.OriginalTextureWrapMode.GetValueOrDefault();
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
            var pointEnd = _isTrailReversed ? PointStart : PointEnd;

            return (Flip ? pointEnd : pointStart, Flip ? pointStart : pointEnd);
        }

        internal class SecondaryTrailHandler
        {
            public CustomTrail Trail { get; }

            private readonly int _lengthOffset;

            public SecondaryTrailHandler(CustomTrail trail, int mainTrailLength)
            {
                Trail = trail;
                _lengthOffset = mainTrailLength - trail.Length;
            }

            public void UpdateLength(int newMainTrailLength)
            {
                Trail.Length = Mathf.Max(0, newMainTrailLength - _lengthOffset);
            }
        }
    }
}