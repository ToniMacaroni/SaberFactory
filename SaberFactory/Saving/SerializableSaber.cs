using System.Collections.Generic;

namespace SaberFactory.Saving
{
    internal class SerializableSaber
    {
        public List<SerializablePiece> Pieces;

        public float SaberWidth;
        public SerializableTrail Trail;
    }
}