using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.TypeHandlers;
using HMUI;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;

namespace SaberFactory.UI.Lib.BSML
{
    public class CustomListTag : BSMLTag
    {
        public override string[] Aliases => new[] { "sui.list" };

        public override GameObject CreateObject(Transform parent)
        {
            var rootGO = new GameObject("CustomListContainer");
            var container = rootGO.AddComponent<RectTransform>();

            container.gameObject.AddComponent<LayoutElement>();
            container.SetParent(parent, false);

            var gameObject = new GameObject();
            gameObject.transform.SetParent(container, false);
            gameObject.name = "CustomList";
            gameObject.SetActive(false);

            gameObject.AddComponent<ScrollRect>();
            gameObject.AddComponent(Resources.FindObjectsOfTypeAll<Canvas>().First(x => x.name == "DropdownTableView"));
            gameObject.AddComponent<VRGraphicRaycaster>().SetField("_physicsRaycaster", BeatSaberUI.PhysicsRaycasterWithCache);
            gameObject.AddComponent<Touchable>();
            gameObject.AddComponent<EventSystemListener>();
            var scrollView = gameObject.AddComponent<ScrollView>();

            TableView tableView = gameObject.AddComponent<BSMLTableView>();
            var tableData = container.gameObject.AddComponent<CustomListTableData>();
            tableData.TableView = tableView;

            tableView.SetField("_preallocatedCells", new TableView.CellsGroup[0]);
            tableView.SetField("_isInitialized", false);
            tableView.SetField("_scrollView", scrollView);

            var viewport = new GameObject("Viewport").AddComponent<RectTransform>();
            viewport.SetParent(gameObject.GetComponent<RectTransform>(), false);
            viewport.gameObject.AddComponent<RectMask2D>();
            gameObject.GetComponent<ScrollRect>().viewport = viewport;

            var content = new GameObject("Content").AddComponent<RectTransform>();
            content.SetParent(viewport, false);

            scrollView.SetField("_contentRectTransform", content);
            scrollView.SetField("_viewport", viewport);

            viewport.anchorMin = new Vector2(0f, 0f);
            viewport.anchorMax = new Vector2(1f, 1f);
            viewport.sizeDelta = new Vector2(0f, 0f);
            viewport.anchoredPosition = new Vector3(0f, 0f);

            var tableviewRect = (RectTransform)tableView.transform;
            tableviewRect.anchorMin = new Vector2(0f, 0f);
            tableviewRect.anchorMax = new Vector2(1f, 1f);
            tableviewRect.sizeDelta = new Vector2(0f, 0f);
            tableviewRect.anchoredPosition = new Vector3(0f, 0f);

            tableView.SetDataSource(tableData, false);
            return container.gameObject;
        }
    }

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
                tableData.TableView.didSelectCellWithIdxEvent += delegate(TableView table, int index)
                {
                    if (!parserParams.actions.TryGetValue(selectCell, out var action))
                        throw new Exception("select-cell action '" + componentType.data["onClick"] + "' not found");

                    action.Invoke(table, index);
                };

            if (componentType.data.TryGetValue("listDirection", out var listDirection))
                tableData.TableView.SetField("_tableType", (TableView.TableType)Enum.Parse(typeof(TableView.TableType), listDirection));

            if (componentType.data.TryGetValue("listStyle", out var listStyle))
                tableData.Style = (CustomListTableData.ListStyle)Enum.Parse(typeof(CustomListTableData.ListStyle), listStyle);

            if (componentType.data.TryGetValue("cellSize", out var cellSize))
                tableData.cellSize = Parse.Float(cellSize);

            if (componentType.data.TryGetValue("expandCell", out var expandCell))
                tableData.ExpandCell = Parse.Bool(expandCell);

            if (componentType.data.TryGetValue("alignCenter", out var alignCenter))
                tableData.TableView.SetField("_alignToCenter", Parse.Bool(alignCenter));


            if (componentType.data.TryGetValue("data", out var value))
            {
                if (!parserParams.values.TryGetValue(value, out var contents))
                    throw new Exception("value '" + value + "' not found");
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