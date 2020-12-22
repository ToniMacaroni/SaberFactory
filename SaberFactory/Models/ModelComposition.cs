using System;
using UnityEngine;

namespace SaberFactory.Models
{
    /// <summary>
    /// Stores mulitple piece models + additional detached game objects in a composition
    /// </summary>
    internal class ModelComposition : IDisposable
    {
        private readonly BasePieceModel _modelLeft;
        private readonly BasePieceModel _modelRigth;
        private readonly GameObject _additionalData;

        public ModelComposition(BasePieceModel modelLeft, BasePieceModel modelRight, GameObject additionalData)
        {
            _modelLeft = modelLeft;
            _modelRigth = modelRight;
            _additionalData = additionalData;
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