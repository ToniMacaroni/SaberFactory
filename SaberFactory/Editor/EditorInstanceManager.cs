using System;
using SaberFactory.Instances;
using SaberFactory.Models;
using SaberFactory.Saving;
using SiraUtil.Tools;
using UnityEngine;

namespace SaberFactory.Editor
{
    internal class EditorInstanceManager
    {
        public event Action<SaberInstance> OnSaberInstanceCreated;
        public event Action<BasePieceInstance> OnPieceInstanceCreated;
        public event Action<ModelComposition> OnModelCompositionSet; 

        public AssetTypeDefinition SelectedDefinition { get; private set; }
        public SaberInstance CurrentSaber { get; private set; }
        public BasePieceInstance CurrentPiece { get; private set; }
        public ModelComposition CurrentModelComposition { get; private set; }
        
        private readonly SiraLog _logger;
        private readonly SaberSet _saberSet;
        private readonly SaveManager _saveManager;
        private readonly SaberInstance.Factory _saberFactory;

        public EditorInstanceManager(SiraLog logger, SaberSet saberSet, SaveManager saveManager, SaberInstance.Factory saberFactory)
        {
            _logger = logger;
            _saberSet = saberSet;
            _saveManager = saveManager;
            _saberFactory = saberFactory;

            SelectedDefinition = AssetTypeDefinition.CustomSaber;

            saveManager.OnSaberLoaded += delegate
            {
                if (saberSet.LeftSaber.GetCustomSaber(out var customsaber))
                {
                    SetModelComposition(customsaber.ModelComposition);
                }
            };
        }

        public void SetModelComposition(ModelComposition composition)
        {
            CurrentModelComposition?.DestroyAdditionalInstances();
            CurrentModelComposition = composition;
            _saberSet.SetModelComposition(CurrentModelComposition);
            OnModelCompositionSet?.Invoke(CurrentModelComposition);
        }

        public void Refresh()
        {
            if (CurrentModelComposition == null) return;
            SetModelComposition(CurrentModelComposition);
        }

        /// <summary>
        /// Copies settings from the current saber to the other one (if it exists)
        /// </summary>
        public void SyncSabers()
        {
            if (CurrentSaber == null) return;

            _saberSet.Sync(CurrentSaber.Model);
        }

        public SaberInstance CreateSaber(SaberModel model, Transform parent, bool raiseSaberEvent = false, bool raisePieceEvent = false)
        {
            CurrentSaber = _saberFactory.Create(model);
            CurrentSaber.SetParent(parent);

            if (raiseSaberEvent)
            {
                OnSaberInstanceCreated?.Invoke(CurrentSaber);
            }

            CurrentPiece = GetPiece(SelectedDefinition);

            if (raisePieceEvent && CurrentPiece != null)
            {
                OnPieceInstanceCreated?.Invoke(CurrentPiece);
            }

            return CurrentSaber;
        }

        public void DestroySaber()
        {
            CurrentModelComposition?.DestroyAdditionalInstances();
            CurrentSaber?.Destroy();
            CurrentSaber = null;
            CurrentPiece = null;
        }

        /// <summary>
        /// Creates a saber without setting it to current
        /// or firing any event
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public SaberInstance CreateTempSaber(SaberModel model)
        {
            return _saberFactory.Create(model);
        }

        /// <summary>
        /// Gets a piece instance out of the current saber instance
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        public BasePieceInstance GetPiece(AssetTypeDefinition definition)
        {
            if (CurrentSaber == null) return null;
            if (CurrentSaber.PieceCollection.TryGetPiece(definition, out var piece)) return piece;
            return null;
        }
    }
}