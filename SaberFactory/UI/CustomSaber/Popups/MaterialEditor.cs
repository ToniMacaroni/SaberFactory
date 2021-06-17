using System;
using BeatSaberMarkupLanguage.Attributes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Components.Settings;
using SaberFactory.Helpers;
using SaberFactory.Instances;
using SaberFactory.UI.CustomSaber.CustomComponents;
using SaberFactory.UI.Lib;
using UnityEngine;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;


namespace SaberFactory.UI.CustomSaber.Popups
{
    internal class MaterialEditor : Popup
    {
        [UIComponent("material-dropdown")] private readonly DropDownListSetting _materialDropDown = null;
        [UIComponent("prop-list")] private readonly PropList _propList = null;

        [UIValue("materials")] private List<object> _materials = new List<object>();

        public async void Show(MaterialDescriptor materialDescriptor)
        {
            if (materialDescriptor == null || materialDescriptor.Material == null)
            {
                Debug.LogError("Material was null in MaterialEditor");
                return;
            }

            _ = Create(false);
            _cachedTransform.localScale = Vector3.zero;

            _materialDropDown.transform.parent.gameObject.SetActive(false);
            SetMaterial(materialDescriptor.Material);

            await Task.Delay(100);
            await AnimateIn();
        }

        public async void Show(IEnumerable<MaterialDescriptor> materialDescriptors)
        {
            _ = Create(true);
            _cachedTransform.localScale = Vector3.zero;

            var descriptorArray = materialDescriptors.ToArray();

            _materials.Clear();
            _materials.Add(descriptorArray.Where(x=>x.Material!=null).Select(x=>x.Material.name));
            
            _materialDropDown.transform.parent.gameObject.SetActive(true);
            SetMaterial(descriptorArray.First().Material);

            await Task.Delay(100);
            await AnimateIn();
        }

        public async void Close()
        {
            await Hide(true);
        }

        private void SetMaterial(Material material)
        {
            var props = new List<PropertyDescriptor>();

            var spi = new ShaderPropertyInfo(material.shader);

            foreach (var prop in spi.GetAll())
            {
                EPropertyType type;

                if(prop.HasAttribute("HideInSF")) continue;

                if (prop.Attributes.Contains("MaterialToggle") || prop.Name == "_CustomColors")
                {
                    var floatProp = (ShaderPropertyInfo.ShaderFloat)prop;
                    type = EPropertyType.Bool;
                    var propObject = floatProp.GetValue(material) > 0;

                    void Callback(object obj)
                    {
                        material.SetFloat(prop.PropId, obj.Cast<bool>()?1:0);
                    }

                    props.Add(new PropertyDescriptor(prop.Description, type, propObject, Callback));
                }
                else
                {
                    type = GetTypeFromShaderType(prop.Type);

                    if (type == EPropertyType.Unhandled) continue;

                    var propObject = GetPropObject(prop.Type, prop.PropId, material);
                    var callback = ConstructCallback(prop.Type, prop.PropId, material);

                    var propertyDescriptor = new PropertyDescriptor(prop.Description, type, propObject, callback);

                    if (prop is ShaderPropertyInfo.ShaderRange range)
                    {
                        propertyDescriptor.AddtionalData = new Vector2(range.Min, range.Max);
                    }
                    else if (prop is ShaderPropertyInfo.ShaderTexture texProp)
                    {
                        propertyDescriptor.AddtionalData = !prop.HasAttribute("SFNoPreview");
                    }

                    props.Add(propertyDescriptor);
                }
            }

            _propList.SetItems(props);
        }

        private EPropertyType GetTypeFromShaderType(ShaderPropertyType type)
        {
            return type switch
            {
                ShaderPropertyType.Float => EPropertyType.Float,
                ShaderPropertyType.Range => EPropertyType.Float,
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
                ShaderPropertyType.Range => material.GetFloat(propId),
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
                ShaderPropertyType.Range => (obj) => { material.SetFloat(propId, (float)obj); }
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
