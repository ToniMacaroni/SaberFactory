using UnityEngine;

namespace SaberFactory.UI
{
    internal interface ICustomListItem
    {
        string ListName { get; }
        string ListAuthor { get; }
        Sprite ListCover { get; }
    }
}