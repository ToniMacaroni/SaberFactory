﻿using System;
using SaberFactory.Instances;
using SaberFactory.Models;
using SaberFactory.Serialization;
using SiraUtil.Logging;
using SiraUtil.Tools;
using UnityEngine;

namespace SaberFactory.Editor
{
    /// <summary>
    ///     Class used to pass instances between different parts of the editor
    /// </summary>
    internal class SaberInstanceManager
    {
        public AssetTypeDefinition SelectedDefinition { get; }
        public SaberInstance CurrentSaber { get; private set; }
        public BasePieceInstance CurrentPiece { get; private set; }
        public ModelComposition CurrentModelComposition { get; private set; }

        public bool LogSelectedSaber { get; set; } = true;

        private readonly SiraLog _logger;
        private readonly SaberInstance.Factory _saberFactory;
        private readonly SaberSet _saberSet;

        public SaberInstanceManager(SiraLog logger, SaberSet saberSet, PresetSaveManager presetSaveManager, SaberInstance.Factory saberFactory)
        {
            _logger = logger;
            _saberSet = saberSet;
            _saberFactory = saberFactory;

            SelectedDefinition = AssetTypeDefinition.CustomSaber;

            presetSaveManager.OnSaberLoaded += OnSaveManagerSaberLoaded;
            OnSaveManagerSaberLoaded();
        }

        public event Action<SaberInstance> OnSaberInstanceCreated;
        public event Action<BasePieceInstance> OnPieceInstanceCreated;
        public event Action<ModelComposition> OnModelCompositionSet;

        public void SetModelComposition(ModelComposition composition, bool lazyInit = true)
        {
            if (CurrentModelComposition != null)
            {
                CurrentModelComposition.SaveAdditionalData();
                CurrentModelComposition.DestroyAdditionalInstances();
            }

            if (lazyInit && CurrentModelComposition != composition)
            {
                composition?.LazyInit();
            }

            CurrentModelComposition = composition;
            _saberSet.SetModelComposition(CurrentModelComposition);
            OnModelCompositionSet?.Invoke(CurrentModelComposition);

            if (LogSelectedSaber)
            {
                _logger.Info($"Selected Saber: {composition?.Name}");
            }
        }

        public void Refresh()
        {
            if (CurrentModelComposition == null)
            {
                return;
            }
            
            var oldLoggingState = LogSelectedSaber;
            LogSelectedSaber = false;

            SetModelComposition(CurrentModelComposition);
            
            LogSelectedSaber = oldLoggingState;
        }
        
        private void OnSaveManagerSaberLoaded()
        {
            if (_saberSet.LeftSaber?.GetCustomSaber(out var customsaber)??false)
            {
                SetModelComposition(customsaber.ModelComposition, false);
            }
        }

        /// <summary>
        ///     Copies settings from the current saber to the other one (if it exists)
        /// </summary>
        public void SyncSabers()
        {
            if (CurrentSaber == null)
            {
                return;
            }

            _saberSet.Sync(CurrentSaber.Model);
        }

        public SaberInstance CreateSaber(SaberModel model, Transform parent, bool raiseSaberEvent = false, bool raisePieceEvent = false)
        {
            CurrentSaber = _saberFactory.Create(model);
            if (parent is { })
            {
                CurrentSaber.SetParent(parent);
            }

            if (raiseSaberEvent)
            {
                RaiseSaberCreatedEvent();
            }

            CurrentPiece = GetPiece(SelectedDefinition);

            if (raisePieceEvent)
            {
                RaisePieceCreatedEvent();
            }

            return CurrentSaber;
        }

        public void RaiseSaberCreatedEvent()
        {
            if (CurrentSaber is null)
            {
                return;
            }

            OnSaberInstanceCreated?.Invoke(CurrentSaber);
        }

        public void RaisePieceCreatedEvent()
        {
            if (CurrentPiece is null)
            {
                return;
            }

            OnPieceInstanceCreated?.Invoke(CurrentPiece);
        }

        public void DestroySaber()
        {
            CurrentModelComposition?.DestroyAdditionalInstances();
            CurrentSaber?.Destroy();
            CurrentSaber = null;
            CurrentPiece = null;
        }

        /// <summary>
        ///     Creates a saber without setting it to current
        ///     or firing any event
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SaberInstance CreateTempSaber(SaberModel model)
        {
            return _saberFactory.Create(model);
        }

        /// <summary>
        ///     Gets a piece instance out of the current saber instance
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        public BasePieceInstance GetPiece(AssetTypeDefinition definition)
        {
            if (CurrentSaber == null)
            {
                return null;
            }

            if (CurrentSaber.PieceCollection.TryGetPiece(definition, out var piece))
            {
                return piece;
            }

            return null;
        }

        /// <summary>
        /// Fires the provided action if a saber is already instanced
        /// and registers the action to the event afterwards
        /// </summary>
        /// <param name="callback"></param>
        /// <returns>True if callback was called immediatly</returns>
        public bool RegisterOnSaberCreated(Action<SaberInstance> callback)
        {
            if (callback == null)
            {
                return false;
            }

            var wasCalled = false;

            if (CurrentSaber != null)
            {
                wasCalled = true;
                callback(CurrentSaber);
            }
            
            OnSaberInstanceCreated += callback;

            return wasCalled;
        }
        
        /// <summary>
        /// Fires the provided action if a composition is already instanced
        /// and registers the action to the event afterwards
        /// </summary>
        /// <param name="callback"></param>
        /// <returns>True if callback was called immediatly</returns>
        public bool RegisterOnCompositionSet(Action<ModelComposition> callback)
        {
            if (callback == null)
            {
                return false;
            }
            
            var wasCalled = false;

            if (CurrentModelComposition != null)
            {
                wasCalled = true;
                callback(CurrentModelComposition);
            }
            
            OnModelCompositionSet += callback;
            return wasCalled;
        }
    }
}