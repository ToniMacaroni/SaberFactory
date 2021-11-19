using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using SaberFactory.UI.Lib.BSML;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Lib.PropCells
{
    internal class FloatPropCell : BasePropCell
    {
        [UIComponent("bg")] private readonly Image _backgroundImage = null;
        [UIComponent("val-slider")] private readonly SliderSetting _sliderSetting = null;
        [UIComponent("val-slider")] private readonly TextMeshProUGUI _sliderSettingText = null;

        public override void SetData(PropertyDescriptor data)
        {
            if (!(data.PropObject is float val))
            {
                return;
            }

            OnChangeCallback = data.ChangedCallback;

            if (data.AddtionalData is Vector2 minMax && val > minMax.x && val < minMax.y)
            {
                _sliderSetting.slider.minValue = minMax.x;
                _sliderSetting.slider.maxValue = minMax.y;
            }

            _sliderSetting.slider.value = val;
            _sliderSetting.ReceiveValue();
            _sliderSettingText.text = data.Text;

            if (ThemeManager.GetDefinedColor("prop-cell", out var bgColor))
            {
                _backgroundImage.type = Image.Type.Sliced;
                _backgroundImage.color = bgColor;
            }
        }

        [UIAction("slider-changed")]
        private void SliderChanged(float val)
        {
            OnChangeCallback?.Invoke(val);
        }
    }
}