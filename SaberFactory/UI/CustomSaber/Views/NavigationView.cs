using System;
using BeatSaberMarkupLanguage.Attributes;
using SaberFactory.UI.Lib;


namespace SaberFactory.UI.CustomSaber.Views
{
    internal class NavigationView : CustomViewController
    {
        public event Action<ENavigationCategory> OnCategoryChanged;
        public event Action OnExit;
        
        public ENavigationCategory CurrentCategory { get; private set; }

        [UIAction("#post-parse")]
        private void Setup()
        {

        }

        private void ChangeCategory(ENavigationCategory category)
        {
            CurrentCategory = category;
            OnCategoryChanged?.Invoke(category);
        }

        [UIAction("Click_Saber")]
        private void ClickSaber()
        {
            ChangeCategory(ENavigationCategory.Saber);
        }

        [UIAction("Click_Trail")]
        private void ClickTrail()
        {
            ChangeCategory(ENavigationCategory.Trail);
        }

        [UIAction("Click_Exit")]
        private void ClickExit()
        {
            OnExit?.Invoke();
        }

        internal enum ENavigationCategory
        {
            Saber,
            Transform,
            Trail
        }
    }
}
