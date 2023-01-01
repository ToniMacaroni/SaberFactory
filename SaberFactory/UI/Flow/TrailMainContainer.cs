using System.Threading.Tasks;
using FlowUi.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Flow
{
    public partial class TrailMainContainer : FlowCategoryContainer
    {
        [SerializeField] private FlowSlider lengthSlider;
        [SerializeField] private FlowSlider widthSlider;

        [SerializeField] private FlowSlider whitestepSlider;
        [SerializeField] private FlowSlider offsetSlider;

        [SerializeField] private Toggle clampToggle;
        [SerializeField] private Toggle flipToggle;

        [SerializeField] private Button revertButton;
        [SerializeField] private Button chooseTrailButton;

        [SerializeField] private ChooseTrailPopup chooseTrailPopup;

        public override int ContainerId => (int)SFTrailVC.ETrailSettingsCategory.Main;
    }
}