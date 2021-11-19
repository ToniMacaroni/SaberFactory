using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.TypeHandlers;
using HMUI;
using IPA.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Lib.BSML
{
    [ComponentHandler(typeof(Backgroundable))]
    public class CustomBackgroundableHandler : TypeHandler<Backgroundable>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>
        {
            { "usingGradient", new[] { "gradient" } },
            { "usingFill", new[] { "fill" } },
            { "skew", new[] { "skew" } },
            { "color0", new[] { "color0" } },
            { "color1", new[] { "color1" } }
        };

        public override Dictionary<string, Action<Backgroundable, string>> Setters =>
            new Dictionary<string, Action<Backgroundable, string>>
            {
                { "usingGradient", SetGradient },
                { "usingFill", SetFill },
                { "skew", SetSkew },
                { "color0", SetColor0 },
                { "color1", SetColor1 }
            };

        public static void SetGradient(Backgroundable background, string usingGradient)
        {
            (background.background as ImageView).SetField("_gradient", bool.Parse(usingGradient));
        }

        public static void SetFill(Backgroundable background, string usingFill)
        {
            background.background.fillCenter = bool.Parse(usingFill);
        }

        private void SetColor0(Backgroundable background, string colorStr)
        {
            ColorUtility.TryParseHtmlString(colorStr, out var color);

            var iv = background.background as ImageView;
            iv.SetField("_color0", color);
            iv.SetVerticesDirty();
        }

        private void SetColor1(Backgroundable background, string colorStr)
        {
            ColorUtility.TryParseHtmlString(colorStr, out var color);

            var iv = background.background as ImageView;
            iv.SetField("_color1", color);
            iv.SetVerticesDirty();
        }

        private void SetSkew(Backgroundable background, string skew)
        {
            var iv = background.background as ImageView;
            iv.SetField("_skew", float.Parse(skew));
            iv.SetVerticesDirty();
        }
    }
}