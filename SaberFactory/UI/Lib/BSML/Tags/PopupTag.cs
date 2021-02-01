using System;
using System.Text.RegularExpressions;
using BeatSaberMarkupLanguage.Tags;
using SaberFactory.Helpers;
using SaberFactory.UI.CustomSaber.CustomComponents;
using UnityEngine;

namespace SaberFactory.UI.Lib.BSML.Tags
{
    internal class PopupTag : BSMLTag
    {
        public override string[] Aliases => new[] { "this." + GetKebabCaseName() };

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

            var comp = _factory.Create(go, _type);

            return go;
        }

        private string GetKebabCaseName()
        {
            return Regex.Replace(
                _type.Name,
                "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
                "-$1",
                RegexOptions.Compiled).Trim().ToLower();
        }
    }
}