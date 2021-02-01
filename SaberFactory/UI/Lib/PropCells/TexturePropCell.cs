using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using SaberFactory.UI.CustomSaber.CustomComponents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Lib.PropCells
{
    internal class TexturePropCell : BasePropCell
    {
        [UIComponent("bg")] private readonly Image _backgroundImage = null;
        [UIComponent("texture")] private readonly Image _propTexture = null;
        [UIComponent("prop-name")] private readonly TextMeshProUGUI _propName = null;
        [UIComponent("texture-picker")] private readonly TexturePickerPopup _texturePicker = null;

        public override void SetData(PropertyDescriptor data)
        {
            if (!(data.PropObject is Texture2D tex)) return;

            OnChangeCallback = data.ChangedCallback;
            _propName.text = data.Text;
            _propTexture.sprite = Utilities.LoadSpriteFromTexture(tex);

            _backgroundImage.type = Image.Type.Sliced;
            _backgroundImage.color = new Color(1, 0, 0, 0.5f);
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