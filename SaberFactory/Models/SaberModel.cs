using System;
using SaberFactory.Helpers;
using SaberFactory.Models.CustomSaber;

namespace SaberFactory.Models
{
    internal class SaberModel
    {
        public readonly PieceCollection<BasePieceModel> PieceCollection;

        public TrailModel TrailModel;

        public readonly ESaberSlot SaberSlot;

        public float SaberWidth = 1;

        private SaberModel(ESaberSlot saberSlot)
        {
            SaberSlot = saberSlot;

            PieceCollection = new PieceCollection<BasePieceModel>();
        }

        public void SetModelComposition(ModelComposition composition)
        {
            PieceCollection[composition.AssetTypeDefinition] = SaberSlot==ESaberSlot.Left
                ? composition.GetLeft()
                : composition.GetRight();
        }

        public TrailModel GetTrailModel()
        {
            if (GetCustomSaber(out var customsaber))
            {
                return customsaber.TrailModel;
            }

            return TrailModel;
        }

        public void Sync()
        {
            foreach (BasePieceModel piece in PieceCollection)
            {
                piece.ModelComposition.Sync(piece);
            }
        }

        public bool GetCustomSaber(out CustomSaberModel customsaber)
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