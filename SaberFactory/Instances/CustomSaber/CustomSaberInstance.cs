using SaberFactory.Instances.Setters;
using SaberFactory.Instances.Trail;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SiraUtil.Tools;
using UnityEngine;

namespace SaberFactory.Instances.CustomSaber
{
    internal class CustomSaberInstance : BasePieceInstance
    {
        private readonly SiraLog _logger;
        public InstanceTrailData InstanceTrailData { get; private set; }

        public CustomSaberInstance(CustomSaberModel model, SiraLog logger) : base(model)
        {
            _logger = logger;
            model.TrailModel = InitializeTrailData(GameObject, model.TrailModel);
        }

        public TrailModel InitializeTrailData(GameObject saberObject, TrailModel trailModel)
        {
            var (data, model) = InstanceTrailData.FromCustomSaber(saberObject, trailModel);
            if (data == null || model == null) return null;

            InstanceTrailData = data;

            return model;
        }

        public void ResetTrail()
        {
            var model = (CustomSaberModel) Model;
            model.TrailModel = InitializeTrailData(GameObject, null);
        }

        public override PartEvents GetEvents()
        {
            return PartEvents.FromCustomSaber(GameObject);
        }

        protected override GameObject Instantiate()
        {
            var instance = Object.Instantiate(GetSaberPrefab(), Vector3.zero, Quaternion.identity);
            instance.SetActive(true);

            PropertyBlockSetterHandler = new CustomSaberPropertyBlockSetterHandler(instance, Model as CustomSaberModel);
            return instance;
        }

        private GameObject GetSaberPrefab()
        {
            return Model.AdditionalInstanceHandler.GetSaber(Model.SaberSlot);
        }
    }
}