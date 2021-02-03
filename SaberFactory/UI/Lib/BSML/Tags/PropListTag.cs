using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;

namespace SaberFactory.UI.Lib.BSML
{
    public class PropListTag : BSMLTag
    {
        public override string[] Aliases => new[] { "prop-list" };

        public override GameObject CreateObject(Transform parent)
        {
            var rootGO = new GameObject("ListContainer");
            var container = rootGO.AddComponent<RectTransform>();

            container.gameObject.AddComponent<LayoutElement>();
            container.SetParent(parent, false);

            GameObject gameObject = new GameObject();
            gameObject.transform.SetParent(container, false);
            gameObject.name = "PropList";
            gameObject.SetActive(false);

            gameObject.AddComponent<ScrollRect>();
            gameObject.AddComponent(Resources.FindObjectsOfTypeAll<Canvas>().First(x => x.name == "DropdownTableView"));
            gameObject.AddComponent<VRGraphicRaycaster>().SetField("_physicsRaycaster", BeatSaberUI.PhysicsRaycasterWithCache);
            gameObject.AddComponent<Touchable>();
            gameObject.AddComponent<EventSystemListener>();

            TableView tableView = gameObject.AddComponent<BSMLTableView>();
            var tableData = container.gameObject.AddComponent<PropListTableData>();
            tableData.tableView = tableView;

            tableView.SetField("_preallocatedCells", new TableView.CellsGroup[0]);
            tableView.SetField("_isInitialized", false);

            RectTransform viewport = new GameObject("Viewport").AddComponent<RectTransform>();
            viewport.SetParent(gameObject.GetComponent<RectTransform>(), false);
            viewport.gameObject.AddComponent<RectMask2D>();
            gameObject.GetComponent<ScrollRect>().viewport = viewport;

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