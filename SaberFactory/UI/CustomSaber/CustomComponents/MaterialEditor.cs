using System;
using BeatSaberMarkupLanguage.Attributes;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Components.Settings;
using SaberFactory.Instances;
using SaberFactory.UI.Lib;
using UnityEngine;
using UnityEngine.Rendering;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class MaterialEditor : Popup
    {
        [UIComponent("material-dropdown")] private readonly DropDownListSetting _materialDropDown = null;
        [UIComponent("prop-list")] private readonly PropList _propList = null;

        [UIValue("materials")] private List<object> _materials = new List<object>();
        [UIValue("shaders")] private List<object> _shaders = new List<object>();

        public void Show(MaterialDescriptor materialDescriptor)
        {
            Show();
            _materialDropDown.transform.parent.gameObject.SetActive(false);
            SetMaterial(materialDescriptor.Material);
        }

        public void Show(IEnumerable<MaterialDescriptor> materialDescriptors)
        {
            Show();
            _materialDropDown.transform.parent.gameObject.SetActive(true);
            SetMaterial(materialDescriptors.First().Material);
        }

        public void Close()
        {
            Hide();
        }

        private void SetMaterial(Material material)
        {
            var props = new List<PropertyDescriptor>();

            var shader = material.shader;
            var propCount = shader.GetPropertyCount();

            for (int i = 0; i < propCount; i++)
            {
                var propName = shader.GetPropertyDescription(i);
                var propId = shader.GetPropertyNameId(i);
                var propType = shader.GetPropertyType(i);
                var type = GetTypeFromShaderType(propType);
                if (type == EPropertyType.Unhandled) continue;

                var propObject = GetPropObject(propType, propId, material);
                var callback = ConstructCallback(propType, propId, material);

                var cell = new PropertyDescriptor(propName, type, propObject, callback);
                props.Add(cell);
            }

            _propList.SetItems(props);
        }

        private EPropertyType GetTypeFromShaderType(ShaderPropertyType type)
        {
            return type switch
            {
                ShaderPropertyType.Float => EPropertyType.Float,
                ShaderPropertyType.Color => EPropertyType.Color,
                ShaderPropertyType.Texture => EPropertyType.Texture,
                _ => EPropertyType.Unhandled
            };
        }

        private object GetPropObject(ShaderPropertyType type, int propId, Material material)
        {
            return type switch
            {
                ShaderPropertyType.Float => material.GetFloat(propId),
                ShaderPropertyType.Color => material.GetColor(propId),
                ShaderPropertyType.Texture => material.GetTexture(propId),
                _ => null
            };
        }

        private Action<object> ConstructCallback(ShaderPropertyType type, int propId, Material material)
        {
            return type switch
            {
                ShaderPropertyType.Float => (obj) => { material.SetFloat(propId, (float)obj); }
                ,
                ShaderPropertyType.Color => (obj) => { material.SetColor(propId, (Color)obj); }
                ,
                ShaderPropertyType.Texture => (obj) => { material.SetTexture(propId, (Texture2D)obj); }
                ,
                _ => null
            };
        }

        [UIAction("click-close")]
        private void ClickClose()
        {
            Close();
        }
    }
}
