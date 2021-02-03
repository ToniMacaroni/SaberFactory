using System;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Lib
{
    internal class ComponentPlaceholderFactory<TValue> : PlaceholderFactory<GameObject, Type, TValue> where TValue : Component
    {
        [Inject] private readonly DiContainer _container = null;

        public override TValue Create(GameObject gameObject, Type type)
        {
            return (TValue)_container.InstantiateComponent(type, gameObject);
        }
    }
}