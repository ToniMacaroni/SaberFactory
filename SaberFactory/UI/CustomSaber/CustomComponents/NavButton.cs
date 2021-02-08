using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;
using SaberFactory.UI.Lib;
using SaberFactory.UI.Lib.BSML;
using UnityEngine;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class NavButton : CustomUiComponent
    {
        public Action<NavButton, string> OnSelect;

        [UIComponent("icon-button")] private readonly ButtonImageController _iconButton = null;

        [UIValue("hover-hint")] private string _hoverHint = null;

        private Color _onColor;
        public Color OnColor
        {
            get => _onColor;
            set
            {
                _onColor = value;
                UpdateColor();
            }
        }

        private Color _offColor;
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
            var image = _iconButton.ForegroundImage;
            if (!image) return;
            image.color = IsOn ? OnColor : OffColor;
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

        [ComponentHandler(typeof(NavButton))]
        internal class TypeHandler : TypeHandler<NavButton>
        {
            public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>
            {
                {"icon", new[] {"icon"}},
                {"onColor", new[] {"on-color"}},
                {"offColor", new[] {"off-color"}},
                {"onSelected", new[] {"on-selected"}},
                {"hoverhint", new[] {"hover-hint"}},
                {"catId", new []{"category"} }
            };

            public override Dictionary<string, Action<NavButton, string>> Setters =>
                new Dictionary<string, Action<NavButton, string>>
                {
                    {"icon", (button, val) => button.SetIcon(val)},
                    {"onColor", SetOnColor},
                    {"offColor", SetOffColor},
                    {"hoverhint", (button, val) => button.SetHoverHint(val)},
                    {"category", (button, val) => button.SetCategoryId(val)}
        };

            public override void HandleType(BSMLParser.ComponentTypeWithData componentType,
                BSMLParserParams parserParams)
            {
                try
                {
                    var button = componentType.component as NavButton;

                    if (componentType.data.TryGetValue("onSelected", out string onToggle))
                    {
                        if (parserParams.actions.TryGetValue(onToggle, out BSMLAction onToggleAction))
                        {
                            button.OnSelect += (btn, val) => { onToggleAction.Invoke(btn, val); };
                        }
                    }

                    base.HandleType(componentType, parserParams);
                }
                catch (Exception)
                {
                }
            }

            private void SetOnColor(NavButton button, string hexString)
            {
                if (hexString == "none")
                    return;
                ColorUtility.TryParseHtmlString(hexString, out var color);
                button.OnColor = color;
            }

            private void SetOffColor(NavButton button, string hexString)
            {
                if (hexString == "none")
                    return;
                ColorUtility.TryParseHtmlString(hexString, out var color);
                button.OffColor = color;
            }
        }
    }
}
