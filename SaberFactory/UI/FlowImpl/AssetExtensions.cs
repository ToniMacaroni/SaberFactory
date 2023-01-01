using System.Threading.Tasks;
using SaberFactory.DataStore;
using SaberFactory.Models;

namespace SaberFactory.UI.Flow;

public static class AssetExtensions
{
    public static async Task<ModelComposition> GetModelComposition(this IAssetInfo asset, MainAssetStore assetStore)
    {
        ModelComposition comp = null;
        if (asset is PreloadMetaData metaData)
        {
            comp = await assetStore[metaData];
        }
        else if (asset is ModelComposition c)
        {
            comp = c;
        }

        return comp;
    }
}