using System;
using System.Collections.Generic;
using System.Linq;
using CustomSaber;
using HarmonyLib;
using SaberFactory.AssetProperties;
using SaberFactory.Configuration;
using SaberFactory.Helpers;
using SaberFactory.Instances.CustomSaber;
using SaberFactory.Instances.PostProcessors;
using SaberFactory.Instances.Trail;
using SaberFactory.Models;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;

namespace SaberFactory.Instances
{
    /// <summary>
    ///     Class for managing an instance of a saber <seealso cref="SaberModel" />
    /// </summary>
    public class SaberInstance
    {
        public const string SaberName = "SF Saber";

        public readonly SaberModel Model;

        public FloatProperty SaberWidth { get; private set; }
        public FloatProperty SaberLength { get; private set; }

        internal List<PartEvents> Events { get; private set; }
        
        public readonly Transform CachedTransform;
        public readonly GameObject GameObject;

        internal ITrailHandler TrailHandler { get; private set; }
        private readonly TrailConfig _trailConfig;
        private InstanceTrailData _instanceTrailData;
        private List<CustomSaberTrailHandler> _secondaryTrails;
        
        internal readonly PieceCollection<BasePieceInstance> PieceCollection;
        private readonly Dictionary<Type, Component> _saberComponents = new();
        
        internal event Action OnDestroyed;

        private SaberInstance(
            SaberModel model,
            BasePieceInstance.Factory pieceFactory,
            SiraLog logger,
            TrailConfig trailConfig,
            List<ISaberPostProcessor> saberMiddlewares)
        {
            _trailConfig = trailConfig;

            Model = model;

            GameObject = new GameObject(SaberName);
            GameObject.AddComponent<SaberMonoBehaviour>().Init(this, _saberComponents, OnSaberGameObjectDestroyed);

            CachedTransform = GameObject.transform;

            PieceCollection = new PieceCollection<BasePieceInstance>();

            var sectionInstantiator = new SectionInstantiator(this, pieceFactory, PieceCollection);
            sectionInstantiator.InstantiateSections();

            saberMiddlewares.Do(x => x.ProcessSaber(this));

            SetupTrailData();
            InitializeEvents();
            RegisterProperties();
        }

        private void RegisterProperties()
        {
            SaberWidth = new FloatProperty(Model.SaberWidth, 0, 3, 0.05f);
            SaberWidth.RegisterHandler(nameof(SaberInstance), val =>
            {
                Model.SaberWidth = val;
                GameObject.transform.localScale = new Vector3(val, val, Model.SaberLength);
            });
            SaberWidth.InvokeValueChange();

            SaberLength = new FloatProperty(Model.SaberLength);
            SaberLength.RegisterHandler(nameof(SaberInstance), val =>
            {
                Model.SaberLength = val;
                GameObject.transform.localScale = new Vector3(Model.SaberWidth, Model.SaberWidth, val);
            });
            SaberLength.InvokeValueChange();
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

            if (_secondaryTrails is { })
            {
                foreach (var trail in _secondaryTrails)
                {
                    trail.SetColor(color);
                }
            }
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

        private void SetupTrailData()
        {
            if (GetCustomSaber(out var customsaber))
            {
                return;
            }

            // TODO: Setup sf trail data
            _instanceTrailData = default;
        }

        public void CreateTrail(bool editor, SaberTrail backupTrail = null)
        {
            var trailData = GetTrailData(out var secondaryTrails);

            if (trailData is null)
            {
                if (backupTrail is { })
                {
                    TrailHandler = new MainTrailHandler(GameObject, backupTrail);
                    TrailHandler.CreateTrail(_trailConfig, editor);
                }

                return;
            }

            TrailHandler = new MainTrailHandler(GameObject);
            TrailHandler.SetTrailData(trailData);
            TrailHandler.CreateTrail(_trailConfig, editor);

            if (secondaryTrails is { })
            {
                _secondaryTrails = new List<CustomSaberTrailHandler>();
                foreach (var customTrail in secondaryTrails)
                {
                    var handler = new CustomSaberTrailHandler(GameObject, customTrail);
                    handler.CreateTrail(_trailConfig, editor);
                    _secondaryTrails.Add(handler);
                }
            }
        }

        public void DestroyTrail(bool immediate = false)
        {
            TrailHandler?.DestroyTrail(immediate);
            if (_secondaryTrails is { })
            {
                foreach (var trail in _secondaryTrails)
                {
                    trail.DestroyTrail();
                }
            }
        }

        public bool GetSaberComponent<T>(out T saberComp) where T : Component
        {
            saberComp = null;
            if (!_saberComponents.TryGetValue(typeof(T), out var comp)) return false;
            saberComp = (T)comp;
            return saberComp;
        }

        public void Destroy()
        {
            GameObject.TryDestroy();
            OnDestroyed?.Invoke();
        }

        // Called when Saber GameObject gets destroyed
        private void OnSaberGameObjectDestroyed()
        {
            DestroyTrail();

            foreach (BasePieceInstance piece in PieceCollection)
            {
                piece.Dispose();
            }
        }

        internal bool GetCustomSaber(out CustomSaberInstance customSaberInstance)
        {
            if (PieceCollection.TryGetPiece(AssetTypeDefinition.CustomSaber, out var instance))
            {
                customSaberInstance = instance as CustomSaberInstance;
                return true;
            }

            customSaberInstance = null;
            return false;
        }

        internal InstanceTrailData GetTrailData(out List<CustomTrail> secondaryTrails)
        {
            secondaryTrails = null;

            if (GetCustomSaber(out var customsaber))
            {
                secondaryTrails = customsaber.InstanceTrailData?.SecondaryTrails.Select(x => x.Trail).ToList();
                return customsaber.InstanceTrailData;
            }

            return _instanceTrailData;
        }

        internal class Factory : PlaceholderFactory<SaberModel, SaberInstance>
        { }

        internal class SaberMonoBehaviour : MonoBehaviour
        {
            public SaberInstance SaberInstance { get; private set; }
            private Action _onDestroyed;
            public Dictionary<Type, Component> SaberComponentDict;

            private void OnDestroy()
            {
                _onDestroyed?.Invoke();
            }

            public void Init(SaberInstance saberInstance, Dictionary<Type, Component> saberComponentDict, Action onDestroyedCallback)
            {
                SaberInstance = saberInstance;
                SaberComponentDict = saberComponentDict;
                _onDestroyed = onDestroyedCallback;
            }

            public void RegisterComponent(Component comp)
            {
                SaberComponentDict[comp.GetType()] = comp;
            }

        }
    }
}