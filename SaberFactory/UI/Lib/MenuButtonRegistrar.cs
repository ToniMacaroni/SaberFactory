using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using Zenject;

namespace SaberFactory.UI
{
    internal class MenuButtonRegistrar : IInitializable, IDisposable
    {
        private readonly MenuButton _menuButton;

        protected MenuButtonRegistrar(string buttonText, string hoverText)
        {
            _menuButton = new MenuButton(buttonText, hoverText, OnClick);
        }

        public void Dispose()
        {
            if (MenuButtons.IsSingletonAvailable && BSMLParser.IsSingletonAvailable)
            {
                MenuButtons.instance.UnregisterButton(_menuButton);
            }
        }

        public void Initialize()
        {
            MenuButtons.instance.RegisterButton(_menuButton);
        }

        protected virtual void OnClick()
        {
        }
    }
}