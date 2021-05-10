using System;
using BeatSaberMarkupLanguage.Tags;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.UI.Lib.BSML.Tags
{
    internal class PopupTag : BSMLTag
    {
        public override string[] Aliases => new[] { "this." + BSMLTools.GetKebabCaseName(_type) };

        private readonly Type _type;
        private readonly Popup.Factory _factory;

        public PopupTag(Type type, Popup.Factory factory)
        {
            _type = type;
            _factory = factory;
        }

        public override GameObject CreateObject(Transform parent)
        {
            var go = parent.CreateGameObject(_type.Name);

            go.AddComponent<RectTransform>();
            go.AddComponent<CanvasGroup>();

            var comp = _factory.Create(go, _type);

            return go;
        }
    }
}