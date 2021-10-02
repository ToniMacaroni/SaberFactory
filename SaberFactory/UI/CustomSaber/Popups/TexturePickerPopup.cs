using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
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
        private Action _onCancelCallback;

        private Action<Texture2D> _onSelectedCallback;
        private TextureAsset _selectedTextureAsset;

        private List<TextureAsset> _textureAssets;

        protected override void Awake()
        {
            base.Awake();
            _textureAssets = new List<TextureAsset>();
        }

        public async void Show(Action<Texture2D> onSelected, Action onCancel = null)
        {
            ParentToViewController();
            _onSelectedCallback = onSelected;
            _onCancelCallback = onCancel;

            await _textureStore.LoadAllTexturesAsync();

            _ = Create(false, false);
            RefreshList(_textureStore.GetAllTextures().ToList());

            await AnimateIn();
        }

        public void RefreshList(List<TextureAsset> items)
        {
            _textureAssets = items;
            var cells = new List<CustomListTableData.CustomCellInfo>();
            foreach (var textureAsset in _textureAssets)
            {
                var cell = new CustomListTableData.CustomCellInfo(textureAsset.Name, null, textureAsset.Sprite);
                cells.Add(cell);
            }

            _itemList.data = cells;
            _itemList.tableView.ReloadData();
        }

        [UIAction("click-cancel")]
        private async void ClickCancel()
        {
            await Hide(false);
            _onCancelCallback?.Invoke();
        }

        [UIAction("click-select")]
        private async void ClickSelect()
        {
            await Hide(false);
            _onSelectedCallback?.Invoke(_selectedTextureAsset?.Texture);
        }

        [UIAction("item-selected")]
        private void ItemSelected(TableView _, int row)
        {
            _selectedTextureAsset = _textureAssets[row];
        }
    }
}