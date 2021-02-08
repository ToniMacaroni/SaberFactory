using System;
using BeatSaberMarkupLanguage.Attributes;
using SaberFactory.UI.CustomSaber.CustomComponents;
using SaberFactory.UI.CustomSaber.Views;

namespace SaberFactory.UI.CustomSaber
{
    internal class NavButtonWrapper
    {
        [UIComponent("button")] public readonly NavButton NavButton = null;

        [UIValue("icon")] private readonly string _iconResource;
        [UIValue("hover-hint")] private readonly string _hoverHint;

        private readonly ENavigationCategory _category;
        private readonly Action<NavButton, ENavigationCategory> _callback;

        public NavButtonWrapper(ENavigationCategory category, string iconResource, Action<NavButton, ENavigationCategory> callback, string hoverHint = "")
        {
            _category = category;
            _iconResource = iconResource;
            _callback = callback;
            _hoverHint = hoverHint;
        }

        [UIAction("selected")]
        private void OnSelect(NavButton button, string id)
        {
            _callback?.Invoke(NavButton, _category);
        }
    }
}