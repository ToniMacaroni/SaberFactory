namespace SaberFactory.Configuration
{
    /// <summary>
    /// Addiditonal global trail settings
    /// </summary>
    public class TrailConfig : ConfigBase
    {
        public int Granularity { get; set; } = 70;

        public int SamplingFrequency { get; set; } = 90;

        public bool OnlyUseVertexColor { get; set; } = true;

        public TrailConfig(PluginDirectories pluginDirs) : base(pluginDirs, "TrailConfig.json")
        { }
    }
}