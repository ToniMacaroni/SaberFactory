using System.Threading.Tasks;
using FlowUi.Runtime;
using HMUI;
using UnityEngine;

namespace SaberFactory.UI.Flow
{
    public partial class SettingsMainContainer : FlowCategoryContainer
    {
        [SerializeField] private ToggleWithCallbacks globalSwitchToggle;

        public override int ContainerId => (int)SFSettingsVC.ESettingsCategory.Main;
    }
}