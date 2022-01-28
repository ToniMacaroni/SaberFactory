using System.Collections.Generic;
using CustomSaber;
using HarmonyLib;
using SaberFactory.Helpers;
using SaberFactory.Instances.PostProcessors;
using SaberFactory.Instances.Setters;
using SaberFactory.Instances.Trail;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SiraUtil.Logging;
using UnityEngine;

namespace SaberFactory.Instances.CustomSaber
{
    internal class CustomSaberInstance : BasePieceInstance
    {
        public InstanceTrailData InstanceTrailData { get; private set; }
        private readonly SiraLog _logger;
        private readonly List<IPartPostProcessor> _postProcessors;

        public CustomSaberInstance(CustomSaberModel model, SiraLog logger, List<IPartPostProcessor> postProcessors) : base(model)
        {
            _logger = logger;
            _postProcessors = postProcessors;
            InitializeTrailData(GameObject, model.TrailModel);
        }

        /// <summary>
        ///     Creates an <see cref="InstanceTrailData" /> object
        ///     with the correct trail transforms
        /// </summary>
        /// <param name="saberObject">The saber gameobject that the <see cref="CustomTrail" /> component is on</param>
        /// <param name="trailModel">The <see cref="TrailModel" /> to use</param>
        public void InitializeTrailData(GameObject saberObject, TrailModel trailModel)
        {
            if (saberObject is null || trailModel is null)
            {
                return;
            }

            var trails = SaberHelpers.GetTrails(saberObject).ToArray();
            
            // Add new CustomTrail component with given data (creating position transforms for given position)
            CustomTrail SetupTrail(int length, float startPos, float endPos, Material material)
            {
                var newTrail = saberObject.AddComponent<CustomTrail>();
                newTrail.Length = length;
                newTrail.PointStart = saberObject.CreateGameObject("PointStart").transform;
                newTrail.PointEnd = saberObject.CreateGameObject("PointEnd").transform;
                newTrail.PointEnd.localPosition = new Vector3(0, 0, endPos);
                newTrail.PointStart.localPosition = new Vector3(0, 0, startPos);
                newTrail.TrailMaterial = material;
                return newTrail;
            }

            // if saber has no trial set up a generic one
            if (trails is null || trails.Length < 1)
            {
                trails = new[] { SetupTrail(12, 0, 1, null) };
            }

            var saberTrail = trails[0];

            List<CustomTrail> secondaryTrails = null;

            // set up secondary trails from other saber
            if (trailModel.TrailOriginTrails is { } && trailModel.TrailOriginTrails.Count > 1)
            {
                secondaryTrails = new List<CustomTrail>();
                // first remove all existing CustomTrail components
                for (var i = 1; i < trails.Length; i++)
                {
                    Object.DestroyImmediate(trails[i]);
                }

                // set up new components from other sabers secondary trails
                for (var i = 1; i < trailModel.TrailOriginTrails.Count; i++)
                {
                    var otherTrail = trailModel.TrailOriginTrails[i];
                    if (otherTrail.PointStart is null || otherTrail.PointEnd is null)
                    {
                        continue;
                    }
                    
                    var newTrail = SetupTrail(
                        otherTrail.Length,
                        otherTrail.PointStart.localPosition.z,
                        otherTrail.PointEnd.localPosition.z,
                        otherTrail.TrailMaterial);
                    
                    secondaryTrails.Add(newTrail);
                }
            }
            // set up secondary trails from this saber
            else if (trails.Length > 1)
            {
                secondaryTrails = new List<CustomTrail>();
                for (var i = 1; i < trails.Length; i++)
                {
                    secondaryTrails.Add(trails[i]);
                }
            }

            // if trail comes from the preset save system
            // the model comes without the material assigned
            // so we assign 
            if (trailModel.Material is null)
            {
                trailModel.Material = new MaterialDescriptor(saberTrail.TrailMaterial);

                // set texture wrap mode
                if (trailModel.Material != null && trailModel.Material.Material.TryGetMainTexture(out var tex))
                {
                    trailModel.OriginalTextureWrapMode = tex.wrapMode;
                }
            }

            var pointStart = saberTrail.PointStart;
            var pointEnd = saberTrail.PointEnd;

            // Correction for sabers that have the transforms set up the other way around
            var isTrailReversed = pointStart.localPosition.z > pointEnd.localPosition.z;

            if (isTrailReversed)
            {
                pointStart = saberTrail.PointEnd;
                pointEnd = saberTrail.PointStart;
            }

            InstanceTrailData = new InstanceTrailData(trailModel, pointStart, pointEnd, isTrailReversed, secondaryTrails);
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

            foreach (var renderer in GameObject.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer is null)
                {
                    continue;
                }

                var rendererMaterials = renderer.sharedMaterials;
                var materialCount = rendererMaterials.Length;

                for (var i = 0; i < materialCount; i++)
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
            _postProcessors.Do(x => x.ProcessPart(instance));
            return instance;
        }

        private GameObject GetSaberPrefab()
        {
            return Model.AdditionalInstanceHandler.GetSaber(Model.SaberSlot);
        }
    }
}