using FlowUi.Runtime;
using UnityEngine;

namespace SaberFactory.UI.Flow
{
    public partial class TrailMaterialContainer : FlowCategoryContainer
    {
        [SerializeField] private PropList _propList;

        public override int ContainerId => (int)SFTrailVC.ETrailSettingsCategory.Material;
    }
}