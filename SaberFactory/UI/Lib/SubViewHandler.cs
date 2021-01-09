namespace SaberFactory.UI.Lib
{
    internal class SubViewHandler
    {
        public SubView CurrentSubView { get; private set; }

        public SubViewHandler()
        {

        }

        public void SwitchView(SubView newSubView)
        {
            if (CurrentSubView == newSubView) return;

            CurrentSubView?.Close();
            CurrentSubView = newSubView;
            CurrentSubView.Open();
        }
    }
}