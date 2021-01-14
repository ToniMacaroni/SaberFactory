using System;
using SaberFactory.DataStore;
using SaberFactory.UI;
using UnityEngine;

namespace SaberFactory.Models
{
    /// <summary>
    /// Model related to everything that makes up a saber
    /// like parts, halos, accessories, custom sabers
    /// </summary>
    internal class BasePieceModel : IDisposable, ICustomListItem
    {
        public ModelComposition ModelComposition { get; set; }

        public readonly StoreAsset StoreAsset;

        public GameObject Prefab => StoreAsset.Prefab;

        protected CommonResources _commonResources;

        protected bool _initialized;

        protected BasePieceModel(StoreAsset storeAsset, CommonResources commonResources)
        {
            StoreAsset = storeAsset;
            _commonResources = commonResources;
        }

        public virtual void Dispose()
        {
        }

        public virtual string ListName { get; }
        public virtual string ListAuthor { get; }
        public virtual Sprite ListCover { get; }
    }
}