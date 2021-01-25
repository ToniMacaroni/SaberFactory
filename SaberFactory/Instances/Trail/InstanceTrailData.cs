using System;
using CustomSaber;
using SaberFactory.Helpers;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using UnityEngine;

namespace SaberFactory.Instances.Trail
{
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

        public InstanceTrailData(TrailModel trailModel, Transform pointStart, Transform pointEnd)
        {
            TrailModel = trailModel;
            PointStart = pointStart;
            PointEnd = pointEnd;

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
            if (TrailModel.OriginalTextureWrapMode.HasValue)
            {
                TrailModel.Material.Material.mainTexture.wrapMode =
                    shouldClampTexture ? TextureWrapMode.Clamp : TrailModel.OriginalTextureWrapMode.Value;
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
                0, saberTrail.TrailMaterial.mainTexture?.wrapMode);

            if (model.Material == null)
            {
                model.Material = new MaterialDescriptor(saberTrail.TrailMaterial);
                model.OriginalTextureWrapMode = model.Material.Material.mainTexture?.wrapMode;
            }

            Transform pointStart = saberTrail.PointStart;
            Transform pointEnd = saberTrail.PointEnd;

            // Correction for sabers that have the transforms set the other way around
            if (pointStart.localPosition.z > pointEnd.localPosition.z)
            {
                pointStart = saberTrail.PointEnd;
                pointEnd = saberTrail.PointStart;
            }

            var data = new InstanceTrailData(model, pointStart, pointEnd);

            return (data, model);
        }
    }
}