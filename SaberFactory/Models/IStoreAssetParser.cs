using SaberFactory.DataStore;

namespace SaberFactory.Models
{
    /// <summary>
    /// Used to give the correct model imeplementation from an store asset
    /// </summary>
    internal interface IStoreAssetParser
    {
        ModelComposition GetComposition(StoreAsset storeAsset);
    }
}