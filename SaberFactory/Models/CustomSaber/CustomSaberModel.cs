using SaberFactory.DataStore;
using SaberFactory.Loaders;
using SiraUtil.Tools;
using Zenject;

namespace SaberFactory.Models.CustomSaber
{
    internal class CustomSaberModel : BasePieceModel
    {
        private readonly SiraLog _logger;

        public CustomSaberModel(StoreAsset storeAsset, SiraLog logger) : base(storeAsset)
        {
            _logger = logger;
        }

        internal class Factory : PlaceholderFactory<StoreAsset, CustomSaberModel> {}
    }
}