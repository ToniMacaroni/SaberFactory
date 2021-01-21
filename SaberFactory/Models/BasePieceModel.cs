using System;
using Newtonsoft.Json.Bson;
using SaberFactory.DataStore;
using SaberFactory.Models.PropHandler;
using SaberFactory.UI;
using UnityEngine;

namespace SaberFactory.Models
{
    /// <summary>
    /// Model related to everything that makes up a saber
    /// like parts, halos, accessories, custom sabers
    /// </summary>
    internal class BasePieceModel : IDisposable
    {
        public ModelComposition ModelComposition { get; set; }

        public readonly StoreAsset StoreAsset;

        public GameObject Prefab => StoreAsset.Prefab;

        protected CommonResources _commonResources;

        public ESaberSlot SaberSlot;

        public AdditionalInstanceHandler AdditionalInstanceHandler;

        public PiecePropertyBlock PropertyBlock;

        protected BasePieceModel(StoreAsset storeAsset, CommonResources commonResources)
        {
            StoreAsset = storeAsset;
            _commonResources = commonResources;
        }

        public virtual void Init()
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

        public virtual void Dispose()
        {
        }
    }
}