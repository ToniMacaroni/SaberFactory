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
    internal class LoadingPopup : Popup
    {
        [UIAction("#post-parse")]
        private void Setup()
        {
            Hide();
        }

        public new void Show()
        {
            base.Show();
        }

        public new void Hide()
        {
            base.Hide();
        }
    }
}
