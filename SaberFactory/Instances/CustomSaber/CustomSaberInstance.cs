using SaberFactory.Models.CustomSaber;
using SiraUtil.Tools;
using UnityEngine;

namespace SaberFactory.Instances.CustomSaber
{
    internal class CustomSaberInstance : BasePieceInstance
    {
        public readonly CustomSaberTrailData CustomSaberTrailData;

        public CustomSaberInstance(CustomSaberModel model, SiraLog logger) : base(model)
        {
            CustomSaberTrailData = CustomSaberTrailData.FromCustomSaber(GameObject);
        }

        protected override GameObject Instantiate()
        {
            return Object.Instantiate(Model.Prefab, Vector3.zero, Quaternion.identity);
        }
    }
}