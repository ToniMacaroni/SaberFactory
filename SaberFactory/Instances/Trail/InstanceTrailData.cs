using System.Collections.Generic;
using System.Linq;
using CustomSaber;
using FlowUi.Helpers;
using HarmonyLib;
using Newtonsoft.Json;
using SaberFactory.AssetProperties;
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

        public IntProperty Length => TrailModel.Length;
        public FloatProperty Whitestep => TrailModel.Whitestep;
        public FloatProperty Offset => TrailModel.Offset;
        public FloatProperty Width => TrailModel.Width;
        public BoolProperty ClampTexture => TrailModel.ClampTexture;
        public BoolProperty Flip => TrailModel.Flip;

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

            SecondaryTrails = secondaryTrails?.Select(x => new SecondaryTrailHandler(x, trailModel.Length.DefaultValue)).ToList() ??
                              new List<SecondaryTrailHandler>();
            SecondaryTrails.Do(x => x.UpdateLength(trailModel.Length.Value));

            Init(trailModel);
        }

        private void Init(TrailModel trailModel)
        {
            trailModel.Length.RegisterHandler(nameof(InstanceTrailData), SetLength);
            trailModel.Offset.RegisterHandler(nameof(InstanceTrailData), val =>
            {
                var pos = trailModel.TrailPosOffset;
                pos.z = val;
                TrailModel.TrailPosOffset = pos;
                PointEnd.localPosition = pos;
                trailModel.Width.InvokeValueChange();
            });
            
            trailModel.Width.RegisterHandler(nameof(InstanceTrailData), SetWidth);
            trailModel.ClampTexture.RegisterHandler(nameof(InstanceTrailData), SetClampTexture);

            trailModel.Width.InvokeValueChange();
            trailModel.Offset.InvokeValueChange();
            trailModel.ClampTexture.InvokeValueChange();
        }

        private void SetWidth(float width)
        {
            var pos = PointStart.localPosition;
            pos.z = PointEnd.parent.localPosition.z - width;
            PointStart.localPosition = pos;
        }

        private void SetLength(int length)
        {
            SecondaryTrails.Do(x => x.UpdateLength(length));
        }

        private void SetClampTexture(bool shouldClampTexture)
        {
            if (TrailModel.OriginalTextureWrapMode.HasValue &&
                TrailModel.Material.IsValid &&
                TrailModel.Material.Material.TryGetMainTexture(out var tex))
            {
                tex.wrapMode = shouldClampTexture ? TextureWrapMode.Clamp : TrailModel.OriginalTextureWrapMode.GetValueOrDefault();
            }
        }

        public (Transform start, Transform end) GetPoints()
        {
            var pointStart = _isTrailReversed ? PointEnd : PointStart;
            var pointEnd = _isTrailReversed ? PointStart : PointEnd;

            return (Flip.Value ? pointEnd : pointStart, Flip.Value ? pointStart : pointEnd);
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