using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaberFactory.UI.Lib;
using VRUIControls;


namespace SaberFactory.UI.CustomSaber.Popups
{
    internal class MessagePopup : Popup
    {
        [UIValue("message")] protected string _message;
        [UIValue("yes-button-text")] protected string _yesButtonText;
        [UIValue("no-button-active")] protected bool _noButtonActive;

        private TaskCompletionSource<bool> _taskCompletionSource;

        public async Task<bool> Show(string message, bool yesNoOption = false)
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();

            _noButtonActive = yesNoOption;
            _yesButtonText = yesNoOption ? "Yes" : "Ok";
            _message = message;
            await Create(true);
            return await _taskCompletionSource.Task;
        }

        [UIAction("yes-click")]
        private async void OnYesClick()
        {
            await Hide(true);
            _taskCompletionSource.SetResult(true);
        }

        [UIAction("no-click")]
        private async void OnNoClick()
        {
            await Hide(true);
            _taskCompletionSource.SetResult(false);
        }
    }
}
