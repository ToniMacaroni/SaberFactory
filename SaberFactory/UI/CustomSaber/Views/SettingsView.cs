using System.Diagnostics;
using System.Threading;
using BeatSaberMarkupLanguage.Attributes;
using SaberFactory.Configuration;
using SaberFactory.UI.CustomSaber.CustomComponents;
using SaberFactory.UI.Lib;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.CustomSaber.Views
{
    internal class SettingsView : SubView, INavigationCategoryView
    {
        private static readonly string PROFILE_URL = "https://ko-fi.com/tonimacaroni";
        private static readonly string DISCORD_URL = "https://discord.gg/PjD7WcChH3";

        [UIComponent("changelog-popup")] private readonly ChangelogPopup _changelogPopup = null;
        [UIObject("github-button")] private readonly GameObject _githubButton = null;

        [UIValue("mod-enabled")]
        private bool ModEnabled
        {
            get => _pluginConfig.Enabled;
            set
            {
                _pluginConfig.Enabled = value;
                OnPropertyChanged();
            }
        }

        [UIValue("events-enabled")]
        private bool EventsEnabled
        {
            get => _pluginConfig.EnableEvents;
            set
            {
                _pluginConfig.EnableEvents = value;
                OnPropertyChanged();
            }
        }

        [UIValue("random-sabers")]
        private bool RandomSabers
        {
            get => _pluginConfig.RandomSaber;
            set
            {
                _pluginConfig.RandomSaber = value;
                OnPropertyChanged();
            }
        }

        [UIValue("override-song-saber")]
        private bool OverrideSongSaber
        {
            get => _pluginConfig.OverrideSongSaber;
            set
            {
                _pluginConfig.OverrideSongSaber = value;
                OnPropertyChanged();
            }
        }

        [Inject] private readonly PluginConfig _pluginConfig = null;
        [Inject] private readonly PluginManager _pluginManager = null;

        public ENavigationCategory Category => ENavigationCategory.Settings;

        public override void DidClose()
        {
            _changelogPopup.Hide();
        }

        [UIAction("#post-parse")]
        private async void Setup()
        {
            var release = await _pluginManager.GetNewestReleaseAsync(CancellationToken.None);
            if (release != null && !release.IsLocalNewest) _githubButton.SetActive(true);
        }

        [UIAction("profile-clicked")]
        private void ProfileClicked()
        {
            Process.Start(PROFILE_URL);
        }

        [UIAction("discord-clicked")]
        private void DiscordClicked()
        {
            Process.Start(DISCORD_URL);
        }

        [UIAction("github-clicked")]
        private async void GithubClicked()
        {
            var release = await _pluginManager.GetNewestReleaseAsync(CancellationToken.None);
            if (release == null) return;
            _changelogPopup.Show(release);
        }
    }
}