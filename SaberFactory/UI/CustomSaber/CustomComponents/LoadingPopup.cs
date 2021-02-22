using SaberFactory.UI.Lib;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class LoadingPopup : Popup
    {
        public void Show()
        {
            Create();
        }

        public async void Hide()
        {
            await Hide(false);
        }
    }
}
