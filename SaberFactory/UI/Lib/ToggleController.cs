using System;
using System.Linq;
using BeatSaberMarkupLanguage.Components.Settings;
using SaberFactory.Helpers;
using TMPro;
using UnityEngine.Events;

namespace SaberFactory.UI.Lib
{
    internal class ToggleController : ComponentController
    {
        public bool Value
        {
            get => Toggle.Value;
            set => Toggle.Value = value;
        }

        public readonly ToggleSetting Toggle;
        private UnityAction<bool> _event;

        public ToggleController(ToggleSetting toggle)
        {
            Toggle = toggle;
        }

        public void SetEvent(Action<bool> action)
        {
            RemoveEvent();
            _event = new UnityAction<bool>(action);
            Toggle.toggle.onValueChanged.AddListener(_event);
        }

        public override void RemoveEvent()
        {
            if (_event != null)
            {
                Toggle.toggle.onValueChanged.RemoveListener(_event);
            }
        }

        public override string GetId()
        {
            return ExternalComponents.components.First(x => true).Cast<TextMeshProUGUI>().text;
        }

        public override void SetValue(object val)
        {
            Value = (bool)val;
        }

        public override object GetValue()
        {
            return Value;
        }
    }
}