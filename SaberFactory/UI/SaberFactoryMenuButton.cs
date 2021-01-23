using BeatSaberMarkupLanguage.MenuButtons;
using SiraUtil.Tools;
using System;
using Zenject;

namespace SaberFactory.UI
{
    internal class SaberFactoryMenuButton : IInitializable, IDisposable
    {
        private readonly SiraLog _logger;
        private readonly Editor.Editor _editor;

        private readonly MenuButton _menuButton;

        private SaberFactoryMenuButton(SiraLog logger, Editor.Editor editor)
        {
            _logger = logger;
            _editor = editor;
            _menuButton = new MenuButton("Saber Factory", "Good quality sabers", OnClick);
        }

        private void OnClick()
        {
            _editor.Open();
        }

        public void Initialize()
        {
            MenuButtons.instance.RegisterButton(_menuButton);
        }

        public void Dispose()
        {
            if (MenuButtons.IsSingletonAvailable && BeatSaberMarkupLanguage.BSMLParser.IsSingletonAvailable)
                MenuButtons.instance.UnregisterButton(_menuButton);
        }
    }
}