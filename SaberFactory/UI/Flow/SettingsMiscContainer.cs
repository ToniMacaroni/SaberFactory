using FlowUi.Runtime;
using UnityEngine;

namespace SaberFactory.UI.Flow
{
    public partial class SettingsMiscContainer : FlowCategoryContainer
    {
        [SerializeField] private PropList propList;

        public override int ContainerId => (int)SFSettingsVC.ESettingsCategory.Misc;
    }
}