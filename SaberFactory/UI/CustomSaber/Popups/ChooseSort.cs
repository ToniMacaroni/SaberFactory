using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Collections.Generic;
using SaberFactory.UI.Lib;
using UnityEngine;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class ChooseSort : Popup
    {
        [UIComponent("sort-list")] private readonly CustomList _sortList = null;

        private Action<ESortMode> _onSelectionChanged;

        public async void Show(Action<ESortMode> onSelectionChanged)
        {
            _onSelectionChanged = onSelectionChanged;

            var modes = new List<SortModeItem>();
            foreach (var mode in (ESortMode[])Enum.GetValues(typeof(ESortMode)))
            {
                modes.Add(new SortModeItem(mode));
            }

            _ = Create(true);
            _sortList.OnItemSelected += SortSelected;
            _sortList.SetItems(modes);

            await AnimateIn();
        }

        private void SortSelected(ICustomListItem item)
        {
            _onSelectionChanged?.Invoke(((SortModeItem)item).SortMode);
            Exit();
        }

        private async void Exit()
        {
            _onSelectionChanged = null;

            _sortList.OnItemSelected -= SortSelected;
            await Hide(true);
        }

        [UIAction("click-cancel")]
        private void ClickSelect()
        {
            Exit();
        }

        class SortModeItem : ICustomListItem
        {
            public readonly ESortMode SortMode;

            public SortModeItem(ESortMode sortMode)
            {
                SortMode = sortMode;
            }

            public string ListName => SortMode.ToString();
            public string ListAuthor { get; }
            public Sprite ListCover { get; }
            public bool IsFavorite { get; }
        }

        public enum ESortMode
        {
            Name,
            Date,
            Size,
            Author
        }
    }
}
