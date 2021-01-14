using SaberFactory.Instances.Trail;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SiraUtil.Tools;
using UnityEngine;

namespace SaberFactory.Instances.CustomSaber
{
    internal class CustomSaberInstance : BasePieceInstance
    {
        private InstanceTrailData _instanceTrailData;
        public Transform BladeEnd { get; private set; }

        public CustomSaberInstance(CustomSaberModel model, SiraLog logger) : base(model)
        {
            InitializeTrailData(GameObject, model.TrailModel);
        }

        public void InitializeTrailData(GameObject saberObject, TrailModel? trailModel)
        {
            var trailData = InstanceTrailData.FromCustomSaber(saberObject);
            if (trailModel.HasValue)
            {
                var tModel = trailModel.Value;
                trailData.Length += tModel.TrailLengthOffset;
                trailData.PointStart.localPosition += new Vector3(0, 0, tModel.TrailWidthOffset);
                trailData.WhiteStep = tModel.Whitestep;
            }

            BladeEnd = trailData.PointEnd;
            _instanceTrailData = trailData;
        }

        public InstanceTrailData GetInstanceTrailData(bool applyModel)
        {
            if (!applyModel) return _instanceTrailData;
            _instanceTrailData.ApplyModel((Model as CustomSaberModel).TrailModel);
            return _instanceTrailData;
        }

        protected override GameObject Instantiate()
        {
            return Object.Instantiate(Model.Prefab, Vector3.zero, Quaternion.identity);
        }
    }
}