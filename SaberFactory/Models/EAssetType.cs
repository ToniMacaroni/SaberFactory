namespace SaberFactory.Models
{
    internal enum EAssetType
    {
        Model,
        Halo
    }

    internal enum EAssetSubType
    {
        Blade,
        Emitter,
        Handle,
        Pommel,
        CustomSaber
    }

    internal readonly struct AssetTypeDefinition
    {
        public static readonly AssetTypeDefinition CustomSaber = new AssetTypeDefinition(EAssetType.Model, EAssetSubType.CustomSaber);

        public EAssetType AssetType { get; }
        public EAssetSubType AssetSubType { get; }

        public AssetTypeDefinition(EAssetType assetType, EAssetSubType assetSubType)
        {
            AssetType = assetType;
            AssetSubType = assetSubType;
        }
    }
}