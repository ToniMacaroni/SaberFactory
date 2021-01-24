namespace SaberFactory.UI.Lib
{
    internal class Popup : CustomParsable
    {
        protected void Show()
        {
            Parse();
        }

        protected void Hide()
        {
            UnParse();
        }
    }
}