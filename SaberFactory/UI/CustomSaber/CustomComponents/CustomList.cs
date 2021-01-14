using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.TypeHandlers;
using HMUI;
using SaberFactory.UI.Lib;
using TMPro;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class CustomList : CustomUiComponent
    {
        public event Action<ICustomListItem> OnItemSelected;

        private List<ICustomListItem> _listObjects;

        [UIComponent("item-list")] private readonly CustomListTableData _list = null;
        [UIComponent("header-text")] private readonly TextMeshProUGUI _textMesh = null;

        public void SetItems(IEnumerable<ICustomListItem> items)
        {
            var listItems = new List<ICustomListItem>();
            var data = new List<CustomListTableData.CustomCellInfo>();
            foreach (var item in items)
            {
                var cell = new CustomListTableData.CustomCellInfo(item.ListName, item.ListAuthor, item.ListCover);
                data.Add(cell);
                listItems.Add(item);
            }

            _list.data = data;
            _list.tableView.ReloadData();

            _listObjects = listItems;
        }

        private void SetText(string text)
        {
            _textMesh.text = text;
        }

        [UIAction("item-selected")]
        private void ItemSelected(TableView _, int row)
        {
            OnItemSelected?.Invoke(_listObjects[row]);
        }

        [ComponentHandler(typeof(CustomList))]
        internal class TypeHandler : TypeHandler<CustomList>
        {
            public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>
            {
                {"text", new[] {"text"}}
            };

            public override Dictionary<string, Action<CustomList, string>> Setters =>
                new Dictionary<string, Action<CustomList, string>>
                {
                    {"text", (list, val) => list.SetText(val)}
                };
        }
    }
}
