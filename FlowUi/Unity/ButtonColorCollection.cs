using UnityEngine;

namespace FlowUi.Runtime
{
    [CreateAssetMenu(menuName = "Flow/Create ButtonColorCollection", fileName = "ButtonColorCollection", order = 0)]
    public class ButtonColorCollection : ScriptableObject
    {
        public Color NormalColor;
        public Color HighlightColor;
        public Color PressedColor;
        public Color DisabledColor;
    }
}