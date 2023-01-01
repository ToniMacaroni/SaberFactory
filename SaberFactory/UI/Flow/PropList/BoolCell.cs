using FlowUi.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Flow
{
    public partial class BoolCell : PropCell
    {
        [SerializeField] private Toggle _toggle;
        [SerializeField] private TextMeshProUGUI _propNameTextMesh;
        [SerializeField] private TextMeshProUGUI _resetTextMesh;
    }
}