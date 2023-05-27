using System;
using System.Diagnostics;
using System.Threading;
using BeatSaberMarkupLanguage.Attributes;
using SaberFactory.Configuration;
using SaberFactory.Editor;
using SaberFactory.ProjectComponents;
using SaberFactory.UI.CustomSaber.CustomComponents;
using SaberFactory.UI.Lib;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.CustomSaber.Views
{
    internal class SettingsView : SubView, INavigationCategoryView
    {
        private const string ProfileUrl = "https://ko-fi.com/tonimacaroni";
        private const string DiscordUrl = "https://discord.gg/PjD7WcChH3";

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
                
        [UIValue("hmd-only")]
        private bool HMDOnly
        {
            get => _pluginConfig.HMDOnly;
            set
            {
                _pluginConfig.HMDOnly = value;
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

        private float SwingSoundVolume
        {
            get => _pluginConfig.SwingSoundVolume;
            set
            {
                _pluginConfig.SwingSoundVolume = value;
                if (_saberSound)
                {
                    _saberSound.ConfigVolume = value;
                }
                OnPropertyChanged();
            }
        }

        private bool EnableCustomBurnmarks
        {
            get => _pluginConfig.EnableCustomBurnmarks;
            set
            {
                _pluginConfig.EnableCustomBurnmarks = value;
                OnPropertyChanged();
            }
        }

        [Inject] private readonly PluginConfig _pluginConfig = null;
        [Inject] private readonly PluginManager _pluginManager = null;
        [Inject] private readonly EditorInstanceManager _editorInstanceManager = null;

        public ENavigationCategory Category => ENavigationCategory.Settings;

        private SFSaberSound _saberSound;

        public override void DidClose()
        {
            _changelogPopup.Hide();
        }

        public override void DidOpen()
        {
            _editorInstanceManager.CurrentSaber?.GetSaberComponent(out _saberSound);
        }

        [UIAction("#post-parse")]
        private async void Setup()
        {
            var release = await _pluginManager.GetNewestReleaseAsync(CancellationToken.None);
            if (release is { IsLocalNewest: false })
            {
                _githubButton.SetActive(true);
            }
        }

        [UIAction("profile-clicked")]
        private void ProfileClicked()
        {
            Process.Start(ProfileUrl);
        }

        [UIAction("discord-clicked")]
        private void DiscordClicked()
        {
            Process.Start(DiscordUrl);
        }

        [UIAction("github-clicked")]
        private async void GithubClicked()
        {
            var release = await _pluginManager.GetNewestReleaseAsync(CancellationToken.None);
            if (release == null)
            {
                return;
            }

            _changelogPopup.Show(release);
        }
    }
}