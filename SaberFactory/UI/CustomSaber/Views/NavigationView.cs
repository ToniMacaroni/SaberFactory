using System;
using System.Collections.Generic;
using System.Threading;
using BeatSaberMarkupLanguage.Attributes;
using SaberFactory.UI.Lib;
using TMPro;
using Zenject;


namespace SaberFactory.UI.CustomSaber.Views
{
    internal class NavigationView : CustomViewController
    {
        [Inject] private readonly PluginManager _pluginManager = null;

        public event Action<ENavigationCategory> OnCategoryChanged;
        public event Action OnExit;
        
        public ENavigationCategory CurrentCategory { get; private set; }

        [UIComponent("settings-notify-text")] private readonly TextMeshProUGUI _settingsNotifyText = null;
        [UIValue("nav-buttons")] private List<object> _navButtons;

        [UIAction("#post-parse")]
        private async void Setup()
        {
            var release = await _pluginManager.GetNewestReleaseAsync(CancellationToken.None);
            if (release!=null && !release.IsLocalNewest)
            {
                _settingsNotifyText.gameObject.SetActive(true);
            }
        }

        private void Awake()
        {
            _navButtons = new List<object>();
            _navButtons.Add(new NavigationButton(ENavigationCategory.Saber, "SaberFactory.Resources.Icons.customsaber-icon.png", ClickedCategory, "Select a saber"));
            _navButtons.Add(new NavigationButton(ENavigationCategory.Trail, "SaberFactory.Resources.Icons.trail-icon.png", ClickedCategory, "Edit the trail"));
            //_navButtons.Add(new NavigationButton(ENavigationCategory.Settings, "SaberFactory.Resources.Icons.cog.png", ClickedCategory, "Setup Saber Factory"));
            //_navButtons.Add(new NavigationButton(ENavigationCategory.Transform, "SaberFactory.Resources.Icons.customsaber-icon.png", ClickedCategory));
        }

        private void ClickedCategory(ENavigationCategory cateogory)
        {
            CurrentCategory = cateogory;
            OnCategoryChanged?.Invoke(cateogory);
        }

        [UIAction("Click_Settings")]
        private void ClickSettings()
        {
            ClickedCategory(ENavigationCategory.Settings);
        }

        [UIAction("Click_Exit")]
        private void ClickExit()
        {
            OnExit?.Invoke();
        }
    }
}
