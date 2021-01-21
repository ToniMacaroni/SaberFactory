namespace SaberFactory.Models.PropHandler
{
    internal abstract class PiecePropertyBlock
    {
        public abstract void SyncFrom(PiecePropertyBlock otherBlock);
    }
}