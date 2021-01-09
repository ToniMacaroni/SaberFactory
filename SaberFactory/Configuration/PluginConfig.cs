using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SaberFactory.Configuration
{
    internal class PluginConfig
    {
        public bool LoadOnStart { get; set; } = true;
    }
}