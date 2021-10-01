using Newtonsoft.Json;
using SaberFactory.Saving.Converters;
using UnityEngine;

namespace SaberFactory.Saving
{
    internal class SerializableTrail
    {
        public bool ClampTexture;

        public bool Flip;
        public int Length;

        public SerializableMaterial Material;

        public string TrailOrigin;

        [JsonConverter(typeof(SerVec3Converter))]
        public Vector3 TrailPosOffset;

        public float Whitestep;

        public float Width;
    }
}