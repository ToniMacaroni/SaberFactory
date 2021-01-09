using System;
using UnityEngine;

namespace SaberFactory.Models
{
    /// <summary>
    /// Stores mulitple piece models + additional detached game objects in a composition
    /// </summary>
    internal class ModelComposition : IDisposable
    {
        public readonly AssetTypeDefinition AssetTypeDefinition;

        private readonly BasePieceModel _modelLeft;
        private readonly BasePieceModel _modelRight;
        private readonly GameObject _additionalData;

        public ModelComposition(AssetTypeDefinition definition, BasePieceModel modelLeft, BasePieceModel modelRight, GameObject additionalData)
        {
            AssetTypeDefinition = definition;
            _modelLeft = modelLeft;
            _modelRight = modelRight;
            _additionalData = additionalData;

            if (_modelLeft != null)
            {
                _modelLeft.ModelComposition = this;
            }

            if (_modelRight != null)
            {
                _modelRight.ModelComposition = this;
            }
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

        public GameObject GetAddidionalData()
        {
            return _additionalData;
        }

        public void Dispose()
        {
            if (_modelLeft != null)
            {
                _modelLeft.StoreAsset.Unload();
                _modelLeft.Dispose();
            }

            _modelRight?.Dispose();
        }
    }
}