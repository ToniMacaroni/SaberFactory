using System;
using SaberFactory.Instances;
using SaberFactory.Models;
using SiraUtil.Tools;

namespace SaberFactory.Editor
{
    internal class EditorInstanceManager
    {
        public event Action<SaberInstance> SaberInstanceCreated;
        public event Action<BasePieceInstance> PieceInstanceCreated;
        public event Action<ModelComposition> ModelCompositionSet; 

        public AssetTypeDefinition SelectedDefinition { get; private set; }
        public SaberInstance CurrentSaber { get; private set; }
        public BasePieceInstance CurrentPiece { get; private set; }
        public ModelComposition CurrentModelComposition { get; private set; }
        
        private readonly SiraLog _logger;
        private readonly SaberSet _saberSet;
        private readonly SaberInstance.Factory _saberFactory;

        public EditorInstanceManager(SiraLog logger, SaberSet saberSet, SaberInstance.Factory saberFactory)
        {
            _logger = logger;
            _saberSet = saberSet;
            _saberFactory = saberFactory;

            SelectedDefinition = AssetTypeDefinition.CustomSaber;
        }

        public void SetModelComposition(ModelComposition composition)
        {
            CurrentModelComposition = composition;
            _saberSet.SetModelComposition(CurrentModelComposition);
            ModelCompositionSet?.Invoke(CurrentModelComposition);
        }

        public void SetSelectedDefinition(AssetTypeDefinition definition, bool raiseEvents = false)
        {
            SelectedDefinition = definition;
            CurrentPiece = GetPiece(definition);

            if (raiseEvents)
            {
                if(CurrentPiece!=null) PieceInstanceCreated?.Invoke(CurrentPiece);
            }
        }

        public SaberInstance CreateSaber(SaberModel model, bool raiseSaberEvent = false, bool raisePieceEvent = false)
        {
            CurrentSaber = _saberFactory.Create(model);

            if (raiseSaberEvent)
            {
                SaberInstanceCreated?.Invoke(CurrentSaber);
            }

            CurrentPiece = GetPiece(SelectedDefinition);

            if (raisePieceEvent && CurrentPiece != null)
            {
                PieceInstanceCreated?.Invoke(CurrentPiece);
            }

            return CurrentSaber;
        }

        public void DestroySaber()
        {
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