namespace SaberFactory.Helpers
{
    public static class BaseGameTypeExtension
    {
        public static bool IsLeft(this SaberType saberType)
        {
            return saberType == SaberType.SaberA;
        }
    }
}