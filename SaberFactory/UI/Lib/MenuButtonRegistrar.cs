using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using Zenject;

namespace SaberFactory.UI.Lib
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
            MenuButtons.instance.UnregisterButton(_menuButton);
        }

        public void Initialize()
        {
            MenuButtons.instance.RegisterButton(_menuButton);
        }

        protected virtual void OnClick()
        { }
    }
}