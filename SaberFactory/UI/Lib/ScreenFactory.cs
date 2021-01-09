using SaberFactory.Helpers;
using UnityEngine;
using VRUIControls;
using Zenject;

namespace SaberFactory.UI.Lib
{
    internal class ScreenFactory : IFactory<CustomScreen.InitData, CustomScreen>
    {
        private readonly DiContainer _container;

        private ScreenFactory(DiContainer container)
        {
            _container = container;
        }

        public CustomScreen Create(CustomScreen.InitData initData)
        {
            var go = initData.Parent.CreateGameObject(initData.Name);

            var screen = _container.InstantiateComponent<CustomScreen>(go);

            screen.Initialize(initData);

            _container.InstantiateComponent<VRGraphicRaycaster>(go);

            return screen;
        }
    }
}