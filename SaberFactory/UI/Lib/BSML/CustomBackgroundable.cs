using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Tags;
using BeatSaberMarkupLanguage.TypeHandlers;
using HMUI;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Components
{
    public class BackgroundTag : BSMLTag
    {
        public override string[] Aliases => new[] { "extended-bg" };

        public override GameObject CreateObject(Transform parent)
        {
            var gameObject = new GameObject();
            gameObject.name = "BSMLBackground";
            gameObject.transform.SetParent(parent, false);
            gameObject.AddComponent<ContentSizeFitter>();
            gameObject.AddComponent<CustomBackgroundable>();

            var rectTransform = gameObject.transform as RectTransform;
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.sizeDelta = new Vector2(0, 0);

            return gameObject;
        }
    }

    [ComponentHandler(typeof(CustomBackgroundable))]
    public class BackgroundableHandler : TypeHandler<CustomBackgroundable>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>
        {
            { "background", new[] { "bg", "background" } },
            { "backgroundColor", new[] { "bg-color", "background-color" } },
            { "usingGradient", new[] { "gradient" } },
            { "usingFill", new[] { "fill" } },
            { "skew", new[] { "skew" } },
            { "color0", new[] { "color0" } },
            { "color1", new[] { "color1" } }
        };

        public override Dictionary<string, Action<CustomBackgroundable, string>> Setters =>
            new Dictionary<string, Action<CustomBackgroundable, string>>
            {
                { "background", (component, value) => component.ApplyBackground(value) },
                { "backgroundColor", TrySetBackgroundColor },
                { "usingGradient", SetGradient },
                { "usingFill", SetFill },
                { "skew", SetSkew },
                { "color0", SetColor0 },
                { "color1", SetColor1 }
            };

        public static void TrySetBackgroundColor(CustomBackgroundable background, string colorStr)
        {
            if (colorStr == "none")
                return;
            ColorUtility.TryParseHtmlString(colorStr, out var color);
            background.background.color = color;
        }

        public static void SetGradient(CustomBackgroundable background, string usingGradient)
        {
            (background.background as ImageView).SetField("_gradient", bool.Parse(usingGradient));
        }

        public static void SetFill(CustomBackgroundable background, string usingFill)
        {
            background.background.fillCenter = bool.Parse(usingFill);
        }

        private void SetColor0(CustomBackgroundable background, string colorStr)
        {
            ColorUtility.TryParseHtmlString(colorStr, out var color);

            var iv = background.background as ImageView;
            iv.SetField("_color0", color);
            iv.SetVerticesDirty();
        }

        private void SetColor1(CustomBackgroundable background, string colorStr)
        {
            ColorUtility.TryParseHtmlString(colorStr, out var color);

            var iv = background.background as ImageView;
            iv.SetField("_color1", color);
            iv.SetVerticesDirty();
        }

        private void SetSkew(CustomBackgroundable background, string skew)
        {
            var iv = background.background as ImageView;
            iv.SetField("_skew", float.Parse(skew));
            iv.SetVerticesDirty();
        }
    }

    public class CustomBackgroundable : MonoBehaviour
    {
        private static Dictionary<string, string> Backgrounds => new Dictionary<string, string>
        {
            { "round-rect-panel", "RoundRect10" },
            { "panel-top", "RoundRect10" },
            { "panel-fade-gradient", "RoundRect10Thin" },
            { "panel-top-gradient", "RoundRect10" }
        };

        private static Dictionary<string, string> ObjectNames => new Dictionary<string, string>
        {
            { "round-rect-panel", "KeyboardWrapper" },
            { "panel-top", "BG" },
            { "panel-fade-gradient", "Background" },
            { "panel-top-gradient", "BG" }
        };

        private static Dictionary<string, string> ObjectParentNames => new Dictionary<string, string>
        {
            { "round-rect-panel", "Wrapper" },
            { "panel-top", "PracticeButton" },
            { "panel-fade-gradient", "LevelListTableCell" },
            { "panel-top-gradient", "ActionButton" }
        };

        public Image background;

        public void ApplyBackground(string name)
        {
            if (background != null)
                throw new Exception("Cannot add multiple backgrounds");

            if (!Backgrounds.TryGetValue(name, out var backgroundName))
                throw new Exception($"Background type '{name}' not found");

            try
            {
                background = gameObject.AddComponent(Resources.FindObjectsOfTypeAll<ImageView>().First(x =>
                    x.gameObject?.name == ObjectNames[name] && x.sprite?.name == backgroundName &&
                    x.transform.parent?.name == ObjectParentNames[name]));
                background.enabled = true;
            }
            catch
            {
            }
        }
    }
}