using System.Collections;
using System.Collections.Generic;

namespace SaberFactory.Models
{
    /// <summary>
    ///     Class for managing a collection of parts
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PieceCollection<T> : IEnumerable
    {
        public T this[AssetTypeDefinition definition]
        {
            get => GetPiece(definition);
            set => AddPiece(definition, value);
        }

        public int PieceCount => _pieceModels.Count;
        private readonly Dictionary<AssetTypeDefinition, T> _pieceModels;

        public PieceCollection()
        {
            _pieceModels = new Dictionary<AssetTypeDefinition, T>();
        }

        public IEnumerator GetEnumerator()
        {
            return _pieceModels.Values.GetEnumerator();
        }

        public bool HasPiece(AssetTypeDefinition definition)
        {
            return _pieceModels.ContainsKey(definition);
        }

        public void AddPiece(AssetTypeDefinition definition, T model)
        {
            if (!HasPiece(definition))
            {
                _pieceModels.Add(definition, model);
            }
            else
            {
                _pieceModels[definition] = model;
            }
        }

        public bool TryGetPiece(AssetTypeDefinition definition, out T model)
        {
            return _pieceModels.TryGetValue(definition, out model);
        }

        public T GetPiece(AssetTypeDefinition definition)
        {
            if (_pieceModels.ContainsKey(definition))
            {
                return _pieceModels[definition];
            }

            return default;
        }
    }
}