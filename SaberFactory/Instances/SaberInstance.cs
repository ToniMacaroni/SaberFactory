using System.Collections.Generic;
using SaberFactory.Helpers;
using SaberFactory.Instances.CustomSaber;
using SaberFactory.Instances.Trail;
using SaberFactory.Models;
using SaberFactory.Models.CustomSaber;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace SaberFactory.Instances
{
    internal class SaberInstance
    {
        public TrailHandler TrailHandler { get; private set; }

        public readonly SaberModel Model;
        public readonly GameObject GameObject;
        public readonly Transform CachedTransform;

        public readonly PieceCollection<BasePieceInstance> PieceCollection;

        private readonly SectionInstantiator _sectionInstantiator;
        private readonly List<Material> _colorMaterials;
        private readonly SiraLog _logger;

        private InstanceTrailData _instanceTrailData;

        private SaberInstance(SaberModel model, BasePieceInstance.Factory pieceFactory, SiraLog logger)
        {
            _logger = logger;

            Model = model;
            GameObject = new GameObject("SF Saber");
            CachedTransform = GameObject.transform;

            PieceCollection = new PieceCollection<BasePieceInstance>();

            _sectionInstantiator = new SectionInstantiator(this, pieceFactory, PieceCollection);
            _sectionInstantiator.InstantiateSections();

            _colorMaterials = new List<Material>();
            GetColorableMaterials(_colorMaterials);

            SetupTrailData();
        }

        public void SetupTrailData()
        {
            if (GetCustomSaber(out var customsaber)) return;

            // TODO: Setup sf trail data
            _instanceTrailData = default;
        }

        public void SetParent(Transform parent)
        {
            CachedTransform.SetParent(parent, false);
        }

        public void SetColor(Color color)
        {
            foreach (var material in _colorMaterials)
            {
                material.color = color;
            }

            if (TrailHandler != null)
            {
                TrailHandler.TrailInstance.Color = color;
            }
        }

        public void CreateTrail(SaberTrailRenderer rendererPrefab)
        {
            TrailHandler = new TrailHandler(GameObject);
            TrailHandler.SetPrefab(rendererPrefab);
            TrailHandler.SetTrailData(_instanceTrailData);
            TrailHandler.CreateTrail();
        }

        public void DestroyTrail()
        {
            TrailHandler?.DestroyTrail();
        }

        public void Destroy()
        {
            GameObject.TryDestroy();
        }

        private bool GetCustomSaber(out CustomSaberInstance customSaberInstance)
        {
            if (PieceCollection.TryGetPiece(AssetTypeDefinition.CustomSaber, out var instance))
            {
                customSaberInstance = instance as CustomSaberInstance;
                return true;
            }

            customSaberInstance = null;
            return false;
        }

        public InstanceTrailData GetTrailData()
        {
            if (GetCustomSaber(out var customsaber))
            {
                return customsaber.GetInstanceTrailData(true);
            }

            return _instanceTrailData;
        }

        private void GetColorableMaterials(List<Material> materials)
        {
            var customsaber = PieceCollection[AssetTypeDefinition.CustomSaber];

            foreach (var renderer in GameObject.GetComponentsInChildren<Renderer>())
            {
                foreach (var material in renderer.materials)
                {
                    // color for saber factory models
                    if (material.HasProperty("_UseColorScheme") && material.GetFloat("_UseColorScheme") > 0.5f || material.HasProperty("_CustomColors") && material.GetFloat("_CustomColors") > 0)
                        materials.Add(material);

                    // color for custom saber models
                    else if (PieceCollection.HasPiece(AssetTypeDefinition.CustomSaber) && (material.HasProperty("_Glow") && material.GetFloat("_Glow") > 0 || material.HasProperty("_Bloom") && material.GetFloat("_Bloom") > 0))
                        materials.Add(material);
                }
            }
        }

        internal class Factory : PlaceholderFactory<SaberModel, SaberInstance> {}
    }
}