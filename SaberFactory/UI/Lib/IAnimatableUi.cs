namespace SaberFactory.UI.Lib
{
    internal interface IAnimatableUi
    {
        EAnimationType AnimationType { get; }

        internal enum EAnimationType
        {
            Horizontal,
            Vertical,
            Z
        }
    }
}