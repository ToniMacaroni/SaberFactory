namespace SaberFactory.Models.PropHandler
{
    /// <summary>
    /// Base class for storing customizable / serializable properties
    /// </summary>
    internal abstract class PiecePropertyBlock
    {
        public abstract void SyncFrom(PiecePropertyBlock otherBlock);
    }
}