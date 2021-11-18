using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.TypeHandlers;

namespace SaberFactory.UI.Lib.BSML
{
    [ComponentHandler(typeof(ButtonStateColors))]
    public class ButtonStateColorsHandler : TypeHandler<ButtonStateColors>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>
        {
            { "normalColor", new[] { "normal-color" } },
            { "hoveredColor", new[] { "hovered-color" } },
            { "selectedColor", new[] { "selected-color" } }
        };

        public override Dictionary<string, Action<ButtonStateColors, string>> Setters =>
            new Dictionary<string, Action<ButtonStateColors, string>>
            {
                { "normalColor", (colors, val) => colors.SetNormalColor(val) },
                { "hoveredColor", (colors, val) => colors.SetHoveredColor(val) },
                { "selectedColor", (colors, val) => colors.SetSelectedColor(val) }
            };
    }
}