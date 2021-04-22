using BeatSaberMarkupLanguage.Attributes;
using System.Diagnostics;
using SaberFactory.UI.Lib;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class ChangelogPopup : Popup
    {
        [UIValue("text")] private string _text { get; set; } = "Nothing here";

        private PluginManager.Release _release;

        public async void Show(PluginManager.Release release)
        {
            _release = release;
            _text = release.Body;
            _ = Create(true);
            await AnimateIn();
        }

        public async void Hide()
        {
            await Hide(true);
        }

        [UIAction("clicked-close")]
        private void ClickedClose()
        {
            Hide();
        }

        [UIAction("clicked-open")]
        private void ClickedOpen()
        {
            Process.Start(_release.Url);
        }
    }
}
