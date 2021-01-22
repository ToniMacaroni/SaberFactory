using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SaberFactory.Configuration
{
    internal class PluginConfig
    {
        [Ignore]
        public bool RuntimeFirstLaunch;

        public bool Enabled { get; set; } = true;

        public bool FirstLaunch { get; set; } = true;

        public bool LoadOnStart { get; set; } = true;

        public bool EnableEvents { get; set; } = true;

        [UseConverter(typeof(ListConverter<string>))]
        public List<string> Favorites { get; set; } = new List<string>();

        public void AddFavorite(string path)
        {
            if (!IsFavorite(path))
            {
                Favorites.Add(path);
            }
        }

        public void RemoveFavorite(string path)
        {
            Favorites.Remove(path);
        }

        public bool IsFavorite(string path)
        {
            return Favorites.Contains(path);
        }
    }
}