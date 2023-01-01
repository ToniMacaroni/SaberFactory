using UnityEngine;

namespace SaberFactory.UI.Flow
{
    public interface IAssetInfo
    {
        /// <summary>
        /// Name of the asset
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Author of the asset
        /// </summary>
        string Author { get; }
        
        /// <summary>
        /// The immediate directory of the asset
        /// </summary>
        string SubDir { get; }
        
        /// <summary>
        /// Is asset prioritized
        /// </summary>
        bool IsFavorite { get; }
        
        /// <summary>
        /// Asset cover
        /// </summary>
        Sprite Cover { get; }
    }
}