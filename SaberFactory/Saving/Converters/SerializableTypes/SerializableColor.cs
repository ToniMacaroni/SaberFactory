using UnityEngine;

namespace SaberFactory.Saving
{
    internal class SerializableColor
    {
        public float R;
        public float G;
        public float B;
        public float A;

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