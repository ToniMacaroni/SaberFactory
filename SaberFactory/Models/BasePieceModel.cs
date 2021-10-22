using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SaberFactory.DataStore;
using SaberFactory.Helpers;
using SaberFactory.Models.PropHandler;
using UnityEngine;

namespace SaberFactory.Models
{
    /// <summary>
    ///     Model related to everything that makes up a saber
    ///     like parts, halos, accessories, custom sabers
    /// </summary>
    internal class BasePieceModel : IDisposable, IFactorySerializable
    {
        /// <summary>
        ///     Type of the associated instance class
        /// </summary>
        public virtual Type InstanceType { get; protected set; }

        public ModelComposition ModelComposition { get; set; }

        public GameObject Prefab => StoreAsset.Prefab;

        public readonly ModifyableComponentManager ModifyableComponentManager;

        public readonly StoreAsset StoreAsset;

        public AdditionalInstanceHandler AdditionalInstanceHandler;

        public PiecePropertyBlock PropertyBlock;

        public ESaberSlot SaberSlot;

        protected BasePieceModel(StoreAsset storeAsset)
        {
            StoreAsset = storeAsset;
            ModifyableComponentManager = new ModifyableComponentManager(storeAsset.Prefab);
            ModifyableComponentManager.Map();
        }

        public virtual void Dispose()
        {
        }

        public virtual async Task FromJson(JObject obj, Serializer serializer)
        {
            await PropertyBlock.FromJson((JObject)obj[nameof(PropertyBlock)], serializer);
            await ModifyableComponentManager.FromJson((JObject)obj[nameof(ModifyableComponentManager)], serializer);
        }

        public virtual async Task<JToken> ToJson(Serializer serializer)
        {
            var obj = new JObject
            {
                { "Path", StoreAsset.RelativePath },
                { nameof(PropertyBlock), await PropertyBlock.ToJson(serializer) },
                { nameof(ModifyableComponentManager), await ModifyableComponentManager.ToJson(serializer) }
            };
            return obj;
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
            ModifyableComponentManager.Sync(otherModel.ModifyableComponentManager);
        }
    }
}