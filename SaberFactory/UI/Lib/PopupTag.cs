using System;
using System.Text.RegularExpressions;
using BeatSaberMarkupLanguage.Tags;
using HMUI;
using SaberFactory.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace SaberFactory.UI.Lib
{
    internal class PopupTag : BSMLTag
    {
        public override string[] Aliases => new[] { "this." + GetKebabCaseName() };

        protected string _resourceName => string.Join(".", _type.Namespace, _type.Name);

        private readonly Type _type;

        public PopupTag(Type type)
        {
            _type = type;
        }

        public override GameObject CreateObject(Transform parent)
        {
            var go = parent.CreateGameObject(_type.Name);
            go.AddComponent<RectTransform>();
            //go.AddComponent<LayoutElement>();

            //var contentSizeFitter = go.AddComponent<ContentSizeFitter>();
            //contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            //contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            //go.AddComponent<StackLayoutGroup>();
            var comp = go.AddComponent(_type);
            UIHelpers.ParseFromResource(_resourceName, go, comp);
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