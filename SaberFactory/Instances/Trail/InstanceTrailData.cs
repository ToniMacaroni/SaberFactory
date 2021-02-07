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

        public static InstanceTrailData FromCustomSaber(GameObject saberObject, TrailModel trailModel)
        {
            var saberTrail = saberObject.GetComponent<CustomTrail>();

            if (!saberTrail || trailModel == null)
            {
                return null;
            }

            // if trail comes from the preset save system
            // the model comes without the material assigned
            if (trailModel.Material == null)
            {
                trailModel.Material = new MaterialDescriptor(saberTrail.TrailMaterial);
                if (trailModel.Material.Material.TryGetMainTexture(out var tex))
                {
                    trailModel.OriginalTextureWrapMode = tex.wrapMode;
                }
            }

            Transform pointStart = saberTrail.PointStart;
            Transform pointEnd = saberTrail.PointEnd;

            // Correction for sabers that have the transforms set up the other way around
            if (pointStart.localPosition.z > pointEnd.localPosition.z)
            {
                pointStart = saberTrail.PointEnd;
                pointEnd = saberTrail.PointStart;
            }

            var data = new InstanceTrailData(trailModel, pointStart, pointEnd);

            return data;
        }
    }
}