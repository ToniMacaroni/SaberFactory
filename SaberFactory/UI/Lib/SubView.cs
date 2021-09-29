using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using JetBrains.Annotations;
using SaberFactory.Helpers;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace SaberFactory.UI.Lib
{
    internal class SubView : MonoBehaviour, INotifyPropertyChanged
    {
        public virtual bool IsActive => gameObject.activeSelf;

        public BSMLParserParams ParserParams { get; private set; }

        public SubViewSwitcher SubViewSwitcher;

        private bool _firstActivation = true;

        protected virtual string _resourceName => string.Join(".", GetType().Namespace, GetType().Name);

        protected SiraLog Logger;

        [Inject]
        private void Construct(SiraLog logger)
        {
            Logger = logger;
        }

        public async Task Open(bool notify = true)
        {
            if (_firstActivation)
            {
                ParserParams = await UIHelpers.ParseFromResourceAsync(_resourceName, gameObject, this);

                gameObject.SetActive(false);
                _firstActivation = false;
                Init();
            }

            gameObject.SetActive(true);
            if(notify) DidOpen();
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

        internal class Factory : PlaceholderFactory<Type, InitData, SubView> {}

        internal struct InitData
        {
            public string Name;
            public Transform Parent;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}