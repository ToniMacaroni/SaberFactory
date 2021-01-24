using System;
using BeatSaberMarkupLanguage.Components.Settings;
using UnityEngine.Events;

namespace SaberFactory.UI.Lib
{
    internal class ToggleController
    {
        public bool Value
        {
            get => _toggle.Value;
            set => _toggle.Value = value;
        }

        private readonly ToggleSetting _toggle;
        private UnityAction<bool> _event;

        public ToggleController(ToggleSetting toggle)
        {
            _toggle = toggle;
        }

        public void SetEvent(Action<bool> action)
        {
            RemoveEvent();
            _event = new UnityAction<bool>(action);
            _toggle.toggle.onValueChanged.AddListener(_event);
        }

        public void RemoveEvent()
        {
            if(_event!=null) _toggle.toggle.onValueChanged.RemoveListener(_event);
        }
    }
}