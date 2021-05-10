using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Lib.BSML.Tags
{
    public class BackgroundTag : BSMLTag
    {
        public override string[] Aliases => new[] { "sui.bg" };

        public override GameObject CreateObject(Transform parent)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = "BSMLBackground";
            gameObject.transform.SetParent(parent, false);
            gameObject.AddComponent<ContentSizeFitter>();
            gameObject.AddComponent<Backgroundable>();

            RectTransform rectTransform = gameObject.transform as RectTransform;
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.sizeDelta = new Vector2(0, 0);

            return gameObject;
        }
    }
}
