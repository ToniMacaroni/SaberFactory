using SaberFactory.Configuration;
using SaberFactory.DataStore;
using UnityEngine;

namespace SaberFactory.Models.CustomSaber
{
    internal class CustomSaberModelLoader : IStoreAssetParser
    {
        private readonly PluginConfig _config;
        private readonly CustomSaberModel.Factory _factory;

        public CustomSaberModelLoader(CustomSaberModel.Factory factory, PluginConfig config)
        {
            _factory = factory;
            _config = config;
        }

        public ModelComposition GetComposition(StoreAsset storeAsset)
        {
            var (leftSaber, rightSaber) = GetSabers(storeAsset.Prefab.transform);

            var storeAssetLeft = new StoreAsset(storeAsset.RelativePath, leftSaber, storeAsset.AssetBundle);
            var storeAssetRight = new StoreAsset(storeAsset.RelativePath, rightSaber, storeAsset.AssetBundle);

            var modelLeft = _factory.Create(storeAssetLeft);
            var modelRight = _factory.Create(storeAssetRight);

            modelLeft.SaberDescriptor = modelRight.SaberDescriptor = storeAsset.Prefab.GetComponent<SaberDescriptor>();
            modelLeft.SaberSlot = ESaberSlot.Left;
            modelRight.SaberSlot = ESaberSlot.Right;

            var composition = new ModelComposition(AssetTypeDefinition.CustomSaber, modelLeft, modelRight, storeAsset.Prefab);
            composition.SetFavorite(_config.IsFavorite(storeAsset.RelativePath));

            return composition;
        }

        private (GameObject leftSaber, GameObject rightSaber) GetSabers(Transform root)
        {
            GameObject leftSaber = null;
            GameObject rightSaber = null;
            foreach (Transform t in root)
            {
                if (t.name == "LeftSaber")
                {
                    leftSaber = t.gameObject;
                }
                else if (t.name == "RightSaber")
                {
                    rightSaber = t.gameObject;
                }

                if (leftSaber != null && rightSaber != null)
                {
                    break;
                }
            }

            return (leftSaber, rightSaber);
        }
    }
}