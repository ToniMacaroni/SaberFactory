using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using HMUI;
using SaberFactory.UI.Lib;
using UnityEngine;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class SaberSelector : CustomUiComponent
    {
        public event Action<object> OnItemSelected;

        public object CurrentSelectedItem { get; private set; }

        [UIValue("params-title")] private string _paramsTitle;

        [UIComponent("custom-list")] private readonly CustomCellListTableData _customList = null;

        protected override void Initialize()
        {
            var options = (SaberSelectorParams) _params;
            _paramsTitle = options.Title;
        }

        public void SetItems(IEnumerable<ICustomListItem> listItems)
        {
            var content = new List<object>();
            foreach (var listItem in listItems)
            {
                content.Add(new ListItem(listItem, listItem.ListName, listItem.ListAuthor, listItem.ListCover));
            }

            _customList.data = content;
            _customList.tableView.ReloadData();
        }

        private void SelectItem(object item)
        {
            if(CurrentSelectedItem==item) return;
            CurrentSelectedItem = item;
            OnItemSelected?.Invoke(item);
        }

        [UIAction("item-selected")]
        private void Item_Selected(TableView _, ListItem item)
        {
            SelectItem(item.ContainingObject);
        }

        internal class ListItem
        {
            public object ContainingObject { get; private set; }

            [UIValue("name")]
            public string Name { get; private set; }

            [UIValue("author")]
            public string Author { get; private set; }

            [UIComponent("cover")] private readonly ImageView _cover = null;

            private readonly Sprite _coverSprite;

            public ListItem(object containingObject, string name, string author, Sprite cover)
            {
                ContainingObject = containingObject;
                Name = name;
                Author = author;
                _coverSprite = cover;
            }

            [UIAction("#post-parse")]
            private void Setup()
            {
                _cover.sprite = _coverSprite;
            }
        }

        internal struct SaberSelectorParams
        {
            public string Title;

            public SaberSelectorParams(string title)
            {
                Title = title;
            }
        }
    }
}
