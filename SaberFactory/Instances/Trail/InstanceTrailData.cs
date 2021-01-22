using System;
using System.Runtime.InteropServices;
using CustomSaber;
using SaberFactory.Helpers;
using SaberFactory.Models;
using UnityEngine;

namespace SaberFactory.Instances.Trail
{
    internal class InstanceTrailData
    {
        public TrailModel TrailModel { get; }
        public Transform PointStart { get; }
        public Transform PointEnd { get; }

        public MaterialDescriptor Material => TrailModel.Material;
        public int Length => TrailModel.Length;
        public float WhiteStep => TrailModel.Whitestep;

        public float Width => Mathf.Abs(PointEnd.localPosition.z - PointStart.localPosition.z);

        public InstanceTrailData(TrailModel trailModel, Transform pointStart, Transform pointEnd)
        {
            TrailModel = trailModel;
            PointStart = pointStart;
            PointEnd = pointEnd;

            Init(trailModel);
        }

        public void Init(TrailModel trailModel)
        {
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

        public static (InstanceTrailData, TrailModel) FromCustomSaber(GameObject saberObject, TrailModel trailModel)
        {
            var saberTrail = saberObject.GetComponent<CustomTrail>();

            if (!saberTrail)
            {
                return default;
            }

            var model = trailModel ?? new TrailModel(
                Vector3.zero,
                saberTrail.GetWidth(),
                saberTrail.Length,
                new MaterialDescriptor(saberTrail.TrailMaterial),
                0);

            model.Material ??= new MaterialDescriptor(saberTrail.TrailMaterial);

            var data = new InstanceTrailData(model, saberTrail.PointStart, saberTrail.PointEnd);

            return (data, model);
        }
    }
}