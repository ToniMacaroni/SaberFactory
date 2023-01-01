using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace FlowUi.Runtime
{
    public class ButtonStaticColors : MonoBehaviour
    {
        [SerializeField]
        private NoTransitionsButton _button;

        [SerializeField]
        private Image _buttonImage;

        [Space] public ButtonColorCollection ButtonColorCollection;

        private bool _didStart;

        protected void Awake()
        {
            _button.selectionStateDidChangeEvent += HandleButtonSelectionStateDidChange;
        }

        protected void Start()
        {
            _didStart = true;
            HandleButtonSelectionStateDidChange(_button.selectionState);
        }

        protected void OnEnable()
        {
            HandleButtonSelectionStateDidChange(_button.selectionState);
        }

        protected void OnDestroy()
        {
            if (_button != null)
            {
                _button.selectionStateDidChangeEvent -= HandleButtonSelectionStateDidChange;
            }
        }

        private void HandleButtonSelectionStateDidChange(NoTransitionsButton.SelectionState state)
        {
            if (_didStart && isActiveAndEnabled && ButtonColorCollection)
            {
                switch (state)
                {
                    case NoTransitionsButton.SelectionState.Normal:
                        _buttonImage.color = ButtonColorCollection.NormalColor;
                        break;
                    case NoTransitionsButton.SelectionState.Highlighted:
                        _buttonImage.color = ButtonColorCollection.HighlightColor;
                        break;
                    case NoTransitionsButton.SelectionState.Pressed:
                        _buttonImage.color = ButtonColorCollection.PressedColor;
                        break;
                    case NoTransitionsButton.SelectionState.Disabled:
                        _buttonImage.color = ButtonColorCollection.DisabledColor;
                        break;
                }
            }
        }
    }
}