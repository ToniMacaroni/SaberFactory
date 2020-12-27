using System;
using SaberFactory.DataStore;
using SaberFactory.Loaders;
using UnityEngine;

namespace SaberFactory.Models.CustomSaber
{
    internal class CustomSaberModelLoader : IStoreAssetParser
    {
        private readonly CustomSaberModel.Factory _factory;

        public CustomSaberModelLoader(CustomSaberModel.Factory factory)
        {
            _factory = factory;
        }

        public ModelComposition GetComposition(StoreAsset storeAsset)
        {
            var sabers = GetSabers(storeAsset.Prefab.transform);

            var storeAssetLeft = new StoreAsset(storeAsset.Path, sabers.Item1, storeAsset.AssetBundle);
            var storeAssetRight = new StoreAsset(storeAsset.Path, sabers.Item2, storeAsset.AssetBundle);

            var modelLeft = _factory.Create(storeAssetLeft);
            var modelRight = _factory.Create(storeAssetRight);

            var composition = new ModelComposition(AssetTypeDefinition.CustomSaber, modelLeft, modelRight, storeAsset.Prefab);

            return composition;
        }

        // returns <leftsaber, rightsaber>
        private Tuple<GameObject, GameObject> GetSabers(Transform root)
        {
            GameObject leftSaber = null;
            GameObject rightSaber = null;
            foreach (Transform t in root)
            {
                if (t.name == "LeftSaber") leftSaber = t.gameObject;
                else if (t.name == "RightSaber") rightSaber = t.gameObject;
                if (leftSaber != null && rightSaber != null) break;
            }

            return new Tuple<GameObject, GameObject>(leftSaber, rightSaber);
        }
    }
}