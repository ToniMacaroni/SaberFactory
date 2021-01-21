using System;
using BeatSaberMarkupLanguage.Attributes;
using SaberFactory.UI.Lib;
using UnityEngine;


namespace SaberFactory.UI.CustomSaber.Views
{
    internal class MainView : CustomViewController
    {
        #region SubViews

        private SaberSelectorView _saberSelectorView;
        private TrailSettingsView _trailSettingsView;
        private TransformSettingsView _transformSettingsView;

        #endregion

        [UIComponent("SubViewContainer")] private readonly Transform _subViewContainer = null;

        [UIAction("#post-parse")]
        private void Setup()
        {
            _saberSelectorView = CreateSubView<SaberSelectorView>(_subViewContainer, switchToView: true);
            _trailSettingsView = CreateSubView<TrailSettingsView>(_subViewContainer);
            _transformSettingsView = CreateSubView<TransformSettingsView>(_subViewContainer);
        }

        public void ChangeCategory(NavigationView.ENavigationCategory category)
        {
            switch (category)
            {
                case NavigationView.ENavigationCategory.Saber:
                    _subViewHandler.SwitchView(_saberSelectorView);
                    break;
                case NavigationView.ENavigationCategory.Transform:
                    _subViewHandler.SwitchView(_transformSettingsView);
                    break;
                case NavigationView.ENavigationCategory.Trail:
                    _subViewHandler.SwitchView(_trailSettingsView);
                    break;
            }
        }
    }
}
