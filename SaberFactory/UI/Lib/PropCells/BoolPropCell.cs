using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using SaberFactory.UI.Lib.BSML;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Lib.PropCells
{
    internal class BoolPropCell : BasePropCell
    {
        [UIComponent("bg")] private readonly Image _backgroundImage = null;
        [UIComponent("bool-val")] private readonly ToggleSetting _toggleSetting = null;

        public override void SetData(PropertyDescriptor data)
        {
            if (!(data.PropObject is bool val)) return;

            OnChangeCallback = data.ChangedCallback;
            _toggleSetting.Value = val;
            _toggleSetting.ReceiveValue();
            _toggleSetting.text.text = data.Text;

            if (ThemeManager.GetDefinedColor("prop-cell", out var bgColor))
            {
                _backgroundImage.type = Image.Type.Sliced;
                _backgroundImage.color = bgColor;
            }
        }

        [UIAction("bool-changed")]
        private void BoolChanged(bool val)
        {
            OnChangeCallback?.Invoke(val);
        }
    }
}