using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using HMUI;
using SaberFactory.UI.Lib;
using SaberFactory.UI.Lib.BSML;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CustomListTableData = SaberFactory.UI.Lib.BSML.CustomListTableData;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class PropList : CustomUiComponent
    {
        [UIComponent("root-vertical")] private readonly LayoutElement _layoutElement = null;
        [UIComponent("item-list")] private readonly PropListTableData _list = null;
        //[UIComponent("header-text")] private readonly TextMeshProUGUI _textMesh = null;

        public void SetItems(Material material)
        {
            SetupProps(material);
        }

        private void SetupProps(Material material)
        {
            var data = new List<PropListTableData.PropertyDescriptor>();

            var shader = material.shader;
            var propCount = shader.GetPropertyCount();

            for (int i = 0; i < propCount; i++)
            {
                var propName = shader.GetPropertyName(i);
                var propType = shader.GetPropertyType(i);
                var cell = new PropListTableData.PropertyDescriptor();
                data.Add(cell);
            }

            _list.data = data;
            _list.tableView.ReloadData();
        }

        private void SetWidth(float width)
        {
            _layoutElement.preferredWidth = width;
        }

        //private void SetText(string text)
        //{
        //    _textMesh.text = text;
        //}

        private void SetBgColor(Color color)
        {
            _layoutElement.gameObject.GetComponent<Backgroundable>().background.color = color;
        }
    }
}
