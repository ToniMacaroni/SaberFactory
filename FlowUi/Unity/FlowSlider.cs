using System;
using HMUI;
#if MOD
using IPA.Utilities;
#endif
using ModestTree;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FlowUi.Runtime
{
    public class FlowSlider : MonoBehaviour
    {
        public event Action<float> OnSnappedValueChanged;
        
        [SerializeField]
        private TextMeshProUGUI text;

        [SerializeField] 
        private CustomFormatRangeValuesSlider _slider;

        [SerializeField]
        private float _increment;

        public CustomFormatRangeValuesSlider Slider => _slider;
        
        public float Increment
        {
            get => _increment;
            set
            {
                _increment = value;
                UpdateSteps();
            }
        }

        public float Min
        {
            get => _slider.minValue;
            set
            {
                _slider.minValue = value;
                UpdateSteps();
            }
        }

        public float Max
        {
            get => _slider.maxValue;
            set
            {
                _slider.maxValue = value;
                UpdateSteps();
            }
        }

        public float Value
        {
            get => _slider.value;
            set => _slider.value = value;
        }

        public string Format
        {
            get => GetFormat();
            set => SetFormat(value);
        }

        public TextMeshProUGUI Text => this.text;
        
        protected void Awake()
        {
            _slider.valueDidChangeEvent += HandleValueDidChange;
        }

        protected void OnEnable()
        {
            UpdateSteps();
        }

        public void HandleValueDidChange(TextSlider slider, float val)
        {
            if (Mathf.Abs(val-_lastValue) > 0.001f)
            {
                _lastValue = val;
                OnSnappedValueChanged?.Invoke(val);
            }
        }
        
        public void UpdateSteps()
        {
            if (Mathf.Approximately(Increment, 0))
            {
                _slider.numberOfSteps = 0;
                return;
            }

            _slider.numberOfSteps = (int)((Max - Min) / Increment) + 1;
        }

        public void SetValueSilently(float value)
        {
            _slider.SetNormalizedValue(value, false);
        }

        #if MOD
        private static readonly FieldAccessor<CustomFormatRangeValuesSlider, string>.Accessor FormatAcc = FieldAccessor<CustomFormatRangeValuesSlider, string>.GetAccessor("_formatString");
        
        private string GetFormat()
        {
            return FormatAcc(ref _slider);
        }

        private void SetFormat(string format)
        {
            FormatAcc(ref _slider) = format;
        }
        #else
        private string GetFormat()
        {
            return "";
        }

        private void SetFormat(string format){}
        #endif
        
        private float _lastValue;
    }
}