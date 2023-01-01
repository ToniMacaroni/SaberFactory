using FlowUi.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Flow
{
    public partial class ChooseTrailPopup : FlowCustomDialog
    {
        [SerializeField] private SaberList saberList;
        [SerializeField] private Button originalButton;
    }
}