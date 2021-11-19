using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SaberFactory.Helpers;
using SaberFactory.Serialization;

namespace SaberFactory.Models.PropHandler
{
    /// <summary>
    ///     Base class for storing customizable / serializable properties
    /// </summary>
    internal abstract class PiecePropertyBlock : IFactorySerializable
    {
        public TransformPropertyBlock TransformProperty;

        protected PiecePropertyBlock()
        {
            TransformProperty = new TransformPropertyBlock();
        }

        public virtual async Task FromJson(JObject obj, Serializer serializer)
        {
            await TransformProperty.FromJson((JObject)obj[nameof(TransformProperty)], serializer);
        }

        public virtual async Task<JToken> ToJson(Serializer serializer)
        {
            var obj = new JObject
            {
                { nameof(TransformProperty), await TransformProperty.ToJson(serializer) }
            };
            return obj;
        }

        public abstract void SyncFrom(PiecePropertyBlock otherBlock);
    }
}