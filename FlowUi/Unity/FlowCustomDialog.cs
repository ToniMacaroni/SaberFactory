using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FlowUi.Runtime
{
    public class FlowCustomDialog : FlowModalView
    {
        [SerializeField] private Button okButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] protected TextMeshProUGUI titleTextmesh;

        protected readonly FlowButtonBinder _buttonBinder = new FlowButtonBinder();
        private TaskCompletionSource<DialogResult> _taskCompletionSource;

        protected virtual void Awake()
        {
            _buttonBinder.AddBinding(okButton, PressedOk);
            _buttonBinder.AddBinding(cancelButton, PressedCancel);
        }

        public enum DialogResult
        {
            Ok,
            Cancel
        }

        public Action<DialogResult> DidClose;

        public virtual async Task<DialogResult> Show(string title)
        {
            _taskCompletionSource = new TaskCompletionSource<DialogResult>();
            if (titleTextmesh)
            {
                titleTextmesh.text = title;
            }
            Show();
            return await _taskCompletionSource.Task;
        }

        public virtual void PressedOk()
        {
            _taskCompletionSource?.SetResult(DialogResult.Ok);
            Hide();
        }

        public virtual void PressedCancel()
        {
            _taskCompletionSource?.SetResult(DialogResult.Cancel);
            Hide();
        }
    }
}