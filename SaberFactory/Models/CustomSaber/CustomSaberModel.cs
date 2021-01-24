using CustomSaber;
using SaberFactory.DataStore;
using SaberFactory.Helpers;
using SaberFactory.Instances;
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

        public TrailModel GetColdTrail()
        {
            var trail = Prefab.GetComponent<CustomTrail>();
            return new TrailModel(
                Vector3.zero,
                trail.GetWidth(),
                trail.Length,
                new MaterialDescriptor(trail.TrailMaterial),
                0, trail.TrailMaterial.mainTexture?.wrapMode,
                StoreAsset.Path);
        }

        internal class Factory : PlaceholderFactory<StoreAsset, CustomSaberModel> {}
    }
}