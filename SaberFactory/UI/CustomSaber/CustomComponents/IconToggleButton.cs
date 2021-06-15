using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;
using SaberFactory.UI.Lib;
using SaberFactory.UI.Lib.BSML;
using UnityEngine;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class IconToggleButton : CustomUiComponent
    {
        public event Action<bool> OnStateChanged;

        [UIComponent("icon-button")] private readonly ButtonImageController _iconButtonImageController = null;
        [UIValue("hover-hint")] private string _hoverHint = "";

        public Color OnColor { get; set; }

        public Color OffColor { get; set; }

        public bool IsOn { get; private set; }

        public void SetIcon(string path)
        {
            _iconButtonImageController.SetIcon(path);
        }

        public void SetState(bool state, bool fireEvent)
        {
            IsOn = state;
            UpdateColor();
            if (fireEvent)
            {
                OnStateChanged?.Invoke(state);
            }
        }

        public void SetHoverHint(string text)
        {
            _hoverHint = text;
        }

        private void UpdateColor()
        {
            var image = _iconButtonImageController.ForegroundImage;
            if (!image) return;
            image.color = IsOn ? OnColor : OffColor;
        }

        [UIAction("clicked")]
        private void ClickedButton()
        {
            SetState(!IsOn, true);
        }

        [ComponentHandler(typeof(IconToggleButton))]
        internal class TypeHandler : TypeHandler<IconToggleButton>
        {
            public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>
            {
                {"icon", new[] {"icon"}},
                {"onColor", new[] {"on-color"}},
                {"offColor", new[] {"off-color"}},
                {"onToggle", new []{"on-toggle"} },
                {"hoverhint", new []{"hover-hint"} }
            };

            public override Dictionary<string, Action<IconToggleButton, string>> Setters =>
                new Dictionary<string, Action<IconToggleButton, string>>
                {
                    {"icon", (button, val) => button.SetIcon(val)},
                    {"onColor", SetOnColor},
                    {"offColor", SetOffColor },
                    {"hoverhint", (button, val) => button.SetHoverHint(val) }
                };

            public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
            {
                try
                {
                    var button = componentType.component as IconToggleButton;

                    if (componentType.data.TryGetValue("onToggle", out string onToggle))
                    {
                        if (parserParams.actions.TryGetValue(onToggle, out BSMLAction onToggleAction))
                        {
                            button.OnStateChanged += (val) =>
                            {
                                onToggleAction.Invoke(val);
                            };
                        }
                    }
                    base.HandleType(componentType, parserParams);
                }
                catch (Exception)
                {
                }
            }

            private void SetOnColor(IconToggleButton button, string hexString)
            {
                if (hexString == "none")
                    return;
                ColorUtility.TryParseHtmlString(hexString, out var color);
                button.OnColor = color;
            }

            private void SetOffColor(IconToggleButton button, string hexString)
            {
                if (hexString == "none")
                    return;
                ColorUtility.TryParseHtmlString(hexString, out var color);
                button.OffColor = color;
            }
        }
    }
}
