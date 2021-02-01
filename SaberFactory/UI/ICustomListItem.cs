using UnityEngine;

namespace SaberFactory.UI
{
    /// <summary>
    /// Interface to provide information shown in a custom list
    /// </summary>
    internal interface ICustomListItem
    {
        string ListName { get; }
        string ListAuthor { get; }
        Sprite ListCover { get; }
        bool IsFavorite { get; }
    }
}