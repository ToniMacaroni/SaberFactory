using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using SiraUtil.Tools;
using Zenject;

namespace SaberFactory.UI
{
    internal class SaberFactoryMenuButton : IInitializable, IDisposable
    {
        private readonly Editor.Editor _editor;
        private readonly SiraLog _logger;

        private readonly MenuButton _menuButton;

        private SaberFactoryMenuButton(SiraLog logger, Editor.Editor editor)
        {
            _logger = logger;
            _editor = editor;
            _menuButton = new MenuButton("Saber Factory", "Good quality sabers", OnClick);
        }

        public void Dispose()
        {
            if (MenuButtons.IsSingletonAvailable && BSMLParser.IsSingletonAvailable)
                MenuButtons.instance.UnregisterButton(_menuButton);
        }

        public void Initialize()
        {
            MenuButtons.instance.RegisterButton(_menuButton);
        }

        private void OnClick()
        {
            _editor.Open();
        }
    }
}