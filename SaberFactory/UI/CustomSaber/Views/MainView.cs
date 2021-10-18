using System.Collections.Generic;
using BeatSaberMarkupLanguage.Attributes;
using SaberFactory.UI.Lib;
using UnityEngine;

namespace SaberFactory.UI.CustomSaber.Views
{
    internal class MainView : CustomViewController
    {
        [UIComponent("SubViewContainer")] private readonly Transform _subViewContainer = null;

        private Dictionary<ENavigationCategory, INavigationCategoryView> _navViews;

        [UIAction("#post-parse")]
        private void Setup()
        {
            _navViews = new Dictionary<ENavigationCategory, INavigationCategoryView>();

            _saberSelectorView = AddView<SaberSelectorView>(true);
            _trailSettingsView = AddView<TrailSettingsView>();
            _settingsView = AddView<SettingsView>();
            _transformSettingsView = AddView<TransformSettingsView>();
            _modifiersSelectionView = AddView<MainModifierPanelView>();
        }

        public void ChangeCategory(ENavigationCategory category)
        {
            if (_navViews.TryGetValue(category, out var view))
                if (view is SubView subView)
                    SubViewSwitcher.SwitchView(subView);
        }

        private T AddView<T>(bool switchToView = false, Transform container = null) where T : SubView
        {
            var view = CreateSubView<T>(container ?? _subViewContainer, switchToView);
            if (view is INavigationCategoryView navView) _navViews.Add(navView.Category, navView);

            if (!switchToView) view.gameObject.SetActive(false);

            return view;
        }

        #region SubViews

        private SaberSelectorView _saberSelectorView;
        private TrailSettingsView _trailSettingsView;
        private SettingsView _settingsView;
        private TransformSettingsView _transformSettingsView;
        private MainModifierPanelView _modifiersSelectionView;

        #endregion
    }
}