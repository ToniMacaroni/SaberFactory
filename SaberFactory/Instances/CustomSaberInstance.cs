using SaberFactory.Models.CustomSaber;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace SaberFactory.Instances
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
            return GameObject.Instantiate(Model.Prefab);
        }
    }
}