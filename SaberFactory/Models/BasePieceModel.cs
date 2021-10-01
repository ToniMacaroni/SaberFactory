using System;
using SaberFactory.DataStore;
using SaberFactory.Models.PropHandler;
using UnityEngine;

namespace SaberFactory.Models
{
    /// <summary>
    ///     Model related to everything that makes up a saber
    ///     like parts, halos, accessories, custom sabers
    /// </summary>
    internal class BasePieceModel : IDisposable
    {
        public virtual Type InstanceType { get; protected set; }

        public ModelComposition ModelComposition { get; set; }

        public GameObject Prefab => StoreAsset.Prefab;

        public readonly StoreAsset StoreAsset;

        public AdditionalInstanceHandler AdditionalInstanceHandler;

        public PiecePropertyBlock PropertyBlock;

        public ESaberSlot SaberSlot;


        protected BasePieceModel(StoreAsset storeAsset)
        {
            StoreAsset = storeAsset;
        }

        public virtual void Dispose()
        {
        }

        public virtual void Init()
        {
        }

        public virtual void OnLazyInit()
        {
        }

        public virtual void SaveAdditionalData()
        {
        }

        public virtual ModelMetaData GetMetaData()
        {
            return default;
        }

        public virtual void SyncFrom(BasePieceModel otherModel)
        {
            PropertyBlock.SyncFrom(otherModel.PropertyBlock);
        }
    }
}