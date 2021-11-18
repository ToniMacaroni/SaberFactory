using UnityEngine;

namespace SaberFactory.Serialization.Converters.SerializableTypes
{
    internal class SerializableVector3
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3 ToVector()
        {
            return new Vector3(X, Y, Z);
        }

        public static SerializableVector3 FromVector(Vector3 vec)
        {
            return new SerializableVector3
            {
                X = vec.x,
                Y = vec.y,
                Z = vec.z
            };
        }
    }
}