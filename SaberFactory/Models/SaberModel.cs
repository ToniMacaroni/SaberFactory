using System;
using SaberFactory.Helpers;
using SaberFactory.Models.CustomSaber;

namespace SaberFactory.Models
{
    internal class SaberModel
    {
        public readonly PieceCollection<BasePieceModel> PieceCollection;

        private readonly TrailModel _trailModel;

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

        public bool InitTrailModel()
        {
            if (!GetCustomSaber(out var customsaber) || customsaber.TrailModel.HasValue) return false;
            customsaber.TrailModel = new TrailModel();
            return true;
        }

        public TrailModel? GetTrailModel()
        {
            if (GetCustomSaber(out var customsaber))
            {
                return customsaber.TrailModel;
            }

            return _trailModel;
        }

        private bool GetCustomSaber(out CustomSaberModel customsaber)
        {
            if (PieceCollection.TryGetPiece(AssetTypeDefinition.CustomSaber, out var model))
            {
                customsaber = model as CustomSaberModel;
                return true;
            }

            customsaber = null;
            return false;
        }
    }
}