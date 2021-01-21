using SaberFactory.Instances.Setters;
using SaberFactory.Models;
using UnityEngine;
using Zenject;

namespace SaberFactory.Instances
{
    internal class BasePieceInstance
    {
        public PropertyBlockSetterHandler PropertyBlockSetterHandler { get; protected set; }

        public readonly BasePieceModel Model;
        public GameObject GameObject;
        public Transform CachedTransform;

        protected BasePieceInstance(BasePieceModel model)
        {
            Model = model;
            GameObject = Instantiate();
            CachedTransform = GameObject.transform;
        }

        public void SetParent(Transform parent)
        {
            CachedTransform.SetParent(parent, false);
        }

        protected virtual GameObject Instantiate()
        {
            return new GameObject("BasePiece");
        }

        public virtual PartEvents GetEvents()
        {
            return null;
        }

        internal class Factory : PlaceholderFactory<BasePieceModel, BasePieceInstance> {}
    }
}