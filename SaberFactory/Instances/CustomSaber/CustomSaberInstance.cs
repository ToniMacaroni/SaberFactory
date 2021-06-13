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

        public List<CustomTrail> SecondaryTrails;

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
            if (trailModel is null) return;

            var trails = saberObject?.GetComponentsInChildren<CustomTrail>();

            if (trails is null || trails.Length < 1) return;

            var saberTrail = trails[0];

            if (trails.Length > 1)
            {
                SecondaryTrails = new List<CustomTrail>();
                for (int i = 1; i < trails.Length; i++)
                {
                    SecondaryTrails.Add(trails[i]);
                }
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
            void AddMaterial(Renderer renderer, Material[] rendererMaterials, int index)
            {
                rendererMaterials[index] = new Material(rendererMaterials[index]);
                renderer.sharedMaterials = rendererMaterials;
                materials.Add(rendererMaterials[index]);
            }

            foreach (Renderer renderer in GameObject.GetComponentsInChildren<Renderer>())
            {
                if (renderer is null)
                {
                    continue;
                }

                var rendererMaterials = renderer.sharedMaterials;
                var materialCount = rendererMaterials.Length;

                for (int i = 0; i < materialCount; i++)
                {
                    var material = rendererMaterials[i];

                    if (material is null ||
                        !material.HasProperty(MaterialProperties.MainColor))
                    {
                        continue;
                    }

                    // always color materials if "_CustomColors" is 1
                    // if "_CustomColors" is present but != 1 don't color the material at all
                    if (material.TryGetFloat(MaterialProperties.CustomColors, out var val))
                    {
                        if (val > 0)
                        {
                            AddMaterial(renderer, rendererMaterials, i);
                        }
                    }
                    //if "_CustomColors" isn't present check for glow > 1 and bloom > 1
                    else if (material.TryGetFloat(MaterialProperties.Glow, out val) && val > 0)
                    {
                        AddMaterial(renderer, rendererMaterials, i);
                    }
                    else if (material.TryGetFloat(MaterialProperties.Bloom, out val) && val > 0)
                    {
                        AddMaterial(renderer, rendererMaterials, i);
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