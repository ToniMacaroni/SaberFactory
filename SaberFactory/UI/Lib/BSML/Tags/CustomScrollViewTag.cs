using System.Collections;
using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using IPA.Utilities;
using SaberFactory.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;

namespace SaberFactory.UI.Lib.BSML.Tags
{
    public class CustomScrollViewTag : BSMLTag
    {
        public override string[] Aliases => new[] { CustomComponentHandler.ComponentPrefix + ".scroll-view" };

        public override GameObject CreateObject(Transform parent)
        {
            var textScrollView =
                Object.Instantiate(
                    Resources.FindObjectsOfTypeAll<ReleaseInfoViewController>().First()
                        .GetField<TextPageScrollView, ReleaseInfoViewController>("_textPageScrollView"), parent);
            textScrollView.name = "BSMLScrollView";
            var pageUpButton = textScrollView.GetField<Button, ScrollView>("_pageUpButton");
            var pageDownButton = textScrollView.GetField<Button, ScrollView>("_pageDownButton");
            var verticalScrollIndicator = textScrollView.GetField<VerticalScrollIndicator, ScrollView>("_verticalScrollIndicator");

            var viewport = textScrollView.GetField<RectTransform, ScrollView>("_viewport");
            viewport.gameObject.AddComponent<VRGraphicRaycaster>().SetField("_physicsRaycaster", BeatSaberUI.PhysicsRaycasterWithCache);

            Object.Destroy(textScrollView.GetField<TextMeshProUGUI, TextPageScrollView>("_text").gameObject);
            var gameObject = textScrollView.gameObject;
            Object.Destroy(textScrollView);
            gameObject.SetActive(false);

            var scrollView = gameObject.AddComponent<BSMLScrollView>();
            scrollView.SetField<ScrollView, Button>("_pageUpButton", pageUpButton);
            scrollView.SetField<ScrollView, Button>("_pageDownButton", pageDownButton);
            scrollView.SetField<ScrollView, VerticalScrollIndicator>("_verticalScrollIndicator", verticalScrollIndicator);
            scrollView.SetField<ScrollView, RectTransform>("_viewport", viewport);

            viewport.anchorMin = new Vector2(0, 0);
            viewport.anchorMax = new Vector2(1, 1);

            var parentObj = new GameObject();
            parentObj.name = "BSMLScrollViewContent";
            parentObj.transform.SetParent(viewport, false);

            var contentSizeFitter = parentObj.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var verticalLayout = parentObj.AddComponent<VerticalLayoutGroup>();
            verticalLayout.childForceExpandHeight = false;
            verticalLayout.childForceExpandWidth = false;
            verticalLayout.childControlHeight = true;
            verticalLayout.childControlWidth = true;
            verticalLayout.childAlignment = TextAnchor.UpperCenter;

            var rectTransform = parentObj.transform.AsRectTransform();

            parentObj.AddComponent<LayoutElement>();
            parentObj.AddComponent<ScrollViewContent>().scrollView = scrollView;

            var child = new GameObject();
            child.name = "BSMLScrollViewContentContainer";
            child.transform.SetParent(rectTransform, false);

            var layoutGroup = child.AddComponent<VerticalLayoutGroup>();
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childAlignment = TextAnchor.LowerCenter;
            layoutGroup.spacing = 0.5f;

            parentObj.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            child.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            child.AddComponent<LayoutElement>();
            var externalComponents = child.AddComponent<ExternalComponents>();
            externalComponents.components.Add(scrollView);
            externalComponents.components.Add(scrollView.transform);

            child.transform.AsRectTransform().sizeDelta = new Vector2(0, -1);

            scrollView.SetField<ScrollView, RectTransform>("_contentRectTransform", parentObj.transform as RectTransform);

            SharedCoroutineStarter.instance.StartCoroutine(Man(gameObject, rectTransform));

            return child;
        }

        private IEnumerator Man(GameObject gameObject, RectTransform rectTransform)
        {
            gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();

            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.sizeDelta = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0.5f, 1);
        }
    }
}