using System;
using SaberFactory.UI;
using UnityEngine;

namespace SaberFactory.Models
{
    /// <summary>
    ///     Stores left and right piece models + additional detached game objects in a composition
    /// </summary>
    internal class ModelComposition : IDisposable, ICustomListItem
    {
        public readonly AdditionalInstanceHandler AdditionalInstanceHandler;
        public readonly AssetTypeDefinition AssetTypeDefinition;

        private readonly BasePieceModel _modelLeft;
        private readonly BasePieceModel _modelRight;

        private bool _didLazyInit;

        private ModelMetaData _metaData;

        public ModelComposition(AssetTypeDefinition definition, BasePieceModel modelLeft, BasePieceModel modelRight, GameObject additionalData)
        {
            AssetTypeDefinition = definition;
            _modelLeft = modelLeft;
            _modelRight = modelRight;
            AdditionalInstanceHandler = new AdditionalInstanceHandler(additionalData);

            if (_modelLeft == null && _modelRight == null) return;

            if (_modelLeft != null)
            {
                _modelLeft.ModelComposition = this;
                _modelLeft.AdditionalInstanceHandler = AdditionalInstanceHandler;
                _modelLeft.Init();

                _metaData = modelLeft.GetMetaData();
            }

            if (_modelRight != null)
            {
                _modelRight.ModelComposition = this;
                _modelRight.AdditionalInstanceHandler = AdditionalInstanceHandler;
                _modelRight.Init();
            }
        }

        public string ListName => _metaData.Name;
        public string ListAuthor => _metaData.Author;
        public Sprite ListCover => _metaData.Cover;
        public bool IsFavorite => _metaData.IsFavorite;

        public void Dispose()
        {
            if (_modelLeft != null)
            {
                _modelLeft.StoreAsset.Unload();
                _modelLeft.Dispose();
            }

            _modelRight?.Dispose();
        }

        public void LazyInit()
        {
            if (_didLazyInit || _modelLeft == null) return;
            _didLazyInit = true;
            _modelLeft.OnLazyInit();
        }

        public void SaveAdditionalData()
        {
            if (_modelLeft == null) return;
            _modelLeft.SaveAdditionalData();
        }

        /// <summary>
        ///     Copy settings from the specified model to the other (if it exists)
        /// </summary>
        /// <param name="syncModel">Model to copy from</param>
        public void Sync(BasePieceModel syncModel)
        {
            var otherModel = _modelLeft == syncModel ? _modelRight : _modelLeft;
            otherModel?.SyncFrom(syncModel);
        }

        public BasePieceModel GetLeft()
        {
            return _modelLeft;
        }

        public BasePieceModel GetRight()
        {
            if (_modelRight == null) return _modelLeft;
            return _modelRight;
        }

        public BasePieceModel GetPiece(ESaberSlot saberSlot)
        {
            return saberSlot == ESaberSlot.Left ? GetLeft() : GetRight();
        }

        public void DestroyAdditionalInstances()
        {
            AdditionalInstanceHandler.Destroy();
        }

        public void SetFavorite(bool isFavorite)
        {
            _metaData.IsFavorite = isFavorite;
        }
    }
}