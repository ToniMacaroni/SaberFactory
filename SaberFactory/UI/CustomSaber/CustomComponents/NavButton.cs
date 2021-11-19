using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;
using SaberFactory.UI.Lib;
using SaberFactory.UI.Lib.BSML;
using UnityEngine;

namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class NavButton : CustomUiComponent
    {
        [UIComponent("icon-button")] private readonly ButtonImageController _iconButton = null;
        [UIValue("hover-hint")] private string _hoverHint;

        public Color OnColor
        {
            get => _onColor;
            set
            {
                _onColor = value;
                UpdateColor();
            }
        }

        public Color OffColor
        {
            get => _offColor;
            set
            {
                _offColor = value;
                UpdateColor();
            }
        }

        public bool IsOn { get; private set; }

        public string CategoryId { get; private set; }
        public Action<NavButton, string> OnSelect;

        private readonly Color _iconShadedColor = new Color(1, 1, 1, 0.6f);

        private ButtonStateColors _buttonStateColors;
        private Color _hoverColor;

        private Color _offColor;

        private Color _onColor;

        public void SetState(bool state, bool fireEvent)
        {
            IsOn = state;
            UpdateColor();
            if (fireEvent)
            {
                OnSelect?.Invoke(this, CategoryId);
            }
        }

        public void Deselect()
        {
            SetState(false, false);
        }

        [UIAction("clicked")]
        private void Clicked()
        {
            SetState(true, true);
        }

        private void UpdateColor()
        {
            if (_buttonStateColors is null)
            {
                return;
            }

            _buttonStateColors.NormalColor = IsOn ? OnColor : Color.clear;
            _buttonStateColors.HoveredColor = IsOn ? OnColor : _hoverColor;
            _iconButton.ForegroundImage.color = IsOn ? Color.white : _iconShadedColor;
            _buttonStateColors.Image.gradient = !IsOn;
            _buttonStateColors.UpdateSelectionState();
        }

        public void SetIcon(string path)
        {
            _iconButton.ForegroundImage.SetImage(path);
        }

        public void SetHoverHint(string hoverHint)
        {
            _hoverHint = hoverHint;
        }

        public void SetCategoryId(string id)
        {
            CategoryId = id;
        }

        [UIAction("#post-parse")]
        private void Setup()
        {
            _buttonStateColors = GetComponentsInChildren<ButtonStateColors>().First();
            _hoverColor = _buttonStateColors.HoveredColor;
        }

        [ComponentHandler(typeof(NavButton))]
        internal class TypeHandler : TypeHandler<NavButton>
        {
            public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>
            {
                { "icon", new[] { "icon" } },
                { "onColor", new[] { "on-color" } },
                { "offColor", new[] { "off-color" } },
                { "onSelected", new[] { "on-selected" } },
                { "hoverhint", new[] { "hover-hint" } },
                { "catId", new[] { "category" } }
            };

            public override Dictionary<string, Action<NavButton, string>> Setters =>
                new Dictionary<string, Action<NavButton, string>>
                {
                    { "icon", (button, val) => button.SetIcon(val) },
                    { "onColor", SetOnColor },
                    { "offColor", SetOffColor },
                    { "hoverhint", (button, val) => button.SetHoverHint(val) },
                    { "category", (button, val) => button.SetCategoryId(val) }
                };

            public override void HandleType(BSMLParser.ComponentTypeWithData componentType,
                BSMLParserParams parserParams)
            {
                try
                {
                    var button = componentType.component as NavButton;

                    if (componentType.data.TryGetValue("onSelected", out var onToggle))
                    {
                        if (parserParams.actions.TryGetValue(onToggle, out var onToggleAction))
                        {
                            button.OnSelect += (btn, val) => { onToggleAction.Invoke(btn, val); };
                        }
                    }

                    base.HandleType(componentType, parserParams);
                }
                catch (Exception)
                { }
            }

            private void SetOnColor(NavButton button, string hexString)
            {
                if (hexString == "none")
                {
                    return;
                }

                ColorUtility.TryParseHtmlString(hexString, out var color);
                button.OnColor = color;
            }

            private void SetOffColor(NavButton button, string hexString)
            {
                if (hexString == "none")
                {
                    return;
                }

                ColorUtility.TryParseHtmlString(hexString, out var color);
                button.OffColor = color;
            }
        }
    }
}