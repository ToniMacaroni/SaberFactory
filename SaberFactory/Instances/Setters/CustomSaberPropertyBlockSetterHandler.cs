using SaberFactory.Models.CustomSaber;
using SaberFactory.Models.PropHandler;
using UnityEngine;

namespace SaberFactory.Instances.Setters
{
    internal class CustomSaberPropertyBlockSetterHandler : PropertyBlockSetterHandler
    {
        public TransformDataSetter TransformDataSetter;

        public CustomSaberPropertyBlockSetterHandler(GameObject gameObject, CustomSaberModel model)
        {
            var propBlock = (CustomSaberPropertyBlock) model.PropertyBlock;
            TransformDataSetter = new TransformDataSetter(gameObject, propBlock.TransformProperty);
        }
    }
}