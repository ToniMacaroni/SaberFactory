namespace SaberFactory.UI.Lib
{
    internal class Popup : CustomParsable
    {
        protected void Show()
        {
            gameObject.SetActive(true);
        }

        protected void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}