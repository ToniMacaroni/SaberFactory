using System.Collections.Generic;

namespace SaberFactory.Saving
{
    internal class SerializableSaber
    {
        public List<SerializablePiece> Pieces;
        public SerializableTrail Trail;
        public float SaberWidth;
    }
}