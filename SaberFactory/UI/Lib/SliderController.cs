using System;
using BeatSaberMarkupLanguage.Components.Settings;
using HMUI;

namespace SaberFactory.UI.Lib
{
    internal class SliderController
    {
        public float Value
        {
            get => _slider.slider.value;
            set
            {
                _slider.slider.value = value;
                _slider.ReceiveValue();
            }
        }

        public int IntValue
        {
            get => (int) Value;
            set => Value = value;
        }

        private readonly SliderSetting _slider;
        private Action<RangeValuesTextSlider, float> _currentEvent;

        public SliderController(SliderSetting slider)
        {
            _slider = slider;
        }

        public void AddEvent(Action<RangeValuesTextSlider, float> action)
        {
            if (_currentEvent is { }) return;
            _currentEvent = action;
            _slider.slider.valueDidChangeEvent += _currentEvent;
        }

        public void RemoveEvent()
        {
            if (_currentEvent is null) return;
            _slider.slider.valueDidChangeEvent -= _currentEvent;
            _currentEvent = null;
        }
    }
}