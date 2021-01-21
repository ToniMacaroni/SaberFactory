using SaberFactory.DataStore;
using SaberFactory.Models.PropHandler;
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
            PropertyBlock = new CustomSaberPropertyBlock();
        }

        public override ModelMetaData GetMetaData()
        {
            return new ModelMetaData(SaberDescriptor.SaberName, SaberDescriptor.AuthorName,
                SaberDescriptor.CoverImage, false);
        }

        public override void SyncFrom(BasePieceModel otherModel)
        {
            base.SyncFrom(otherModel);
            var otherCs = (CustomSaberModel) otherModel;
            TrailModel = otherCs.TrailModel;
        }

        internal class Factory : PlaceholderFactory<StoreAsset, CustomSaberModel> {}
    }
}