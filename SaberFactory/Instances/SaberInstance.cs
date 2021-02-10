using System.Collections.Generic;
using SaberFactory.Helpers;
using SaberFactory.Instances.CustomSaber;
using SaberFactory.Instances.Trail;
using SaberFactory.Models;
using SiraUtil.Interfaces;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace SaberFactory.Instances
{
    /// <summary>
    /// Class for managing an instance of a saber <seealso cref="SaberModel"/>
    /// </summary>
    internal class SaberInstance
    {
        public TrailHandler TrailHandler { get; private set; }

        public readonly SaberModel Model;
        public readonly GameObject GameObject;
        public readonly Transform CachedTransform;

        public readonly PieceCollection<BasePieceInstance> PieceCollection;
        public List<PartEvents> Events { get; private set; }

        private readonly SectionInstantiator _sectionInstantiator;
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

            GameObject.transform.localScale = new Vector3(model.SaberWidth, model.SaberWidth, 1);

            SetupTrailData();
            InitializeEvents();
        }

        public void SetParent(Transform parent)
        {
            CachedTransform.SetParent(parent, false);
        }

        public void SetColor(Color color)
        {
            foreach (BasePieceInstance piece in PieceCollection)
            {
                piece.SetColor(color);
            }

            TrailHandler?.SetColor(color);
        }

        private void InitializeEvents()
        {
            Events = new List<PartEvents>();
            foreach (BasePieceInstance piece in PieceCollection)
            {
                var events = piece.GetEvents();
                if (events != null)
                {
                    Events.Add(events);
                }
            }
        }

        public void SetupTrailData()
        {
            if (GetCustomSaber(out var customsaber)) return;

            // TODO: Setup sf trail data
            _instanceTrailData = default;
        }

        public void CreateTrail(SaberTrailRenderer rendererPrefab, TrailSettings trailSettings, SaberTrail backupTrail = null)
        {
            var trailData = GetTrailData();

            if (trailData is null)
            {
                if (backupTrail is {})
                {
                    TrailHandler = new TrailHandler(GameObject, backupTrail);
                    TrailHandler.SetPrefab(rendererPrefab);
                    TrailHandler.CreateTrail(trailSettings);
                }

                return;
            }

            TrailHandler = new TrailHandler(GameObject);
            TrailHandler.SetPrefab(rendererPrefab);
            TrailHandler.SetTrailData(GetTrailData());
            TrailHandler.CreateTrail(trailSettings);
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
                return customsaber.InstanceTrailData;
            }

            return _instanceTrailData;
        }

        public void SetSaberWidth(float width)
        {
            Model.SaberWidth = width;
            GameObject.transform.localScale = new Vector3(width, width, 1);
        }

        internal class Factory : PlaceholderFactory<SaberModel, SaberInstance> {}
    }
}