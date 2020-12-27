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
        private readonly BasePieceModel _modelRigth;
        private readonly GameObject _additionalData;

        public ModelComposition(AssetTypeDefinition definition, BasePieceModel modelLeft, BasePieceModel modelRight, GameObject additionalData)
        {
            AssetTypeDefinition = definition;
            _modelLeft = modelLeft;
            _modelRigth = modelRight;
            _additionalData = additionalData;

            if (_modelLeft != null)
            {
                _modelLeft.ModelComposition = this;
            }

            if (_modelRigth != null)
            {
                _modelRigth.ModelComposition = this;
            }
        }

        public BasePieceModel GetLeft()
        {
            return _modelLeft;
        }

        public BasePieceModel GetRight()
        {
            if (_modelRigth == null) return _modelLeft;
            return _modelRigth;
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

            _modelRigth?.Dispose();
        }
    }
}