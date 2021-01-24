using System;
using BeatSaberMarkupLanguage.Attributes;
using SaberFactory.UI.CustomSaber.Views;

namespace SaberFactory.UI.CustomSaber
{
    internal class NavigationButton
    {
        [UIValue("icon")] private readonly string _iconResource;
        [UIValue("hover-hint")] private readonly string _hoverHint;

        private readonly ENavigationCategory _category;
        private readonly Action<ENavigationCategory> _callback;

        public NavigationButton(ENavigationCategory category, string iconResource, Action<ENavigationCategory> callback, string hoverHint = "")
        {
            _category = category;
            _iconResource = iconResource;
            _callback = callback;
            _hoverHint = hoverHint;
        }

        [UIAction("clicked")]
        private void OnClick()
        {
            _callback?.Invoke(_category);
        }
    }
}