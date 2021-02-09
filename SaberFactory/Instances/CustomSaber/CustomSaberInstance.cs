using System.Collections.Generic;
using SaberFactory.Helpers;
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
            InitializeTrailData(GameObject, model.TrailModel);
        }

        public void InitializeTrailData(GameObject saberObject, TrailModel trailModel)
        {
            InstanceTrailData = InstanceTrailData.FromCustomSaber(saberObject, trailModel);
        }

        public void ResetTrail()
        {
            var model = (CustomSaberModel) Model;
            model.TrailModel = null;
            InitializeTrailData(GameObject, null);
        }

        public override PartEvents GetEvents()
        {
            return PartEvents.FromCustomSaber(GameObject);
        }

        protected override void GetColorableMaterials(List<Material> materials)
        {
            foreach (Renderer renderer in GameObject.GetComponentsInChildren<Renderer>())
            {
                if (renderer is null)
                {
                    continue;
                }

                foreach (Material renderMaterial in renderer.materials)
                {
                    if (renderMaterial is null || !renderMaterial.HasProperty(MaterialProperties.MainColor))
                    {
                        continue;
                    }

                    if (renderMaterial.TryGetFloat(MaterialProperties.CustomColors, out var val))
                    {
                        if (val > 0)
                        {
                            materials.Add(renderMaterial);
                        }
                    }
                    else if (renderMaterial.TryGetFloat(MaterialProperties.Glow, out val) && val > 0)
                    {
                        materials.Add(renderMaterial);
                    }
                    else if (renderMaterial.TryGetFloat(MaterialProperties.Bloom, out val) && val > 0)
                    {
                        materials.Add(renderMaterial);
                    }
                }
            }
        }

        protected override GameObject Instantiate()
        {
            Model.Cast<CustomSaberModel>().FixTrailParents();
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