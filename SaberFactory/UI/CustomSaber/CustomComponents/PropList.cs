using System;
using BeatSaberMarkupLanguage.Attributes;
using System.Collections.Generic;
using SaberFactory.Helpers;
using SaberFactory.UI.Lib;
using SaberFactory.UI.Lib.PropCells;
using UnityEngine;
using UnityEngine.UI;


namespace SaberFactory.UI.CustomSaber.CustomComponents
{
    internal class PropList : CustomUiComponent
    {
        [UIObject("item-container")] private readonly GameObject _itemContainer = null;

        private List<BasePropCell> _cells = new List<BasePropCell>();

        public void SetItems(IEnumerable<PropertyDescriptor> props)
        {
            Clear();
            foreach (var propertyDescriptor in props)
            {
                AddCell(propertyDescriptor);
            }
        }

        public void Clear()
        {
            foreach (Transform t in _itemContainer.transform)
            {
                t.gameObject.TryDestroy();
            }
            _cells.Clear();
        }

        public void AddCell(PropertyDescriptor data)
        {
            if (data.PropObject == null) return;

            var go = _itemContainer.CreateGameObject("PropCell");
            go.AddComponent<RectTransform>();
            var layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 16;
            layoutElement.preferredWidth = 70;

            var contentSizeFitter = go.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            //go.AddComponent<StackLayoutGroup>();

            var cellType = data.Type switch
            {
                EPropertyType.Float => typeof(FloatPropCell),
                EPropertyType.Bool => typeof(BoolPropCell),
                EPropertyType.Color => typeof(ColorPropCell),
                EPropertyType.Texture => typeof(TexturePropCell),
                _ => throw new ArgumentOutOfRangeException()
            };

            var comp = (BasePropCell)go.AddComponent(cellType);
            UIHelpers.ParseFromResource(comp.ContentLocation, go, comp);
            comp.SetData(data);
            _cells.Add(comp);
        }

        
    }
}
