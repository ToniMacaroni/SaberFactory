using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.Tags;
using SaberFactory.Helpers;
using UnityEngine;
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
            var go = parent.CreateGameObject(_type.Name);
            go.AddComponent<RectTransform>();
            var comp = go.AddComponent(_type);
            UIHelpers.ParseFromResource(_resourceName, go, comp);
            return go;
        }
    }
}