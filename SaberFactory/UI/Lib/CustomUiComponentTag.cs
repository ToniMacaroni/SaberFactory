using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.Tags;
using SaberFactory.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SaberFactory.UI.Lib
{
    internal class CustomUiComponentTag : BSMLTag
    {
        public override string[] Aliases => new[] {"saberfactory." + _type.Name.ToLower()};

        protected string _resourceName => string.Join(".", _type.Namespace, _type.Name);

        private readonly Type _type;

        public CustomUiComponentTag(Type type)
        {
            _type = type;
        }

        public override GameObject CreateObject(Transform parent)
        {
            //var go = parent.CreateGameObject(_type.Name);
            //go.AddComponent<RectTransform>();
            //go.AddComponent<LayoutElement>();
            //go.AddComponent<ContentSizeFitter>();
            var comp = parent.gameObject.AddComponent(_type);
            UIHelpers.ParseFromResource(_resourceName, parent.gameObject, comp);
            return parent.gameObject;
        }
    }
}