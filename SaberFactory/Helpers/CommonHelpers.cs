namespace SaberFactory.Helpers
{
    internal static class CommonHelpers
    {
        public static SaberType ToSaberType(this ESaberSlot saberSlot)
        {
            return saberSlot == ESaberSlot.Left ? SaberType.SaberA : SaberType.SaberB;
        }

        public static T Cast<T>(this object obj)
        {
            return (T) obj;
        }

        public static T CastChecked<T>(this object obj)
        {
            if (obj is T ret) return ret;
            return default;
        }
    }
}