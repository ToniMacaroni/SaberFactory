using UnityEngine;

namespace SaberFactory.Models
{
    internal struct ModelMetaData
    {
        public string Name;
        public string Author;
        public Sprite Cover;
        public bool IsFavorite;

        public ModelMetaData(string name, string author, Sprite cover, bool isFavorite)
        {
            Name = name;
            Author = author;
            Cover = cover;
            IsFavorite = isFavorite;
        }
    }
}