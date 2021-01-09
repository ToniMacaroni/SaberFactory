using System;
using BeatSaberMarkupLanguage.Attributes;
using SaberFactory.UI.Lib;


namespace SaberFactory.UI.CustomSaber.Views
{
    internal class NavigationView : CustomViewController
    {
        public event Action<ENavigationCategory> OnCategoryChanged;
        
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
        private void Click_Saber()
        {
            ChangeCategory(ENavigationCategory.Saber);
        }

        [UIAction("Click_Trail")]
        private void Click_Trail()
        {
            ChangeCategory(ENavigationCategory.Trail);
        }

        internal enum ENavigationCategory
        {
            Saber,
            Trail
        }
    }
}
