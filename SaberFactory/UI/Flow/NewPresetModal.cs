using System;
using System.Threading.Tasks;
using FlowUi.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Flow
{
    public class NewPresetModal : MonoBehaviour
    {
        [SerializeField]
        private FlowInputField _inputField;

        [SerializeField]
        private CanvasGroup _groupToHide;

        [SerializeField]
        private Button _okButton;

        [SerializeField]
        private Button _cancelButton;

        public string PresetName => _inputField.text;

        public async Task<Result> Show()
        {
            gameObject.SetActive(true);
            _inputField.ClearInput();
            
            _taskCompletionSource = new TaskCompletionSource<Result>();
            
            _buttonBinder.AddBinding(_okButton, OkPressed);
            _buttonBinder.AddBinding(_cancelButton, CancelPressed);

            if (_groupToHide)
            {
                _groupToHide.alpha = 0.4f;
                _groupToHide.interactable = false;
            }

            return await _taskCompletionSource.Task;
        }

        public void Close()
        {
            _buttonBinder.ClearBindings();
            gameObject.SetActive(false);

            if (_groupToHide)
            {
                _groupToHide.alpha = 1;
                _groupToHide.interactable = true;
            }
        }

        private void CancelPressed()
        {
            Close();
            _taskCompletionSource.SetResult(Result.Cancel);
        }

        private void OkPressed()
        {
            Close();
            _taskCompletionSource.SetResult(Result.Ok);
        }

        private TaskCompletionSource<Result> _taskCompletionSource;
        private readonly FlowButtonBinder _buttonBinder = new FlowButtonBinder();
        
        public enum Result
        {
            Ok,
            Cancel
        }
    }
}