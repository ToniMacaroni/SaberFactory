using System;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using SaberFactory.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Lib.BSML.Tags
{
    internal class CustomUiComponentTag : BSMLTag
    {
        public override string[] Aliases => new[] {"sui." + BSMLTools.GetKebabCaseName(_type)};

        private readonly Type _type;
        private readonly CustomUiComponent.Factory _factory;

        public CustomUiComponentTag(Type type, CustomUiComponent.Factory factory)
        {
            _type = type;
            _factory = factory;
        }

        public override GameObject CreateObject(Transform parent)
        {
            var go = parent.CreateGameObject(_type.Name);
            go.AddComponent<RectTransform>();
            go.AddComponent<LayoutElement>();

            var contentSizeFitter = go.AddComponent<ContentSizeFitter>();
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            go.AddComponent<StackLayoutGroup>();

            var comp = _factory.Create(go, _type);
            comp.Parse();
            if(!comp.gameObject.activeSelf) comp.gameObject.SetActive(true);

            return go;
        }
    }
}