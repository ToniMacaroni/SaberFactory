using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using SaberFactory.UI.Lib;
using UnityEngine;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class LoadingPopup : CustomUiComponent
    {
        [UIObject("root")] private readonly GameObject _root = null;

        public void Show()
        {
            _root.SetActive(true);
        }

        public void Hide()
        {
            _root.SetActive(false);
        }

        [UIAction("#post-parse")]
        private void Setup()
        {
            _root.SetActive(false);
        }
    }
}
