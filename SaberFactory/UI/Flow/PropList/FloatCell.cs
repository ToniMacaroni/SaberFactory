using FlowUi.Runtime;
using TMPro;
using UnityEngine;

namespace SaberFactory.UI.Flow
{
    public partial class FloatCell : PropCell
    {
        [SerializeField] private FlowSlider _slider;
        [SerializeField] private TextMeshProUGUI _textMesh;
        [SerializeField] private TextMeshProUGUI _resetTextMesh;
    }
}