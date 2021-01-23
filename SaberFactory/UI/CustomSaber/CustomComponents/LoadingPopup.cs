using BeatSaberMarkupLanguage.Attributes;
using SaberFactory.UI.Lib;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class LoadingPopup : Popup
    {
        [UIAction("#post-parse")]
        private void Setup()
        {
            Hide();
        }

        public new void Show()
        {
            base.Show();
        }

        public new void Hide()
        {
            base.Hide();
        }
    }
}
