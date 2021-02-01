namespace SaberFactory.UI.Lib
{
    /// <summary>
    /// Interface for an ui object that is parsable through bsml
    /// </summary>
    internal interface ICustomParsable
    {
        /// <summary>
        /// Create the object
        /// </summary>
        void Parse();

        /// <summary>
        /// Destroy the ui object
        /// </summary>
        void Unparse();
    }
}