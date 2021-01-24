using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;
using HMUI;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Lib
{
    [ComponentHandler(typeof(PropListTableData))]
    public class PropListTableDataHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "selectCell", new[]{ "select-cell" } },
            { "visibleCells", new[]{ "visible-cells"} },
            { "cellSize", new[]{ "cell-size"} },
            { "id", new[]{ "id" } },
            { "data", new[] { "data", "content" } },
            { "listWidth", new[] { "list-width" } },
            { "listHeight", new[] { "list-height" } },
            { "listDirection", new[] { "list-direction" } },
            { "alignCenter", new[] { "align-to-center" } }
        };

        public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            var tableData = (PropListTableData)componentType.component;

            //if (componentType.data.TryGetValue("selectCell", out string selectCell))
            //{
            //    tableData.tableView.didSelectCellWithIdxEvent += delegate (TableView table, int index)
            //    {
            //        if (!parserParams.actions.TryGetValue(selectCell, out BSMLAction action))
            //            throw new Exception("select-cell action '" + componentType.data["onClick"] + "' not found");

            //        action.Invoke(table, index);
            //    };
            //}

            if (componentType.data.TryGetValue("listDirection", out string listDirection))
                tableData.tableView.SetField("_tableType", (TableView.TableType)Enum.Parse(typeof(TableView.TableType), listDirection));

            if (componentType.data.TryGetValue("cellSize", out string cellSize))
                tableData.cellSize = Parse.Float(cellSize);

            if (componentType.data.TryGetValue("alignCenter", out string alignCenter))
                tableData.tableView.SetField("_alignToCenter", Parse.Bool(alignCenter));


            if (componentType.data.TryGetValue("data", out string value))
            {
                if (!parserParams.values.TryGetValue(value, out BSMLValue contents))
                    throw new Exception("value '" + value + "' not found");
                tableData.data = contents.GetValue() as List<PropertyDescriptor>;
                tableData.tableView.ReloadData();
            }

            switch (tableData.tableView.tableType)
            {
                case TableView.TableType.Vertical:
                    (componentType.component.gameObject.transform as RectTransform).sizeDelta = new Vector2(componentType.data.TryGetValue("listWidth", out string vListWidth) ? Parse.Float(vListWidth) : 60, tableData.cellSize * (componentType.data.TryGetValue("visibleCells", out string vVisibleCells) ? Parse.Float(vVisibleCells) : 7));
                    break;
                case TableView.TableType.Horizontal:
                    (componentType.component.gameObject.transform as RectTransform).sizeDelta = new Vector2(tableData.cellSize * (componentType.data.TryGetValue("visibleCells", out string hVisibleCells) ? Parse.Float(hVisibleCells) : 4), componentType.data.TryGetValue("listHeight", out string hListHeight) ? Parse.Float(hListHeight) : 40);
                    break;
            }

            componentType.component.gameObject.GetComponent<LayoutElement>().preferredHeight = (componentType.component.gameObject.transform as RectTransform).sizeDelta.y;
            componentType.component.gameObject.GetComponent<LayoutElement>().preferredWidth = (componentType.component.gameObject.transform as RectTransform).sizeDelta.x;

            tableData.tableView.gameObject.SetActive(true);
            tableData.tableView.LazyInit();

            if (componentType.data.TryGetValue("id", out string id))
            {
                TableViewScroller scroller = tableData.tableView.GetField<TableViewScroller, TableView>("scroller");
                parserParams.AddEvent(id + "#PageUp", scroller.PageScrollUp);
                parserParams.AddEvent(id + "#PageDown", scroller.PageScrollDown);
            }
        }
    }
}