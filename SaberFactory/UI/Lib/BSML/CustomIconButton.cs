using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.TypeHandlers;
using HMUI;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SaberFactory.UI.Lib.BSML
{
    [ComponentHandler(typeof(CustomButtonImages))]
    public class CustomButtonIconHandler : TypeHandler<CustomButtonImages>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>()
        {
            { "icon", new[]{"icon"} },
            { "bg", new []{"bg"} },
            { "useLine", new []{"use-line"} },
            { "usingGradient", new[]{ "gradient" } },
            { "usingFill", new[]{ "fill" } },
            { "skew", new[]{ "skew" } },
            { "color0", new[]{ "color0" } },
            { "color1", new[]{ "color1" } }
        };

        public override Dictionary<string, Action<CustomButtonImages, string>> Setters => new Dictionary<string, Action<CustomButtonImages, string>>()
        {
            { "icon", (images, iconPath) => images.SetIcon(iconPath)},
            { "bg", (images, iconPath) => images.SetBG(iconPath)},
            { "useLine", (images, stringBool) => images.ShowLine(bool.Parse(stringBool))},
            {"usingGradient", SetGradient },
            {"usingFill", SetFill },
            {"skew", SetSkew },
            {"color0", SetColor0 },
            {"color1", SetColor1 }
        };

        public void SetGradient(CustomButtonImages images, string usingGradient)
        {
            images.bg.SetField("_gradient", bool.Parse(usingGradient));
        }

        public void SetFill(CustomButtonImages images, string usingFill)
        {
            images.bg.fillCenter = bool.Parse(usingFill);
        }

        private void SetColor0(CustomButtonImages images, string colorStr)
        {
            ColorUtility.TryParseHtmlString(colorStr, out Color color);

            images.bg.SetField("_color0", color);
            images.bg.SetVerticesDirty();
        }

        private void SetColor1(CustomButtonImages images, string colorStr)
        {
            ColorUtility.TryParseHtmlString(colorStr, out Color color);

            images.bg.SetField("_color1", color);
            images.bg.SetVerticesDirty();           
        }

        private void SetSkew(CustomButtonImages images, string skew)
        {
            images.bg.SetField("_skew", float.Parse(skew));
            images.bg.SetVerticesDirty();
        }

    }

    public class ButtonWithIconTag : BSMLTag
    {
        public override string[] Aliases => new[] { "custom-icon-btn" };

        public override GameObject CreateObject(Transform parent)
        {
            Button button = Object.Instantiate(Resources.FindObjectsOfTypeAll<Button>().Last(x => x.name == "PracticeButton"), parent, false);
            button.name = "CustomIconButton";
            button.interactable = true;

            Object.Destroy(button.GetComponent<HoverHint>());
            Object.Destroy(button.GetComponent<LocalizedHoverHint>());
            button.gameObject.AddComponent<ExternalComponents>().components.Add(button.GetComponentsInChildren<LayoutGroup>().First(x => x.name == "Content"));

            Transform contentTransform = button.transform.Find("Content");

            contentTransform.GetComponent<LayoutElement>().minWidth = 0;

            Object.Destroy(contentTransform.Find("Text").gameObject);

            Image iconImage = new GameObject("Icon").AddComponent<ImageView>();
            iconImage.material = Utilities.ImageResources.NoGlowMat;
            iconImage.rectTransform.SetParent(contentTransform, false);
            iconImage.rectTransform.sizeDelta = new Vector2(20f, 20f);
            iconImage.sprite = Utilities.ImageResources.BlankSprite;
            iconImage.preserveAspect = true;

            CustomButtonImages btnImages = button.gameObject.AddComponent<CustomButtonImages>();
            btnImages.image = iconImage;
            btnImages.bg = button.transform.Find("BG").GetComponent<ImageView>();
            btnImages.line = button.transform.Find("Underline").GetComponent<ImageView>();

            return button.gameObject;
        }
    }

    public class CustomButtonImages : MonoBehaviour
    {
        public Image image;
        public ImageView bg;
        public Image line;

        public void SetIcon(string path)
        {
            if (image == null)
                return;
            image.SetImage(path);
        }

        public void SetBG(string path)
        {
            if (bg == null)
                return;
            bg.SetImage(path);
        }

        public void ShowLine(bool show)
        {
            if (line == null) return;

            line.gameObject.SetActive(show);
        }
    }
}