using SaberFactory.Models;
using UnityEngine;
using Zenject;

namespace SaberFactory.Instances
{
    internal class BasePieceInstance
    {
        public GameObject GameObject;
        public Transform CachedTransform;

        private BasePieceInstance(BasePieceModel model)
        {
            GameObject = new GameObject("Saber Piece");
            CachedTransform = GameObject.transform;
        }

        public void SetParent(Transform parent)
        {
            CachedTransform.SetParent(parent);
        }

        internal class Factory : PlaceholderFactory<BasePieceModel, BasePieceInstance> {}
    }
}