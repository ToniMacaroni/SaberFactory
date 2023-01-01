using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace FlowUi.Runtime
{
    public class SwitchStaticColors : MonoBehaviour
    {
        [SerializeField]
        private Image _backgroundImage;

        [Space] public ButtonColorCollection ButtonColorCollection;

        private ToggleWithCallbacks _toggle;

        protected void Awake()
        {
            _toggle = GetComponent<ToggleWithCallbacks>();
        }

        protected void Start()
        {
            _toggle.onValueChanged.AddListener(HandleOnValueChanged);
            _toggle.stateDidChangeEvent += HandleStateDidChange;
            RefreshVisuals();
        }

        protected void OnDestroy()
        {
            _toggle.onValueChanged.RemoveListener(HandleOnValueChanged);
            _toggle.stateDidChangeEvent -= HandleStateDidChange;
        }

        public void HandleOnValueChanged(bool value)
        {
            RefreshVisuals();
        }

        private void HandleStateDidChange(ToggleWithCallbacks.SelectionState value)
        {
            RefreshVisuals();
        }

        private void RefreshVisuals()
        {
            if (isActiveAndEnabled && ButtonColorCollection)
            {
                switch (_toggle.selectionState)
                {
                    case ToggleWithCallbacks.SelectionState.Normal:
                        if (_toggle.isOn)
                        {
                            _backgroundImage.color = ButtonColorCollection.HighlightColor;
                        }
                        else
                        {
                            _backgroundImage.color = ButtonColorCollection.NormalColor;
                        }
                        break;
                    case ToggleWithCallbacks.SelectionState.Highlighted:
                        _backgroundImage.color = ButtonColorCollection.HighlightColor;
                        break;
                    case ToggleWithCallbacks.SelectionState.Pressed:
                        _backgroundImage.color = ButtonColorCollection.PressedColor;
                        break;
                    case ToggleWithCallbacks.SelectionState.Disabled:
                        _backgroundImage.color = ButtonColorCollection.DisabledColor;
                        break;
                }
            }
        }
    }
}