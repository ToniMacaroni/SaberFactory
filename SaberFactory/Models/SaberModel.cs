using System;
using SaberFactory.Helpers;
using SaberFactory.Models.CustomSaber;

namespace SaberFactory.Models
{
    internal class SaberModel
    {
        public readonly PieceCollection<BasePieceModel> PieceCollection;

        private TrailModel _trailModel;

        public readonly SaberType SaberType;

        private SaberModel(SaberType saberType)
        {
            SaberType = saberType;

            PieceCollection = new PieceCollection<BasePieceModel>();
            _trailModel = new TrailModel();
        }

        public void SetModelComposition(ModelComposition composition)
        {
            PieceCollection[composition.AssetTypeDefinition] = SaberType.IsLeft()
                ? composition.GetLeft()
                : composition.GetRight();
        }

        public TrailModel GetTrailModel()
        {
            if (PieceCollection.TryGetPiece(AssetTypeDefinition.CustomSaber, out var model))
            {
                return (model as CustomSaberModel).TrailModel;
            }

            return _trailModel;
        }
    }
}