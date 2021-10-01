using BeatSaberMarkupLanguage.Attributes;
using SaberFactory.UI.Lib;

namespace SaberFactory.UI.CustomSaber.Popups
{
    internal class LoadingPopup : Popup
    {
        [UIValue("text")] private string _text;
        [UIValue("text-active")] private bool _isTextActive => !string.IsNullOrEmpty(_text);

        public void Show()
        {
            ShowInteral(string.Empty);
        }

        public void Show(string text)
        {
            ShowInteral(text);
        }

        private void ShowInteral(string text)
        {
            _text = text;
            _ = Create(false);
        }

        public async void Hide()
        {
            await Hide(false);
        }
    }
}