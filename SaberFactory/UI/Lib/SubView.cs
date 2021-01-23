using System;
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

        public SubViewHandler SubViewHandler;

        protected virtual string _resourceName => string.Join(".", GetType().Namespace, GetType().Name);

        protected SiraLog _logger;

        [Inject]
        private async void Construct(SiraLog logger)
        {
            _logger = logger;

            ParserParams = await UIHelpers.ParseFromResourceAsync(_resourceName, gameObject, this);
            Init();
        }

        public void Open(bool notify = true)
        {
            gameObject.SetActive(true);
            if(notify) DidOpen();
        }

        public void Close()
        {
            gameObject.SetActive(false);
            DidClose();
        }

        public virtual void DidOpen()
        {

        }

        public virtual void DidClose()
        {

        }

        public void GoBack()
        {
            SubViewHandler.GoBack();
        }

        protected virtual void Init()
        {
        }

        internal class Factory : PlaceholderFactory<Type, InitData, SubView> {}

        internal struct InitData
        {
            public string Name;
            public Transform Parent;
        }
    }
}