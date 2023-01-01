using System;
using FlowUi.Runtime;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Flow
{
    public partial class SaberCell : FlowTableCell
    {
        [SerializeField] private ImageView coverImage;
        [SerializeField] private TextMeshProUGUI nameTextmesh;
        [SerializeField] private TextMeshProUGUI authorTextmesh;
        [SerializeField] private ImageView favoriteImage;
        
        [Space]
        [SerializeField] private Button favoriteButton;
        [SerializeField] private Button deleteButton;
        
        public Button FavoriteButton => favoriteButton;
        public Button DeleteButton => deleteButton;
        
        public Action OnFavorite;
        public Action OnReload;
        public Action OnDelete;

        public void FavoritePressed()
        {
            OnFavorite?.Invoke();
        }
        
        public void ReloadPressed()
        {
            OnReload?.Invoke();
        }
        
        public void DeletePressed()
        {
            OnDelete?.Invoke();
        }
    }
}