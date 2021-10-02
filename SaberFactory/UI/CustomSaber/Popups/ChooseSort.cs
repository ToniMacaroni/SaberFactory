using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Attributes;
using SaberFactory.UI.CustomSaber.CustomComponents;
using SaberFactory.UI.Lib;
using UnityEngine;

namespace SaberFactory.UI.CustomSaber.Popups
{
    internal class ChooseSort : Popup
    {
        public enum ESortMode
        {
            Name,
            Date,
            Size,
            Author
        }

        [UIComponent("sort-list")] private readonly CustomList _sortList = null;

        public bool ShouldScrollToTop { get; set; } = true;

        private Action<ESortMode> _onSelectionChanged;

        public async void Show(Action<ESortMode> onSelectionChanged)
        {
            _onSelectionChanged = onSelectionChanged;

            var modes = new List<SortModeItem>();
            foreach (var mode in (ESortMode[])Enum.GetValues(typeof(ESortMode))) modes.Add(new SortModeItem(mode));

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

        private class SortModeItem : ICustomListItem
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
    }
}