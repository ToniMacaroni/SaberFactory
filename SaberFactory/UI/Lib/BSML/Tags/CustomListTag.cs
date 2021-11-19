using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;

namespace SaberFactory.UI.Lib.BSML.Tags
{
    public class CustomListTag : BSMLTag
    {
        public override string[] Aliases => new[] { CustomComponentHandler.ComponentPrefix + ".list" };

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
}