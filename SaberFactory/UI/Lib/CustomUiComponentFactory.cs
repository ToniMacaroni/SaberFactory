using System;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Lib
{
    internal class CustomUiComponentFactory : IFactory<Type, Transform, object, CustomUiComponent>
    {
        private readonly DiContainer _container;

        private CustomUiComponentFactory(DiContainer container)
        {
            _container = container;
        }

        public CustomUiComponent Create(Type componentType, Transform parent, object componentParams)
        {
            //var go = parent.CreateGameObject(componentType.Name);
            var comp = (CustomUiComponent) _container.InstantiateComponent(componentType, parent.gameObject);
            comp.Construct(componentParams);

            return comp;
        }
    }
}