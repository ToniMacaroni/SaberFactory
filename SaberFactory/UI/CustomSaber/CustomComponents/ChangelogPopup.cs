using BeatSaberMarkupLanguage.Attributes;
using System.Diagnostics;
using SaberFactory.UI.Lib;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class ChangelogPopup : Popup
    {
        [UIValue("text")] private string _text { get; set; } = "Nothing here";

        private PluginManager.Release _release;

        public void Show(PluginManager.Release release)
        {
            _release = release;
            _text = release.Body;
            Show();
        }

        public new void Hide()
        {
            base.Hide();
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
