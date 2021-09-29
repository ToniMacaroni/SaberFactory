using System;
using System.Linq;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;
using SaberFactory.Helpers;
using TMPro;

namespace SaberFactory.UI.Lib
{
    internal class SliderController : ComponentController
    {
        public float Value
        {
            get => Slider.slider.value;
            set
            {
                Slider.slider.value = value;
                Slider.ReceiveValue();
            }
        }

        public int IntValue
        {
            get => (int) Value;
            set => Value = value;
        }

        public readonly SliderSetting Slider;
        private Action<RangeValuesTextSlider, float> _currentEvent;

        public SliderController(SliderSetting slider)
        {
            Slider = slider;
        }

        public void AddEvent(Action<RangeValuesTextSlider, float> action)
        {
            if (_currentEvent is { }) return;
            _currentEvent = action;
            Slider.slider.valueDidChangeEvent += _currentEvent;
        }

        public override void RemoveEvent()
        {
            if (_currentEvent is null) return;
            Slider.slider.valueDidChangeEvent -= _currentEvent;
            _currentEvent = null;
        }

        public override string GetId()
        {
            return ExternalComponents.components.First(x => x.name == "NameText").Cast<TextMeshProUGUI>().text;
        }

        public override void SetValue(object val)
        {
            Value = (float)val;
        }

        public override object GetValue()
        {
            return Value;
        }
    }
}