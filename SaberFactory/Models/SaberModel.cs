using SaberFactory.Models.CustomSaber;
using SaberFactory.Saving;

namespace SaberFactory.Models
{
    /// <summary>
    ///     Stores information on how to build a saber instance
    /// </summary>
    internal class SaberModel
    {
        public bool IsEmpty => PieceCollection.PieceCount == 0;
        public readonly PieceCollection<BasePieceModel> PieceCollection;

        public readonly ESaberSlot SaberSlot;

        [MapSerialize] public float SaberWidth = 1;

        public TrailModel TrailModel;

        public SaberModel(ESaberSlot saberSlot)
        {
            SaberSlot = saberSlot;

            PieceCollection = new PieceCollection<BasePieceModel>();
        }

        public void SetModelComposition(ModelComposition composition)
        {
            PieceCollection[composition.AssetTypeDefinition] = SaberSlot == ESaberSlot.Left
                ? composition.GetLeft()
                : composition.GetRight();
        }

        public TrailModel GetTrailModel()
        {
            if (GetCustomSaber(out var customsaber)) return customsaber.TrailModel;

            return TrailModel;
        }

        public void Sync()
        {
            foreach (BasePieceModel piece in PieceCollection) piece.ModelComposition.Sync(piece);
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