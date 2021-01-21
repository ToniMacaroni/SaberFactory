namespace SaberFactory.Models.PropHandler
{
    internal class CustomSaberPropertyBlock : PiecePropertyBlock
    {
        public TransformPropertyBlock TransformProperty;

        public CustomSaberPropertyBlock()
        {
            TransformProperty = new TransformPropertyBlock();
        }

        public override void SyncFrom(PiecePropertyBlock otherBlock)
        {
            var block = (CustomSaberPropertyBlock) otherBlock;
            TransformProperty.Width = block.TransformProperty.Width;
        }
    }
}