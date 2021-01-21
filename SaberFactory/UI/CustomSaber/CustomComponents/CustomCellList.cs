using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.TypeHandlers;
using HMUI;
using SaberFactory.UI.Lib;
using TMPro;
using UnityEngine;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class CustomCellList : CustomUiComponent
    {
        public event Action<ICustomListItem> OnItemSelected;

        public object CurrentSelectedItem { get; private set; }

        [UIComponent("header-text")] private readonly TextMeshProUGUI _headerText = null;
        [UIComponent("item-list")] private readonly CustomCellListTableData _customList = null;

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

        public void SetHeader(string header)
        {
            _headerText.text = header;
        }

        private void SelectItem(ICustomListItem item)
        {
            if (CurrentSelectedItem == item) return;
            CurrentSelectedItem = item;
            OnItemSelected?.Invoke(item);
        }

        [UIAction("item-selected")]
        private void Item_Selected(TableView _, ListItem item)
        {
            SelectItem((ICustomListItem)item.ContainingObject);
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
                if(_cover) _cover.sprite = _coverSprite;
            }
        }

        [ComponentHandler(typeof(CustomCellList))]
        internal class TypeHander : TypeHandler<CustomCellList>
        {
            public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>
            {
                {"title", new []{"title", "header"} }
            };

            public override Dictionary<string, Action<CustomCellList, string>> Setters =>
                new Dictionary<string, Action<CustomCellList, string>>
                {
                    {"title", (list, val) => list.SetHeader(val) }
                };
        }
    }
}
