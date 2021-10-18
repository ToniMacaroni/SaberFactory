using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaberFactory.Helpers;
using SaberFactory.Models.CustomSaber;
using SaberFactory.Saving;

namespace SaberFactory.Models
{
    /// <summary>
    ///     Stores information on how to build a saber instance
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal class SaberModel : IFactorySerializable
    {
        public bool IsEmpty => PieceCollection.PieceCount == 0;
        public readonly PieceCollection<BasePieceModel> PieceCollection;

        public readonly ESaberSlot SaberSlot;

        [JsonProperty] [MapSerialize] public float SaberWidth = 1;

        public TrailModel TrailModel;

        public SaberModel(ESaberSlot saberSlot)
        {
            SaberSlot = saberSlot;

            PieceCollection = new PieceCollection<BasePieceModel>();
        }

        public async Task FromJson(JObject obj, Serializer serializer)
        {
            obj.Populate(this);
            var piecesTkn = obj.Property(nameof(PieceCollection));
            if (piecesTkn != null)
            {
                var pieceList = (JArray)piecesTkn.Value;
                foreach (var pieceTkn in pieceList)
                {
                    var piece = await serializer.LoadPiece(pieceTkn["Path"]);
                    if (piece == null) continue;
                    PieceCollection.AddPiece(piece.AssetTypeDefinition, piece.GetPiece(SaberSlot));
                    await piece.GetLeft()?.FromJson((JObject)pieceTkn, serializer);
                }
            }
        }

        public async Task<JToken> ToJson(Serializer serializer)
        {
            var obj = JObject.FromObject(this);

            var pieceList = new JArray();

            foreach (BasePieceModel pieceModel in PieceCollection) pieceList.Add(await pieceModel.ToJson(serializer));

            obj.Add(nameof(PieceCollection), pieceList);
            return obj;
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