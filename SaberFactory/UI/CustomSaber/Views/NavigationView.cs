using System;
using System.Collections.Generic;
using System.Threading;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using SaberFactory.UI.CustomSaber.CustomComponents;
using SaberFactory.UI.Lib;
using TMPro;
using UnityEngine;
using Zenject;


namespace SaberFactory.UI.CustomSaber.Views
{
    internal class NavigationView : CustomViewController
    {
        public event Action<ENavigationCategory> OnCategoryChanged;
        public event Action OnExit;

        public ENavigationCategory CurrentCategory { get; private set; }

        [Inject] private readonly PluginManager _pluginManager = null;

        [UIComponent("settings-notify-text")] private readonly TextMeshProUGUI _settingsNotifyText = null;
        [UIValue("nav-buttons")] private List<object> _navButtons;
        [UIObject("exit_btn")] private readonly GameObject _exitBtn = null;

        private NavButton _currentSelectedNavButton;

        [UIAction("#post-parse")]
        private async void Setup()
        {
            // why is this icon so fucking big
            _exitBtn.transform.Find("Content").GetComponent<StackLayoutGroup>().padding = new RectOffset(3, 3, 3, 3);

            if (_navButtons is {} && _navButtons.Count > 0)
            {
                _currentSelectedNavButton = ((NavButtonWrapper) _navButtons[0]).NavButton;
                _currentSelectedNavButton.SetState(true, false);
            }

            var release = await _pluginManager.GetNewestReleaseAsync(CancellationToken.None);

            if (release is {IsLocalNewest: false})
            {
                _settingsNotifyText.gameObject.SetActive(true);
            }
        }

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

            _navButtons.Add(saberButton);
            _navButtons.Add(trailButton);
        }

        private void ClickedCategory(NavButton button, ENavigationCategory category)
        {
            if(_currentSelectedNavButton==button) return;
            _currentSelectedNavButton?.Deselect();
            _currentSelectedNavButton = button;
            CurrentCategory = category;
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

        public override IAnimatableUi.EAnimationType AnimationType => IAnimatableUi.EAnimationType.Vertical;
    }
}
