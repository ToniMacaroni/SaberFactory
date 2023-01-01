using FlowUi;
using FlowUi.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Flow
{
    public partial class ChoosePresetPopup : FlowCustomDialog
    {
        [Space]
        [SerializeField] private PresetList presetList;

        [SerializeField]
        private NewPresetModal _newPresetModal;

        [Space, Header("Controls")]
        [SerializeField] private Button newButton;

    }
}