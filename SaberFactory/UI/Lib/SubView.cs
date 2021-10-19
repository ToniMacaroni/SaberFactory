using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Parser;
using JetBrains.Annotations;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Lib
{
    internal class SubView : MonoBehaviour, INotifyPropertyChanged
    {
        public virtual bool IsActive => gameObject.activeSelf;

        public BSMLParserParams ParserParams { get; private set; }

        protected virtual string _resourceName => string.Join(".", GetType().Namespace, GetType().Name);

        public SubViewSwitcher SubViewSwitcher;

        protected SiraLog Logger;
        private BsmlDecorator _bsmlDecorator;
        private bool _firstActivation = true;

        public event PropertyChangedEventHandler PropertyChanged;

        [Inject]
        private void Construct(SiraLog logger, BsmlDecorator bsmlDecorator)
        {
            _bsmlDecorator = bsmlDecorator;
            Logger = logger;
        }

        public async Task Open(bool notify = true)
        {
            if (_firstActivation)
            {
                ParserParams = await _bsmlDecorator.ParseFromResourceAsync(_resourceName, gameObject, this);

                gameObject.SetActive(false);
                _firstActivation = false;
                Init();
            }

            gameObject.SetActive(true);
            if (notify)
            {
                DidOpen();
            }
        }

        public void Close()
        {
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
            SubViewSwitcher.GoBack();
        }

        public void UpdateProps()
        {
            ParserParams.EmitEvent("update-props");
        }

        protected virtual void Init()
        {
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal class Factory : PlaceholderFactory<Type, InitData, SubView>
        {
        }

        internal struct InitData
        {
            public string Name;
            public Transform Parent;
        }
    }
}