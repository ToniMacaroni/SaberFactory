using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
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

        public override void SetData(PropertyDescriptor data)
        {
            if (!(data.PropObject is Texture tex)) return;

            OnChangeCallback = data.ChangedCallback;
            _propName.text = data.Text;
            _propTexture.sprite = Utilities.LoadSpriteFromTexture((Texture2D)tex);

            _backgroundImage.type = Image.Type.Sliced;
            _backgroundImage.color = new Color(1, 0, 0, 0.5f);
        }
    }
}