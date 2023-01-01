namespace SaberFactory.Helpers
{
    internal static class MaterialAttributes
    {
        /// <summary>
        /// Don't show a preview of the texture in material editors
        /// </summary>
        public const string SFNoPreview = "SFNoPreview";
        
        /// <summary>
        /// Don't show the Property in material editors
        /// This also prevents the Property from being copied over to the other saber
        /// </summary>
        public const string SFHide = "SFHide";
        
        /// <summary>
        /// Show the Property as a toggle in material editors
        /// </summary>
        public const string SFToggle = "SFToggle";
    }
}