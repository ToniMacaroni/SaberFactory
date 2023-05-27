using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using JetBrains.Annotations;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace SaberFactory.Configuration
{
    internal class PluginConfig : INotifyPropertyChanged
    {
        public bool Enabled { get; set; } = true;

        // Only show custom sabers within the HMD View
        public bool HMDOnly { get; set; } = false;

        // is used to check if it's the user's first time
        // launching the mod
        public bool FirstLaunch { get; set; } = true;

        // Enable saber events
        public bool EnableEvents { get; set; } = true;

        // Use your own saber instead of the one dictated by the song
        public bool OverrideSongSaber { get; set; } = false;

        // Randomize saber on each song start
        public bool RandomSaber { get; set; } = false;

        public bool AnimateSaberSelection { get; set; } = true;

        // How far does the trail width slider go
        public float TrailWidthMax { get; set; } = 1;

        // How far does the global saber width slider go
        public float GlobalSaberWidthMax { get; set; } = 3;

        // Show additional trail settings
        public bool ShowAdvancedTrailSettings { get; set; } = false;

        // Show downloadable (and featured) saber on the top of the saber selection list
        public bool ShowDownloadableSabers { get; set; } = true;

        // Not used anymore
        public bool AutoUpdateTrail { get; set; } = true;

        // Show the the "sabers" button in the gameplay settings (button beside "colors")
        public bool ShowGameplaySettingsButton { get; set; } = true;

        // Control the trail width and length with the thumbstick when in the trail editor
        public bool ControlTrailWithThumbstick { get; set; } = true;

        // A multiplier for the sounds a saber may have (e.g. plasma katana startup sound)
        public float SaberAudioVolumeMultiplier { get; set; } = 1;

        // Show a special background during events like halloween
        public bool SpecialBackground { get; set; } = true;

        // First color of the gradiant of items in a the saber list
        [UseConverter(typeof(HexColorConverter))]
        public Color ListCellColor0 { get; set; } = new Color(0.047f, 0.471f, 0.949f);
        
        // Second color of the gradiant of items in a the saber list
        [UseConverter(typeof(HexColorConverter))]
        public Color ListCellColor1 { get; set; } = new Color(0.875f, 0.086f, 0.435f);

        // Automatically reload the saber when the file changes (saber needs to be selected)
        public bool ReloadOnSaberUpdate { get; set; } = false;

        // Volume of the saber swing sound (saber needs to implement it)
        public float SwingSoundVolume { get; set; } = 1;

        // Allow the custom burnmarks feature (saber needs to implement it)
        public bool EnableCustomBurnmarks { get; set; } = true;

        // How many threads to spawn when loading all sabers
        // ! Not used as of right now !
        [Ignore] public int LoadingThreads { get; set; } = 2;


        // Which type to use with the mod (parts / custom sabers)
        [UseConverter(typeof(EnumConverter<EAssetTypeConfiguration>))]
        public EAssetTypeConfiguration AssetType { get; set; } = EAssetTypeConfiguration.None;

        // List of sabers / parts marked as favorite
        [UseConverter(typeof(ListConverter<string>))]
        public List<string> Favorites { get; set; } = new List<string>();

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
            }
        }

        /// <summary>
        ///     Remove an asset from the favorites list
        /// </summary>
        /// <param name="path"></param>
        public void RemoveFavorite(string path)
        {
            Favorites.Remove(path);
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}