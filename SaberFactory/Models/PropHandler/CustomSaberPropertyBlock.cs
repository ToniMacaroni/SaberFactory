namespace SaberFactory.Models.PropHandler
{
    internal class CustomSaberPropertyBlock : PiecePropertyBlock
    {
        public override void SyncFrom(PiecePropertyBlock otherBlock)
        {
            var block = (CustomSaberPropertyBlock)otherBlock;
            TransformProperty.Width = block.TransformProperty.Width;
            TransformProperty.Rotation = -block.TransformProperty.Rotation;
            TransformProperty.Offset = block.TransformProperty.Offset;
        }
    }
}