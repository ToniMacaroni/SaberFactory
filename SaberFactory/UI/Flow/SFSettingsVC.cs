using System.Collections.Generic;
using FlowUi.Runtime;
using HMUI;
using UnityEngine;

namespace SaberFactory.UI.Flow
{
    public partial class SFSettingsVC : FlowViewController
    {
        [Space]
        [SerializeField] private TextSegmentedControl navigationSegmentedControl;
        
        public enum ESettingsCategory
        {
            Main,
            Events,
            Misc
        }

    }
}