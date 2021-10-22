using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.TypeHandlers;
using HMUI;
using IPA.Utilities;
using UnityEngine;

namespace SaberFactory.UI.Lib.BSML
{
    public class ButtonImageController : MonoBehaviour
    {
        public ImageView BackgroundImage;
        public ImageView ForegroundImage;
        public ImageView LineImage;

        public void SetIcon(string path)
        {
            if (ForegroundImage == null)
            {
                return;
            }

            ForegroundImage.SetImage(path);
        }

        public void SetIconColor(string colorString)
        {
            if (ForegroundImage is null)
            {
                return;
            }

            if (ColorUtility.TryParseHtmlString(colorString, out var color))
            {
                ForegroundImage.color = color;
            }
        }

        public void SetBackgroundIcon(string path)
        {
            if (BackgroundImage == null)
            {
                return;
            }

            BackgroundImage.SetImage(path);
        }

        public void ShowLine(bool show)
        {
            if (LineImage == null)
            {
                return;
            }

            LineImage.gameObject.SetActive(show);
        }

        public void SetLineColor(string colorString)
        {
            if (LineImage is null)
            {
                return;
            }

            if (!ColorUtility.TryParseHtmlString(colorString, out var color))
            {
                return;
            }

            LineImage.color = color;
        }

        public void SetIconPad(string sizeStr)
        {
            if (ForegroundImage is null)
            {
                return;
            }

            var size = int.Parse(sizeStr);
            ForegroundImage.transform.parent.GetComponent<StackLayoutGroup>().padding = new RectOffset(size, size, size, size);
        }
    }

    [ComponentHandler(typeof(ButtonImageController))]
    public class ButtonImageControllerHandler : TypeHandler<ButtonImageController>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>
        {
            { "icon", new[] { "icon" } },
            { "iconColor", new[] { "icon-color" } },
            { "iconPad", new[] { "icon-pad" } },
            { "bgIcon", new[] { "bg-icon" } },
            { "showLine", new[] { "show-line" } },
            { "lineColor", new[] { "line-color" } },
            { "useGradient", new[] { "use-gradient" } },
            { "showFill", new[] { "show-fill" } },
            { "skew", new[] { "skew" } },
            { "color0", new[] { "color0" } },
            { "color1", new[] { "color1" } }
        };

        public override Dictionary<string, Action<ButtonImageController, string>> Setters =>
            new Dictionary<string, Action<ButtonImageController, string>>
            {
                { "icon", (images, iconPath) => images.SetIcon(iconPath) },
                { "iconColor", (images, color) => images.SetIconColor(color) },
                { "iconPad", (images, size) => images.SetIconPad(size) },
                { "bgIcon", (images, iconPath) => images.SetBackgroundIcon(iconPath) },
                { "showLine", (images, stringBool) => images.ShowLine(bool.Parse(stringBool)) },
                { "lineColor", (images, color) => images.SetLineColor(color) },
                { "useGradient", SetGradient },
                { "showFill", SetFill },
                { "skew", SetSkew },
                { "color0", SetColor0 },
                { "color1", SetColor1 }
            };

        public void SetGradient(ButtonImageController imageController, string usingGradient)
        {
            imageController.BackgroundImage.SetField("_gradient", bool.Parse(usingGradient));
        }

        public void SetFill(ButtonImageController imageController, string usingFill)
        {
            imageController.BackgroundImage.fillCenter = bool.Parse(usingFill);
        }

        private void SetColor0(ButtonImageController imageController, string colorStr)
        {
            ColorUtility.TryParseHtmlString(colorStr, out var color);

            imageController.BackgroundImage.color0 = color;
        }

        private void SetColor1(ButtonImageController imageController, string colorStr)
        {
            ColorUtility.TryParseHtmlString(colorStr, out var color);

            imageController.BackgroundImage.color1 = color;
        }

        private void SetSkew(ButtonImageController imageController, string skew)
        {
            imageController.BackgroundImage.SetField("_skew", float.Parse(skew));
            imageController.LineImage.SetField("_skew", float.Parse(skew));
            imageController.ForegroundImage?.SetField("_skew", float.Parse(skew));

            imageController.BackgroundImage.SetVerticesDirty();
            imageController.LineImage.SetVerticesDirty();
            imageController.ForegroundImage?.SetVerticesDirty();
        }
    }
}