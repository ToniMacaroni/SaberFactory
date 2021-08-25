using System.Collections.Generic;
using SaberFactory.Configuration;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Lib.BSML
{
    internal class ThemeManager
    {
        public static readonly Dictionary<string, Color> ColorTheme = new Dictionary<string, Color>
        {
            {"light-bg", GetColor("#FFF")},
            {"dark-bg", GetColor("#0000FF")},
            {"default-panel", GetColor("#666")},
            {"saber-selector", GetColor("#666")},
            {"saber-selector-sec", GetColor("#000000CC")},
            {"navbar", GetColor("#000000CC")},
        };

        public static bool GetDefinedColor(string name, out Color color)
        {
            return ColorTheme.TryGetValue(name, out color);
        }

        public static bool GetColor(string colorString, out Color color)
        {
            if (colorString[0] == '$' && GetDefinedColor(colorString.Substring(1), out color)) return true;
            if (ColorUtility.TryParseHtmlString(colorString, out color)) return true;
            return false;
        }

        private static Color GetColor(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out var color)) return color;
            return Color.white;
        }
    }
}