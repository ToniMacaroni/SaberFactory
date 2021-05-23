using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using SaberFactory.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SaberFactory.UI.Lib.BSML.Tags
{
    public class ButtonWithIconTag : BSMLTag
    {
        public override string[] Aliases => new[] { "sui.icon-button" };

        public override GameObject CreateObject(Transform parent)
        {
            Button button = Object.Instantiate(Resources.FindObjectsOfTypeAll<Button>().Last(x => x.name == "PracticeButton"), parent, false);
            button.name = "CustomIconButton";
            button.interactable = true;

            Object.Destroy(button.GetComponent<HoverHint>());
            Object.Destroy(button.GetComponent<LocalizedHoverHint>());
            button.GetComponent<ButtonStaticAnimations>().TryDestroy();
            button.gameObject.AddComponent<ExternalComponents>().components.Add(button.GetComponentsInChildren<LayoutGroup>().First(x => x.name == "Content"));

            Transform contentTransform = button.transform.Find("Content");

            contentTransform.GetComponent<LayoutElement>().minWidth = 0;

            Object.Destroy(contentTransform.Find("Text").gameObject);

            var iconImage = new GameObject("Icon").AddComponent<ImageView>();
            iconImage.material = Utilities.ImageResources.NoGlowMat;
            iconImage.rectTransform.SetParent(contentTransform, false);
            iconImage.rectTransform.sizeDelta = new Vector2(20f, 20f);
            iconImage.sprite = Utilities.ImageResources.BlankSprite;
            iconImage.preserveAspect = true;

            ButtonImageController btnImageController = button.gameObject.AddComponent<ButtonImageController>();
            btnImageController.ForegroundImage = iconImage;
            btnImageController.BackgroundImage = button.transform.Find("BG").GetComponent<ImageView>();
            btnImageController.LineImage = button.transform.Find("Underline").GetComponent<ImageView>();

            btnImageController.BackgroundImage.color0 = new Color(1, 1, 1, 1);
            btnImageController.BackgroundImage.color1 = new Color(1, 1, 1, 0f);

            var noTransitionsButton = button.gameObject.GetComponent<NoTransitionsButton>();
            var buttonStateColors = button.gameObject.AddComponent<ButtonStateColors>();
            buttonStateColors.Image = btnImageController.BackgroundImage;
            noTransitionsButton.selectionStateDidChangeEvent += buttonStateColors.SelectionDidChange;

            if (!button.gameObject.activeSelf)
            {
                button.gameObject.SetActive(true);
            }

            return button.gameObject;
        }
    }
}