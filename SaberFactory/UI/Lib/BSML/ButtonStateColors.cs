using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.TypeHandlers;
using HMUI;
using UnityEngine;

namespace SaberFactory.UI.Lib.BSML
{
    public class ButtonStateColors : MonoBehaviour
    {
        public Color HoveredColor = new Color(0, 0, 0, 0.8f);
        public ImageView Image;

        public Color NormalColor = new Color(0, 0, 0, 0.5f);
        public Color SelectedColor;
        public ImageView UnderlineImage;

        private NoTransitionsButton.SelectionState _currentSelectionState = NoTransitionsButton.SelectionState.Normal;

        public void SetNormalColor(string colorStr)
        {
            ColorUtility.TryParseHtmlString(colorStr, out NormalColor);
            SelectionDidChange(_currentSelectionState);
        }

        public void SetHoveredColor(string colorStr)
        {
            ColorUtility.TryParseHtmlString(colorStr, out HoveredColor);
            SelectionDidChange(_currentSelectionState);
        }

        public void SetSelectedColor(string colorStr)
        {
            ColorUtility.TryParseHtmlString(colorStr, out SelectedColor);
            SelectionDidChange(_currentSelectionState);
        }

        public void SelectionDidChange(NoTransitionsButton.SelectionState selectionState)
        {
            _currentSelectionState = selectionState;
            switch (selectionState)
            {
                case NoTransitionsButton.SelectionState.Normal:
                    Image.color = NormalColor;
                    if (UnderlineImage is { }) UnderlineImage.enabled = false;
                    break;
                case NoTransitionsButton.SelectionState.Highlighted:
                    Image.color = HoveredColor;
                    if (UnderlineImage is { }) UnderlineImage.enabled = true;
                    break;
                case NoTransitionsButton.SelectionState.Pressed:
                    Image.color = SelectedColor;
                    break;
                default:
                    Image.color = NormalColor;
                    break;
            }
        }

        public void UpdateSelectionState()
        {
            SelectionDidChange(_currentSelectionState);
        }
    }

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