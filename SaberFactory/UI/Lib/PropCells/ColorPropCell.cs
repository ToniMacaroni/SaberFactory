using System;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using SaberFactory.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Lib.PropCells
{
    internal class ColorPropCell : BasePropCell
    {
        [UIComponent("bg")] private readonly Image _backgroundImage = null;
        [UIComponent("color-setting")] private readonly ColorSetting _colorSetting = null;
        [UIComponent("color-setting")] private readonly TextMeshProUGUI _propName = null;

        public override void SetData(PropertyDescriptor data)
        {
            if (!(data.PropObject is Color color)) return;

            OnChangeCallback = data.ChangedCallback;
            _propName.text = data.Text;
            _colorSetting.CurrentColor = color;

            _backgroundImage.type = Image.Type.Sliced;
            _backgroundImage.color = new Color(1, 0, 0, 0.5f);

            var positioner = _colorSetting.modalColorPicker.gameObject.AddComponent<ModalPositioner>();
            positioner.SetPosition(new Vector2(11, 10));
        }

        [UIAction("color-changed")]
        private void ColorChanged(Color color)
        {
            OnChangeCallback?.Invoke(color);
        }

        internal class ModalPositioner : MonoBehaviour
        {
            private Vector2 _position;

            public async void OnEnable()
            {
                await Task.Delay(10);
                (transform as RectTransform).anchoredPosition = _position;
            }

            public void SetPosition(Vector2 pos)
            {
                _position = pos;
            }
        }
    }
}