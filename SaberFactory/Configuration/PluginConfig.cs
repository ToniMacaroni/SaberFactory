using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using JetBrains.Annotations;
using SaberFactory.UI.Flow;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace SaberFactory.Configuration
{
    internal class PluginConfig : INotifyPropertyChanged
    {
        public bool Enabled { get; set; } = true;

        // is used to check if it's the user's first time
        // launching the mod
        public bool FirstLaunch { get; set; } = true;

        // Use your own saber instead of the one dictated by the song
        public bool OverrideSongSaber { get; set; } = false;

        // Randomize saber on each song start
        public bool RandomSaber { get; set; } = false;

        public bool AnimateSaberSelection { get; set; } = true;

        // How far does the trail width slider go
        public float TrailWidthMax { get; set; } = 1;

        // How far does the global saber width slider go
        public float GlobalSaberWidthMax { get; set; } = 3;

        public bool ShowAdvancedTrailSettings { get; set; } = false;

        public bool ShowDownloadableSabers { get; set; } = true;

        public bool ShowGameplaySettingsButton { get; set; } = true;

        public float SaberAudioVolumeMultiplier { get; set; } = 1;

        public bool ReloadOnSaberUpdate { get; set; } = false;

        public float SwingSoundVolume { get; set; } = 1;

        public bool EnableCustomBurnmarks { get; set; } = true;
        
        public float SaberSelectionBackgroundOpacity { get; set; } = 0.1f;
        
        public int SaberSelectionBackgroundBlurAmount { get; set; } = 4;

        public string LoadedPreset { get; set; } = "default.sfps";

        public string MonitorPreset { get; set; } = "";

        // Which type to use with the mod (parts / custom sabers)
        [UseConverter(typeof(EnumConverter<EAssetTypeConfiguration>))]
        public EAssetTypeConfiguration AssetType { get; set; } = EAssetTypeConfiguration.None;

        // List of sabers / parts marked as favorite
        [UseConverter(typeof(ListConverter<string>))]
        public List<string> Favorites { get; set; } = new List<string>();
        
        [UseConverter(typeof(EventSettingsConverter))]
        public EventSettings EventSettings { get; set; } = new();

        [Ignore] public bool RuntimeFirstLaunch;

        /// <summary>
        ///     Add an asset to the favorites list
        /// </summary>
        /// <param name="path"></param>
        public void AddFavorite(string path)
        {
            if (!IsFavorite(path))
            {
                Favorites.Add(path);
                Changed();
            }
        }

        /// <summary>
        ///     Remove an asset from the favorites list
        /// </summary>
        /// <param name="path"></param>
        public void RemoveFavorite(string path)
        {
            Favorites.Remove(path);
            Changed();
        }

        /// <summary>
        ///     Check if an asset is marked as favorite
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool IsFavorite(string path)
        {
            return Favorites.Contains(path);
        }
        
        // Is used to notify a change to the config
        public virtual void Changed()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}