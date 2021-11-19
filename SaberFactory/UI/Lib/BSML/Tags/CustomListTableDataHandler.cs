using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;
using HMUI;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Lib.BSML.Tags
{
    [ComponentHandler(typeof(CustomListTableData))]
    public class CustomListTableDataHandler : TypeHandler
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>
        {
            { "selectCell", new[] { "select-cell" } },
            { "visibleCells", new[] { "visible-cells" } },
            { "cellSize", new[] { "cell-size" } },
            { "id", new[] { "id" } },
            { "data", new[] { "data", "content" } },
            { "listWidth", new[] { "list-width" } },
            { "listHeight", new[] { "list-height" } },
            { "expandCell", new[] { "expand-cell" } },
            { "listStyle", new[] { "list-style" } },
            { "listDirection", new[] { "list-direction" } },
            { "alignCenter", new[] { "align-to-center" } }
        };

        public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            var tableData = componentType.component as CustomListTableData;
            if (componentType.data.TryGetValue("selectCell", out var selectCell))
            {
                tableData.TableView.didSelectCellWithIdxEvent += delegate(TableView table, int index)
                {
                    if (!parserParams.actions.TryGetValue(selectCell, out var action))
                    {
                        throw new Exception("select-cell action '" + componentType.data["onClick"] + "' not found");
                    }

                    action.Invoke(table, index);
                };
            }

            if (componentType.data.TryGetValue("listDirection", out var listDirection))
            {
                tableData.TableView.SetField("_tableType", (TableView.TableType)Enum.Parse(typeof(TableView.TableType), listDirection));
            }

            if (componentType.data.TryGetValue("listStyle", out var listStyle))
            {
                tableData.Style = (CustomListTableData.ListStyle)Enum.Parse(typeof(CustomListTableData.ListStyle), listStyle);
            }

            if (componentType.data.TryGetValue("cellSize", out var cellSize))
            {
                tableData.cellSize = Parse.Float(cellSize);
            }

            if (componentType.data.TryGetValue("expandCell", out var expandCell))
            {
                tableData.ExpandCell = Parse.Bool(expandCell);
            }

            if (componentType.data.TryGetValue("alignCenter", out var alignCenter))
            {
                tableData.TableView.SetField("_alignToCenter", Parse.Bool(alignCenter));
            }


            if (componentType.data.TryGetValue("data", out var value))
            {
                if (!parserParams.values.TryGetValue(value, out var contents))
                {
                    throw new Exception("value '" + value + "' not found");
                }

                tableData.Data = contents.GetValue() as List<CustomListTableData.CustomCellInfo>;
                tableData.TableView.ReloadData();
            }

            switch (tableData.TableView.tableType)
            {
                case TableView.TableType.Vertical:
                    (componentType.component.gameObject.transform as RectTransform).sizeDelta = new Vector2(
                        componentType.data.TryGetValue("listWidth", out var vListWidth) ? Parse.Float(vListWidth) : 60,
                        tableData.cellSize * (componentType.data.TryGetValue("visibleCells", out var vVisibleCells)
                            ? Parse.Float(vVisibleCells)
                            : 7));
                    break;
                case TableView.TableType.Horizontal:
                    (componentType.component.gameObject.transform as RectTransform).sizeDelta = new Vector2(
                        tableData.cellSize * (componentType.data.TryGetValue("visibleCells", out var hVisibleCells) ? Parse.Float(hVisibleCells) : 4),
                        componentType.data.TryGetValue("listHeight", out var hListHeight) ? Parse.Float(hListHeight) : 40);
                    break;
            }

            componentType.component.gameObject.GetComponent<LayoutElement>().preferredHeight =
                (componentType.component.gameObject.transform as RectTransform).sizeDelta.y;
            componentType.component.gameObject.GetComponent<LayoutElement>().preferredWidth =
                (componentType.component.gameObject.transform as RectTransform).sizeDelta.x;

            tableData.TableView.gameObject.SetActive(true);
            tableData.TableView.LazyInit();

            if (componentType.data.TryGetValue("id", out var id))
            {
                var scroller = tableData.TableView.GetField<ScrollView, TableView>("_scrollView");
                parserParams.AddEvent(id + "#PageUp", scroller.PageUpButtonPressed);
                parserParams.AddEvent(id + "#PageDown", scroller.PageDownButtonPressed);
            }
        }
    }
}