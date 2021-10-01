using System;
using UnityEngine;
using VRUIControls;
using Zenject;

namespace SaberFactory.UI.Lib
{
    internal class ViewControllerFactory : IFactory<Type, CustomViewController.InitData, CustomViewController>
    {
        private readonly DiContainer _container;

        public ViewControllerFactory(DiContainer container)
        {
            _container = container;
        }

        public CustomViewController Create(Type viewControllerType, CustomViewController.InitData initData)
        {
            var go = new GameObject(viewControllerType.Name);
            go.transform.SetParent(initData.Parent);
            go.SetActive(false);

            var canvas = go.AddComponent<Canvas>();
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Normal;
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.Tangent;

            _container.InstantiateComponent<VRGraphicRaycaster>(go);

            var vc = (CustomViewController)_container.InstantiateComponent(viewControllerType, go);

            var rt = vc.rectTransform;
            rt.localEulerAngles = Vector3.zero;
            rt.localScale = rt.anchorMax = Vector3.one;
            rt.anchorMin = rt.sizeDelta = Vector2.zero;

            return vc;
        }
    }
}