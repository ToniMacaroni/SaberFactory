using FlowUi.Runtime;
using HMUI;
using UnityEngine;

namespace SaberFactory.UI.Flow
{
    public partial class SettingsEventContainer : FlowCategoryContainer
    {
        [SerializeField] private Transform gridContainer;
        [SerializeField] private GameObject eventTogglePrefab;
        [SerializeField] private ToggleWithCallbacks globalSwitchToggle;

        public override int ContainerId => (int)SFSettingsVC.ESettingsCategory.Events;
    }
}