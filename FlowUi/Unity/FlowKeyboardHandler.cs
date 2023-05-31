using System;
using System.Threading.Tasks;
using FlowUi.Runtime;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace FlowUi
{
    public class FlowKeyboardHandler : InputFieldView
    {
        public string Text
        {
            get => _text;
            private set
            {
                _text = value;
                _textView.SetText(_text);
                _textView.ForceMeshUpdate(true);
            }
        }

        [SerializeField]
        private GameObject _popup;

        [SerializeField]
        private Button _cancelButton;

        [SerializeField]
        private CanvasGroup _groupToHide;

        [SerializeField]
        private float _hiddenGroupAlpha = 0.3f;

        [Inject]
        private readonly UIKeyboardManager _uiKeyboardManager = null;

#if MOD
        public override void Awake()
#else
        protected override void Awake()
#endif
        {
        }

        // public override void DoStateTransition(Selectable.SelectionState state, bool instant)
        // {
        // }

        public new void ActivateKeyboard(HMUI.UIKeyboard keyboard)
        {
            if (!_hasKeyboardAssigned)
            {
                _hasKeyboardAssigned = true;
                keyboard.keyWasPressedEvent += KeyboardKeyPressed;
                keyboard.deleteButtonWasPressedEvent += KeyboardDeletePressed;
            }
        }

        public new void DeactivateKeyboard(HMUI.UIKeyboard keyboard)
        {
            if (_hasKeyboardAssigned)
            {
                _hasKeyboardAssigned = false;
                keyboard.keyWasPressedEvent -= KeyboardKeyPressed;
                keyboard.deleteButtonWasPressedEvent -= KeyboardDeletePressed;
                Closed(false);
            }
        }

        public async Task<bool> Open()
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();
            
            _uiKeyboardManager.OpenKeyboardFor(this);
            _popup.SetActive(true);

            if (_groupToHide)
            {
                _groupToHide.alpha = _hiddenGroupAlpha;
                _groupToHide.interactable = false;
            }

            _buttonBinder.AddBinding(_cancelButton, CancelButtonPressed);

            return await _taskCompletionSource.Task;
        }

        public new void KeyboardDeletePressed()
        {
            Text = Text.Length <= 0 ? "" : Text.Substring(0, Text.Length - 1);
            _onValueChanged.Invoke(this);
        }

        public new void KeyboardKeyPressed(char letter)
        {
            if (Text.Length < _textLengthLimit)
            {
                var obj = Text;
                var c = _useUppercase ? char.ToUpper(letter) : letter;
                Text = obj + c;
                _onValueChanged.Invoke(this);
            }
        }
        
        private void KeyboardOkButtonWasPressed()
        {
            Debug.Log("Ok button pressed");
        }

        private void Closed(bool cancelled)
        {
            Debug.Log("Closed");
            _popup.SetActive(false);

            if (_groupToHide)
            {
                _groupToHide.alpha = 1f;
                _groupToHide.interactable = true;
            }
            
            _buttonBinder.ClearBindings();
            
            _taskCompletionSource.SetResult(!cancelled);
        }

        private void CancelButtonPressed()
        {
            //Closed(true);
        }

        private string _text = "";
        private TaskCompletionSource<bool> _taskCompletionSource;
        private readonly FlowButtonBinder _buttonBinder = new FlowButtonBinder();

    }
}