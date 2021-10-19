using System;
using SaberFactory.Instances;
using SaberFactory.Models;
using SaberFactory.Saving;
using SiraUtil.Tools;
using UnityEngine;

namespace SaberFactory.Editor
{
    /// <summary>
    ///     Class used to pass instances between different parts of the editor
    /// </summary>
    internal class EditorInstanceManager
    {
        public AssetTypeDefinition SelectedDefinition { get; }
        public SaberInstance CurrentSaber { get; private set; }
        public BasePieceInstance CurrentPiece { get; private set; }
        public ModelComposition CurrentModelComposition { get; private set; }

        private readonly SiraLog _logger;
        private readonly SaberInstance.Factory _saberFactory;
        private readonly SaberSet _saberSet;

        public EditorInstanceManager(SiraLog logger, SaberSet saberSet, PresetSaveManager presetSaveManager, SaberInstance.Factory saberFactory)
        {
            _logger = logger;
            _saberSet = saberSet;
            _saberFactory = saberFactory;

            SelectedDefinition = AssetTypeDefinition.CustomSaber;

            presetSaveManager.OnSaberLoaded += delegate
            {
                if (saberSet.LeftSaber.GetCustomSaber(out var customsaber))
                {
                    SetModelComposition(customsaber.ModelComposition, false);
                }
            };
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
            _logger.Info($"Selected Saber: {composition?.ListName}");
        }

        public void Refresh()
        {
            if (CurrentModelComposition == null)
            {
                return;
            }

            SetModelComposition(CurrentModelComposition);
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
    }
}