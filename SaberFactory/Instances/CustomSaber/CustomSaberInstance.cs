using System.Collections.Generic;
using CustomSaber;
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

        /// <summary>
        /// Creates an <see cref="InstanceTrailData"/> object
        /// with the correct trail transforms
        /// </summary>
        /// <param name="saberObject">The saber gameobject that the <see cref="CustomTrail"/> component is on</param>
        /// <param name="trailModel">The <see cref="TrailModel"/> to use</param>
        public void InitializeTrailData(GameObject saberObject, TrailModel trailModel)
        {
            var saberTrail = saberObject?.GetComponent<CustomTrail>();

            if (saberTrail is null || trailModel is null)
            {
                return;
            }

            // if trail comes from the preset save system
            // the model comes without the material assigned
            // so we assign 
            if (trailModel.Material is null)
            {
                trailModel.Material = new MaterialDescriptor(saberTrail.TrailMaterial);

                // set texture wrap mode
                if (trailModel.Material.Material.TryGetMainTexture(out var tex))
                {
                    trailModel.OriginalTextureWrapMode = tex.wrapMode;
                }
            }

            Transform pointStart = saberTrail.PointStart;
            Transform pointEnd = saberTrail.PointEnd;

            // Correction for sabers that have the transforms set up the other way around
            bool isTrailReversed = pointStart.localPosition.z > pointEnd.localPosition.z;

            if (isTrailReversed)
            {
                pointStart = saberTrail.PointEnd;
                pointEnd = saberTrail.PointStart;
            }

            InstanceTrailData = new InstanceTrailData(trailModel, pointStart, pointEnd, isTrailReversed);
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