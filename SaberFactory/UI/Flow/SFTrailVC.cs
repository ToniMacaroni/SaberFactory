using System.Collections.Generic;
using FlowUi.Runtime;
using HMUI;
using UnityEngine;

namespace SaberFactory.UI.Flow
{
    public partial class SFTrailVC : FlowViewController
    {
        [Space]
        [SerializeField] private TextSegmentedControl navigationSegmentedControl;

        public enum ETrailSettingsCategory
        {
            Main,
            Material
        }
    }
}