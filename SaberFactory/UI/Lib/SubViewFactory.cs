using System;
using SaberFactory.Helpers;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Lib
{
    internal class SubViewFactory : IFactory<Type, SubView.InitData, SubView>
    {
        private readonly DiContainer _container;

        private SubViewFactory(DiContainer container)
        {
            _container = container;
        }

        public SubView Create(Type subViewType, SubView.InitData initData)
        {
            var go = initData.Parent.CreateGameObject(initData.Name);

            var rt = go.AddComponent<RectTransform>();
            rt.localEulerAngles = Vector3.zero;
            rt.localScale = rt.anchorMax = Vector3.one;
            rt.anchorMin = rt.sizeDelta = Vector2.zero;

            go.AddComponent<CanvasGroup>();

            var subView = (SubView)_container.InstantiateComponent(subViewType, go);
            return subView;
        }
    }
}