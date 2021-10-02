using UnityEngine;

namespace SaberFactory.Saving.Converters.SerializableTypes
{
    internal class SerializableColor
    {
        public float A;
        public float B;
        public float G;
        public float R;

        public Color ToColor()
        {
            return new Color(R, G, B, A);
        }

        public static SerializableColor FromColor(Color color)
        {
            return new SerializableColor
            {
                R = color.r,
                G = color.g,
                B = color.b,
                A = color.a
            };
        }
    }
}