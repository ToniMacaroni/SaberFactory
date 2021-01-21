using System;
using SaberFactory.Configuration;
using SaberFactory.DataStore;
using UnityEngine;

namespace SaberFactory.Models.CustomSaber
{
    internal class CustomSaberModelLoader : IStoreAssetParser
    {
        private readonly CustomSaberModel.Factory _factory;
        private readonly PluginConfig _config;

        public CustomSaberModelLoader(CustomSaberModel.Factory factory, PluginConfig config)
        {
            _factory = factory;
            _config = config;
        }

        public ModelComposition GetComposition(StoreAsset storeAsset)
        {
            var modelLeft = _factory.Create(storeAsset);
            var modelRight = _factory.Create(storeAsset);

            modelLeft.SaberDescriptor = modelRight.SaberDescriptor = storeAsset.Prefab.GetComponent<SaberDescriptor>();
            modelLeft.SaberSlot = ESaberSlot.Left;
            modelRight.SaberSlot = ESaberSlot.Right;

            var composition = new ModelComposition(AssetTypeDefinition.CustomSaber, modelLeft, modelRight, storeAsset.Prefab);
            composition.SetFavorite(_config.IsFavorite(storeAsset.Path));

            return composition;
        }
    }
}