using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using HMUI;
using SaberFactory.DataStore;
using SaberFactory.UI.Lib;
using UnityEngine;
using Zenject;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class TexturePickerPopup : Popup
    {
        [UIComponent("item-list")] private readonly CustomListTableData _itemList = null;

        [Inject] private readonly TextureStore _textureStore = null;

        private Action<Texture2D> _onSelectedCallback = null;
        private Action _onCancelCallback = null;

        private List<TextureAsset> _textureAssets;
        private TextureAsset _selectedTextureAsset;

        public async void Show(Action<Texture2D> onSelected, Action onCancel = null)
        {
            ParentToViewController();
            _onSelectedCallback = onSelected;
            _onCancelCallback = onCancel;

            await _textureStore.LoadAllTexturesAsync();

            Show();
            RefreshList(_textureStore.GetAllTextures().ToList());
        }

        public void RefreshList(List<TextureAsset> items)
        {
            _textureAssets = items;
            var cells = new List<CustomListTableData.CustomCellInfo>();
            foreach (var textureAsset in _textureAssets)
            {
                Sprite tex = null;
                if (textureAsset.Name != "trail2.png")
                {
                    tex = textureAsset.Sprite;
                }
                var cell = new CustomListTableData.CustomCellInfo(textureAsset.Name, null, tex);
                cells.Add(cell);
            }

            _itemList.data = cells;
            _itemList.tableView.ReloadData();
        }

        private void Awake()
        {
            _textureAssets = new List<TextureAsset>();
        }

        [UIAction("click-cancel")]
        private void ClickCancel()
        {
            Hide();
            _onCancelCallback?.Invoke();
        }

        [UIAction("click-select")]
        private void ClickSelect()
        {
            Hide();
            _onSelectedCallback?.Invoke(_selectedTextureAsset?.Texture);
        }

        [UIAction("item-selected")]
        private void ItemSelected(TableView _, int row)
        {
            _selectedTextureAsset = _textureAssets[row];
        }
    }
}
