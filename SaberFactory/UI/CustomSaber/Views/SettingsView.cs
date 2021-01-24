using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using SaberFactory.Configuration;
using SaberFactory.UI.Lib;
using Zenject;


namespace SaberFactory.UI.CustomSaber.Views
{
    internal class SettingsView : SubView, INotifyPropertyChanged, INavigationCategoryView
    {
        private static readonly string PROFILE_URL = "https://ko-fi.com/tonimacaroni";
        private static readonly string DISCORD_URL = "https://discord.gg/PjD7WcChH3";

        public ENavigationCategory Category => ENavigationCategory.Settings;

        [Inject] private readonly PluginConfig _pluginConfig = null;


        [UIValue("mod-enabled")]
        private bool _modEnabled
        {
            get => _pluginConfig.Enabled;
            set
            {
                _pluginConfig.Enabled = value;
                OnPropertyChanged();
            }
        }

        [UIValue("events-enabled")]
        private bool _eventsEnabled
        {
            get => _pluginConfig.EnableEvents;
            set
            {
                _pluginConfig.EnableEvents = value;
                OnPropertyChanged();
            }
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

        public event PropertyChangedEventHandler PropertyChanged;


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
