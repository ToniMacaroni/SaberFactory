using System;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using UnityEngine;

namespace SaberFactory.UI.Lib
{
    public class PropCell : TableCell
    {
        [UIComponent("bool-container")] private readonly GameObject _boolContainer = null;
        [UIComponent("color-container")] private readonly GameObject _colorContainer = null;
        [UIComponent("float-container")] private readonly GameObject _floatContainer = null;
        [UIComponent("texture-container")] private readonly GameObject _textureContainer = null;

        public void SetData(PropertyDescriptor data)
        {
            switch (data.Type)
            {
                case EPropertyType.Float:
                    FloatSetup(data);
                    break;
                case EPropertyType.Bool:
                    BoolSetup(data);
                    break;
                case EPropertyType.Color:
                    ColorSetup(data);
                    break;
                case EPropertyType.Texture:
                    TextureSetup(data);
                    break;
            }
        }

        private void FloatSetup(PropertyDescriptor data)
        {
            _floatContainer.SetActive(true);
        }

        private void BoolSetup(PropertyDescriptor data)
        {
            _boolContainer.SetActive(true);
        }

        private void ColorSetup(PropertyDescriptor data)
        {
            _colorContainer.SetActive(true);
        }

        private void TextureSetup(PropertyDescriptor data)
        {
            _textureContainer.SetActive(true);
        }
    }
}