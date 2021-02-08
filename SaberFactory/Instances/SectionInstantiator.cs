using SaberFactory.Models;

namespace SaberFactory.Instances
{
    /// <summary>
    /// Class used to instantiate all the parts and pieces of a saber and keeping track of them
    /// </summary>
    internal class SectionInstantiator
    {
        private readonly SaberInstance _saberInstance;
        private readonly PieceCollection<BasePieceInstance> _pieceCollection;
        private readonly BasePieceInstance.Factory _pieceInstanceFactory;

        public SectionInstantiator(SaberInstance saberInstance, BasePieceInstance.Factory pieceInstanceFactory, PieceCollection<BasePieceInstance> pieceCollection)
        {
            _saberInstance = saberInstance;
            _pieceInstanceFactory = pieceInstanceFactory;
            _pieceCollection = pieceCollection;
        }

        public void InstantiateSection(AssetTypeDefinition definition)
        {
            var modelPiece = _saberInstance.Model.PieceCollection[definition];
            if (modelPiece != null)
            {
                var instance = _pieceInstanceFactory.Create(modelPiece);
                instance.SetParent(_saberInstance.CachedTransform);
                _saberInstance.PieceCollection[definition] = instance;
            }
        }

        public void InstantiateSection(EAssetType type, EAssetSubType subType)
        {
            var definition = new AssetTypeDefinition(type, subType);
            InstantiateSection(definition);
        }

        public void InstantiateSections()
        {
            if (_saberInstance.Model.PieceCollection.HasPiece(AssetTypeDefinition.CustomSaber))
            {
                InstantiateCustomSaber();

                return;
            }

            InstantiateSection(EAssetType.Model, EAssetSubType.Pommel);
            InstantiateSection(EAssetType.Model, EAssetSubType.Handle);
            InstantiateSection(EAssetType.Model, EAssetSubType.Emitter);
            InstantiateSection(EAssetType.Model, EAssetSubType.Blade);
        }

        public void InstantiateCustomSaber()
        {
            InstantiateSection(EAssetType.Model, EAssetSubType.CustomSaber);
        }
    }
}