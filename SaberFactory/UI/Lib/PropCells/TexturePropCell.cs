using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using SaberFactory.UI.CustomSaber.CustomComponents;
using SaberFactory.UI.Lib.BSML;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Lib.PropCells
{
    internal class TexturePropCell : BasePropCell
    {
        [UIComponent("bg")] private readonly Image _backgroundImage = null;
        [UIComponent("prop-name")] private readonly TextMeshProUGUI _propName = null;
        [UIComponent("texture")] private readonly Image _propTexture = null;
        [UIComponent("texture-picker")] private readonly TexturePickerPopup _texturePicker = null;

        public override void SetData(PropertyDescriptor data)
        {
            if (!(data.PropObject is Texture2D tex))
            {
                return;
            }

            OnChangeCallback = data.ChangedCallback;
            _propName.text = ShortenText(data.Text, 14);

            if (data.AddtionalData is bool showPreview && showPreview)
            {
                _propTexture.sprite = Utilities.LoadSpriteFromTexture(tex);
            }

            if (ThemeManager.GetDefinedColor("prop-cell", out var bgColor))
            {
                _backgroundImage.type = Image.Type.Sliced;
                _backgroundImage.color = bgColor;
            }
        }

        private string ShortenText(string text, int length)
        {
            if (text.Length < length)
            {
                return text;
            }

            return text.Substring(0, length) + "...";
        }

        [UIAction("click-select")]
        private void ClickSelect()
        {
            _texturePicker.Show(tex =>
            {
                _propTexture.sprite = Utilities.LoadSpriteFromTexture(tex);
                OnChangeCallback?.Invoke(tex);
            });
        }
    }
}