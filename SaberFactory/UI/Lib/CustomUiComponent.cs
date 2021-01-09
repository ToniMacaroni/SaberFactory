using System;
using BeatSaberMarkupLanguage.Parser;
using SaberFactory.Helpers;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Lib
{
    internal class CustomUiComponent : MonoBehaviour
    {
        public BSMLParserParams ParserParams { get; private set; }

        protected object _params;

        protected string _resourceName => string.Join(".", GetType().Namespace, GetType().Name);

        public async void Construct(object componentParams)
        {
            _params = componentParams;
            Initialize();
            ParserParams = await UIHelpers.ParseFromResource(_resourceName, gameObject, this);
        }

        protected virtual void Initialize() { }

        internal class Factory : PlaceholderFactory<Type, Transform, object, CustomUiComponent>{}
    }
}