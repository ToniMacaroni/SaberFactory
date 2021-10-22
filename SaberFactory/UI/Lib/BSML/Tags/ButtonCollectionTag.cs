using System.Linq;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using IPA.Utilities;
using SaberFactory.UI.Lib.BSML.Components;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Lib.BSML.Tags
{
    internal class ButtonCollectionTag : BSMLTag
    {
        public override string[] Aliases => new[] { CustomComponentHandler.ComponentPrefix+".button-collection" };

        private TextSegmentedControl segmentControlTemplate;

        public override GameObject CreateObject(Transform parent)
        {
            if (segmentControlTemplate == null)
            {
                segmentControlTemplate = Resources.FindObjectsOfTypeAll<TextSegmentedControl>().FirstOrDefault(x =>
                    x.transform.parent.name == "PlayerStatisticsViewController" &&
                    x.GetField<DiContainer, TextSegmentedControl>("_container") != null);
            }

            var textSegmentedControl = Object.Instantiate(segmentControlTemplate, parent, false);
            textSegmentedControl.name = "SUIButtonCollection";
            textSegmentedControl.SetField("_container", segmentControlTemplate.GetField<DiContainer, TextSegmentedControl>("_container"));
            (textSegmentedControl.transform as RectTransform).anchoredPosition = new Vector2(0, 0);
            foreach (Transform transform in textSegmentedControl.transform)
            {
                Object.Destroy(transform.gameObject);
            }

            textSegmentedControl.gameObject.AddComponent<ButtonCollection>().textSegmentedControl = textSegmentedControl;
            return textSegmentedControl.gameObject;
        }
    }
}