using System;
using System.Collections.Generic;
using System.Threading;
using BeatSaberMarkupLanguage.Attributes;
using SaberFactory.UI.CustomSaber.CustomComponents;
using SaberFactory.UI.Lib;
using TMPro;
using Zenject;

namespace SaberFactory.UI.CustomSaber.Views
{
    internal class NavigationView : CustomViewController
    {
        [UIComponent("settings-notify-text")] private readonly TextMeshProUGUI _settingsNotifyText = null;
        [UIValue("nav-buttons")] private List<object> _navButtons;

        public override IAnimatableUi.EAnimationType AnimationType => IAnimatableUi.EAnimationType.Vertical;

        [Inject] private readonly PluginManager _pluginManager = null;

        private NavButton _currentSelectedNavButton;

        private void Awake()
        {
            _navButtons = new List<object>();

            var saberButton = new NavButtonWrapper(
                ENavigationCategory.Saber,
                "SaberFactory.Resources.Icons.customsaber-icon.png",
                ClickedCategory,
                "Select a saber");

            var trailButton = new NavButtonWrapper(
                ENavigationCategory.Trail,
                "SaberFactory.Resources.Icons.trail-icon.png",
                ClickedCategory,
                "Edit the trail");

            var transformButton = new NavButtonWrapper(
                ENavigationCategory.Transform,
                "SaberFactory.Resources.Icons.transform-icon.png",
                ClickedCategory,
                "Transform settings");

            var propButton = new NavButtonWrapper(
                ENavigationCategory.Modifier,
                "SaberFactory.Resources.Icons.wrench.png",
                ClickedCategory,
                "Saber modifier");

            _navButtons.Add(saberButton);
            _navButtons.Add(trailButton);
            _navButtons.Add(transformButton);
            _navButtons.Add(propButton);
        }

        public event Action<ENavigationCategory> OnCategoryChanged;
        public event Action OnExit;

        [UIAction("#post-parse")]
        private async void Setup()
        {
            if (_navButtons is { } && _navButtons.Count > 0)
            {
                _currentSelectedNavButton = ((NavButtonWrapper)_navButtons[0]).NavButton;
                _currentSelectedNavButton.SetState(true, false);
            }

            var release = await _pluginManager.GetNewestReleaseAsync(CancellationToken.None);

            if (release is { IsLocalNewest: false })
            {
                _settingsNotifyText.gameObject.SetActive(true);
            }
        }

        private void ClickedCategory(NavButton button, ENavigationCategory category)
        {
            if (_currentSelectedNavButton == button)
            {
                return;
            }

            if (_currentSelectedNavButton != null)
            {
                _currentSelectedNavButton.Deselect();
            }

            _currentSelectedNavButton = button;
            OnCategoryChanged?.Invoke(category);
        }

        [UIAction("clicked-settings")]
        private void ClickSettings(NavButton button, string _)
        {
            ClickedCategory(button, ENavigationCategory.Settings);
        }

        [UIAction("clicked-exit")]
        private void ClickExit()
        {
            OnExit?.Invoke();
        }
    }
}