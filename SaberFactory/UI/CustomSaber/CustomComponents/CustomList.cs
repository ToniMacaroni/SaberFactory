using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.TypeHandlers;
using HMUI;
using SaberFactory.UI.Lib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CustomListTableData = SaberFactory.UI.Lib.BSML.CustomListTableData;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class CustomList : CustomUiComponent
    {
        public event Action<ICustomListItem> OnItemSelected;

        [UIComponent("root-vertical")] private readonly LayoutElement _layoutElement = null;
        [UIComponent("item-list")] private readonly CustomListTableData _list = null;
        [UIComponent("header-text")] private readonly TextMeshProUGUI _textMesh = null;

        private List<ICustomListItem> _listObjects;
        private int _currentIdx = -1;

        public void SetItems(IEnumerable<ICustomListItem> items)
        {
            var listItems = new List<ICustomListItem>();
            var data = new List<CustomListTableData.CustomCellInfo>();
            foreach (var item in items)
            {
                var cell = new CustomListTableData.CustomCellInfo
                {
                    Text = item.ListName,
                    Subtext = item.ListAuthor,
                    Icon = item.ListCover,
                    IsFavorite = item.IsFavorite
                };

                data.Add(cell);
                listItems.Add(item);
            }

            _list.data = data;
            _list.tableView.ReloadData();

            _listObjects = listItems;
        }

        public void Reload()
        {
            SetItems(_listObjects);
        }

        public void Select(ICustomListItem item, bool scroll = true)
        {
            if (item == null || _listObjects == null) return;
            var idx = _listObjects.IndexOf(item);
            Select(idx, scroll);
        }

        public void Select(string listName, bool scroll = true)
        {
            if (string.IsNullOrEmpty(listName)) return;
            var item = _listObjects.FirstOrDefault(x => x.ListName == listName);
            if (item == null) return;
            Select(item, scroll);
        }

        public void Select(int idx, bool scroll = true)
        {
            if (idx == -1 || idx == _currentIdx) return;
            _list.tableView.SelectCellWithIdx(idx);
            _currentIdx = idx;
            if(scroll) _list.tableView.ScrollToCellWithIdx(idx, TableView.ScrollPositionType.Beginning, false);
        }

        private void SetWidth(float width)
        {
            _layoutElement.preferredWidth = width;
        }

        private void SetText(string text)
        {
            _textMesh.text = text;
        }

        private void SetBgColor(Color color)
        {
            _layoutElement.gameObject.GetComponent<Backgroundable>().background.color = color;
        }

        [UIAction("item-selected")]
        private void ItemSelected(TableView _, int row)
        {
            _currentIdx = row;
            OnItemSelected?.Invoke(_listObjects[row]);
        }

        [ComponentHandler(typeof(CustomList))]
        internal class TypeHandler : TypeHandler<CustomList>
        {
            public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>
            {
                {"title", new[] {"title", "header"}},
                {"width", new[] {"width"}},
                {"bgColor", new []{"bg-color"} }
            };

            public override Dictionary<string, Action<CustomList, string>> Setters =>
                new Dictionary<string, Action<CustomList, string>>
                {
                    {"title", (list, val) => list.SetText(val)},
                    {"width", (list, val) => list.SetWidth(float.Parse(val)) },
                    {"bgColor", SetBgColor }
                };

            private void SetBgColor(CustomList list, string hex)
            {
                ColorUtility.TryParseHtmlString(hex, out var color);
                list.SetBgColor(color);
            }
        }
    }
}
