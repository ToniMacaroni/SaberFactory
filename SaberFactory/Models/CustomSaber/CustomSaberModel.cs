using SaberFactory.DataStore;
using UnityEngine;
using Zenject;

namespace SaberFactory.Models.CustomSaber
{
    internal class CustomSaberModel : BasePieceModel
    {
        public TrailModel TrailModel;
        public SaberDescriptor SaberDescriptor;

        public CustomSaberModel(StoreAsset storeAsset, CommonResources commonResources) : base(storeAsset, commonResources)
        {
            TrailModel = new TrailModel();
        }

        internal class Factory : PlaceholderFactory<StoreAsset, CustomSaberModel> {}

        public override string ListName => SaberDescriptor.SaberName;
        public override string ListAuthor => SaberDescriptor.AuthorName;
        public override Sprite ListCover => SaberDescriptor.CoverImage ?? _commonResources.DefaultCover;
    }
}