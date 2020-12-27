using SaberFactory.DataStore;
using SaberFactory.Helpers;
using SaberFactory.Loaders;
using SiraUtil.Tools;
using Zenject;

namespace SaberFactory.Models.CustomSaber
{
    internal class CustomSaberModel : BasePieceModel
    {
        public TrailModel TrailModel;

        public CustomSaberModel(StoreAsset storeAsset) : base(storeAsset)
        {
            TrailModel = new TrailModel();
        }

        internal class Factory : PlaceholderFactory<StoreAsset, CustomSaberModel> {}
    }
}