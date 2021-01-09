using System;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Parser;
using SaberFactory.Helpers;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Lib
{
    internal class SubView : MonoBehaviour
    {
        public virtual bool IsActive => gameObject.activeSelf;

        public BSMLParserParams ParserParams { get; private set; }

        protected virtual string _resourceName => string.Join(".", GetType().Namespace, GetType().Name);

        protected SiraLog _logger;
        private CustomUiComponent.Factory _componentFactory;

        [Inject]
        private async void Construct(SiraLog logger, CustomUiComponent.Factory componentFactory)
        {
            _logger = logger;
            _componentFactory = componentFactory;

            await Init();
        }

        public virtual void Open()
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }

        protected T CreateComponent<T>(Transform parent, object componentParams) where T : CustomUiComponent
        {
            return (T)_componentFactory.Create(typeof(T), parent, componentParams);
        }

        private async Task Init()
        {
            ParserParams = await UIHelpers.ParseFromResource(_resourceName, gameObject, this);
        }

        internal class Factory : PlaceholderFactory<Type, InitData, SubView> {}

        internal struct InitData
        {
            public string Name;
            public Transform Parent;
        }
    }
}