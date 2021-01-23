using BeatSaberMarkupLanguage.Attributes;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Components.Settings;
using SaberFactory.Instances;
using SaberFactory.UI.Lib;
using UnityEngine;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class MaterialEditor : Popup
    {
        [UIComponent("material-dropdown")] private readonly DropDownListSetting _materialDropDown = null;
        [UIComponent("prop-list")] private readonly PropList _propList = null;

        [UIValue("materials")] private List<object> _materials = new List<object>();
        [UIValue("shaders")] private List<object> _shaders = new List<object>();

        [UIAction("#post-parse")]
        private void Setup()
        {
            gameObject.SetActive(false);
        }

        public void Show(MaterialDescriptor materialDescriptor)
        {
            _materialDropDown.gameObject.SetActive(false);
            SetMaterial(materialDescriptor.Material);
            gameObject.SetActive(true);
        }

        public void Show(IEnumerable<MaterialDescriptor> materialDescriptors)
        {
            _materialDropDown.gameObject.SetActive(true);
            SetMaterial(materialDescriptors.First().Material);
            gameObject.SetActive(true);
        }

        private void SetMaterial(Material material)
        {
            _propList.SetItems(material);
        }
    }
}
