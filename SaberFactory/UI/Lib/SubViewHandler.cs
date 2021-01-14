namespace SaberFactory.UI.Lib
{
    internal class SubViewHandler
    {
        public SubView CurrentSubView { get; private set; }

        private SubView _previousSubView;

        public SubViewHandler()
        {

        }

        public void SwitchView(SubView newSubView)
        {
            if (CurrentSubView == newSubView) return;

            CurrentSubView?.Close();
            _previousSubView = CurrentSubView;
            CurrentSubView = newSubView;
            CurrentSubView.Open();
        }

        public void NotifyDidOpen()
        {
            CurrentSubView?.DidOpen();
        }

        public void NotifyDidClose()
        {
            CurrentSubView?.DidClose();
        }

        public void GoBack()
        {
            SwitchView(_previousSubView);
        }
    }
}