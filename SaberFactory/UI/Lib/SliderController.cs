using System;
using System.Collections.Generic;
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

        public SliderController(SliderSetting slider)
        {
            _slider = slider;
        }

        public void AddEvent(Action<RangeValuesTextSlider, float> action)
        {
            _slider.slider.valueDidChangeEvent += action;
        }

        public void RemoveEvent(Action<RangeValuesTextSlider, float> action)
        {
            _slider.slider.valueDidChangeEvent -= action;
        }
    }
}