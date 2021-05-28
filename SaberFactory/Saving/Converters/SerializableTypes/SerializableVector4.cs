using UnityEngine;

namespace SaberFactory.Saving.Converters.SerializableTypes
{
    internal class SerializableVector4
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Vector4 ToVector()
        {
            return new Vector4(X, Y, Z, W);
        }

        public static SerializableVector4 FromVector(Vector4 vec)
        {
            return new SerializableVector4
            {
                X = vec.x,
                Y = vec.y,
                Z = vec.z,
                W = vec.w
            };
        }
    }
}