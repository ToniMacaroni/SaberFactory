using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;
using HMUI;
using SaberFactory.Helpers;
using SaberFactory.UI.Components;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SaberFactory.UI.Lib.BSML
{
    [ComponentHandler(typeof(Backgroundable))]
    public class BackgroundableHandler : TypeHandler<Backgroundable>
    {
        public override Dictionary<string, string[]> Props => new Dictionary<string, string[]>
        {
            { "shader", new[]{ "shader" } }
        };

        public override Dictionary<string, Action<Backgroundable, string>> Setters => new Dictionary<string, Action<Backgroundable, string>>();

        public override void HandleTypeAfterChildren(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
        {
            base.HandleTypeAfterParse(componentType, parserParams);
            base.HandleType(componentType, parserParams);
            var backgroundable = (Backgroundable)componentType.component;
            if (componentType.data.TryGetValue("border", out var borderAttr))
            {
                if (backgroundable.background?.material != null)
                {
                    AddBorder(backgroundable.gameObject);
                }
            }
        }

        private void AddBorder(GameObject go)
        {
            if (_borderTemplate is null)
            {
                var button = Resources.FindObjectsOfTypeAll<Button>().FirstOrDefault(x => x.name == "ActionButton");
                var borderTransform = button?.transform.Find("Border");
                if (borderTransform is null) return;
                _borderTemplate = borderTransform.gameObject;
            }

            var borderGo = Object.Instantiate(_borderTemplate, go.transform).GetRect();
            borderGo.anchorMin = Vector2.zero;
            borderGo.anchorMax = Vector2.one;
            borderGo.anchoredPosition = Vector2.zero;
            borderGo.sizeDelta = Vector2.zero;

            borderGo.GetComponent<ImageView>().SetSkew(0);
        }

        private GameObject _borderTemplate;
    }
}
