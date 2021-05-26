using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaberFactory.Saving.Converters;
using UnityEngine;

namespace SaberFactory.Saving
{
    internal class SerializableTrail
    {
        public int Length;

        public float Width;

        public float Whitestep;

        public string TrailOrigin;

        public bool ClampTexture;

        public SerializableMaterial Material;

        public bool Flip;

        [JsonConverter(typeof(SerVec3Converter))]
        public Vector3 TrailPosOffset;
    }
}